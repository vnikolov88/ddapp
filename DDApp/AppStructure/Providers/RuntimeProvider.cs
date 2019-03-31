using AutoMapper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DDApp.AppStructure.Providers
{
    // Note: We need to add late bounding of data types to support circuler reference
    public class RuntimeProvider : IRuntimeProvider
    {
        #region Pre Computed

        private static readonly Type _script = typeof(Script<>);
        private static readonly Type[] _scriptCreateParamTypes = new Type[] { typeof(string), typeof(ScriptOptions), typeof(Type), typeof(InteractiveAssemblyLoader) };
        private static readonly MethodInfo _scriptCreateMethod = typeof(CSharpScript).GetMethod("Create", 1, _scriptCreateParamTypes);
        private static readonly Type[] _scriptRunAsyncParamTypes = new Type[] { typeof(object), typeof(CancellationToken) };
        
        #endregion Pre Computed

        #region Default context

        // Note: every instance of the RuntimeProvider creates it's own assemblies and loads them in to the default AssemblyLoadContext,
        // We have to add support for the unloading of the old assemblies when .Net Core 3.0 is out with the support for dynamic unloading
        private readonly string _runtimeContextName = Path.GetRandomFileName();

        private static readonly IEnumerable<string> _usings = new[] {
            "System",
            "System.IO",
            "System.Net",
            "System.Linq",
            "System.Text",
            "Newtonsoft.Json",
            "DDApp.Extensions",
            "DDApp.Infrastructure",
            "System.Collections",
            "System.Xml.Serialization",
            "System.Collections.Generic",
            "System.Text.RegularExpressions",
            "DDApp.AppStructure.RenderModels",
        };

        public ScriptOptions ScriptingOptions { get; set; } = ScriptOptions.Default.AddReferences(typeof(RuntimeProvider).Assembly)
            .WithImports(_usings);

        #endregion Default context

        public RuntimeProvider()
        {
            #region Load static assemblies

            IEnumerable<MetadataReference> allReferences = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location))
                .Select(x => MetadataReference.CreateFromFile(x.Location))
                .ToArray();
            
            ScriptingOptions = ScriptingOptions.AddReferences(allReferences);

            #endregion Load static assemblies
        }

        #region IRuntimeProvider

        public async Task<object> CreateExecutionAsync(
            string lambdaExpression,
            Type instanceType,
            Type resultType,
            object globals = null)
        {
            var scriptCreateMethod = _scriptCreateMethod.MakeGenericMethod(resultType);

            var scriptRunAsyncMethod = _script.MakeGenericType(resultType)
                .GetMethod("RunAsync", _scriptRunAsyncParamTypes);

            using (var interactiveAssemblyLoader = new InteractiveAssemblyLoader())
            {
                interactiveAssemblyLoader.RegisterDependency(instanceType.Assembly);

                var scriptState = scriptCreateMethod.Invoke(null, new object[] {
                    lambdaExpression,
                    ScriptingOptions,
                    globals?.GetType(),
                    interactiveAssemblyLoader,
                });

                var lambdaMethodScript = await (dynamic)scriptRunAsyncMethod.Invoke(scriptState, new object[] {
                    globals,
                    default(CancellationToken)
                });

                return lambdaMethodScript.ReturnValue;
            }
        }

        public void RegisterRuntimeTypes(DataDrivenApp app)
        {
            foreach(var dataType in app.DataTypes)
            {
                RegisterRuntimeType(dataType.Key, dataType.Value);
            }
        }

        public Type GetTypeInApp(string typeName)
        {
            var assembly = GetAppDomainAssemblyByName(GetRuntimeTypeAssemblyName(typeName));
            var objectType = assembly.GetType(typeName);

            return objectType;
        }

        #endregion IRuntimeProvider

        private void RegisterRuntimeType(string typeName, IDataDrivenAppDataType dataType)
        {
            var classDefinition = CreateClassDefinition(typeName, dataType);
            
            var assemblyName = GetRuntimeTypeAssemblyName(typeName);
            var lib = CreateCompilationWithDefaultReferences(
                assemblyName,
                classDefinition,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release)
                    .WithUsings(_usings)
                );

            var libImage = EmitToArray(lib);

            MetadataReference libRef = AssemblyMetadata
                .CreateFromImage(libImage)
                .GetReference(display: $"{assemblyName}.dll");

            ScriptingOptions = ScriptingOptions.AddReferences(libRef);

            // Load the assembly in to the default load context
            using (var stream = new MemoryStream(libImage))
            {
                System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(stream);
            }
        }

        #region Auxiliary
        
        private string CreateClassDefinition(
            string typeName,
            IDataDrivenAppDataType dataType)
        {
            var usingsSignature = string.Join("\n", _usings.Select(x => $"using {x};"));

            var propertiesSignature = string.Join("\n", dataType.Properties.Select(
                x => $"{string.Join(" ", x.Value.Attributes)} public {x.Value.Type} {x.Key} {{ get; set; }}"));

            var methodsSignature = string.Join("\n", dataType.Methods.Select(x => $"public dynamic {x.Key}{x.Value};"));

            var classAttributes = string.Join("\n", dataType.Attributes);

            return $"{usingsSignature}\n{classAttributes}public class {typeName} {{ {propertiesSignature}\n{methodsSignature}}}";
        }

        private string GetRuntimeTypeAssemblyName(string typeName) => $"{typeName}_{_runtimeContextName}";

        private static Assembly GetAppDomainAssemblyByName(string name)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var tests = assemblies.Select(x => x.GetName()).ToList();
            var assembly = assemblies.SingleOrDefault(x => x.GetName().Name == name);
            if (assembly == null)
                throw new AppDomainUnloadedException($"{name} is not loaded in the currect app domain.");

            return assembly;
        }

        private CSharpCompilation CreateCompilationWithDefaultReferences(
            string assemblyOrModuleName,
            string code,
            CSharpCompilationOptions compilerOptions = null,
            IEnumerable<MetadataReference> references = null)
        {
            // create the syntax tree
            SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(code,
                new CSharpParseOptions(kind: SourceCodeKind.Regular));

            var allReferences = ScriptingOptions.MetadataReferences
                .Where(x => !(x is UnresolvedMetadataReference));

            if (references != null)
            {
                allReferences = allReferences.Concat(references);
            }
            
            // create and return the compilation
            CSharpCompilation compilation = CSharpCompilation.Create
            (
                assemblyOrModuleName,
                new[] { syntaxTree },
                options: compilerOptions,
                references: allReferences
            );

            return compilation;
        }

        // emit the compilation result into a byte array.
        // throw an exception with corresponding message
        // if there are errors
        private byte[] EmitToArray(Compilation compilation)
        {
            using (var stream = new MemoryStream())
            {
                // emit result into a stream
                var emitResult = compilation.Emit(stream);

                if (!emitResult.Success)
                {
                    // if not successful, throw an exception
                    Diagnostic firstError =
                        emitResult
                            .Diagnostics
                            .FirstOrDefault
                            (
                                diagnostic =>
                                    diagnostic.Severity == DiagnosticSeverity.Error
                            );

                    throw new Exception(firstError?.GetMessage());
                }
                // get the byte array from a stream
                return stream.ToArray();
            }
        }

        #endregion Auxiliary
    }
}
