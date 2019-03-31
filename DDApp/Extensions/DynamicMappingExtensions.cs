using AutoMapper;
using DDApp.AppStructure;
using DDApp.AppStructure.Providers;
using DDApp.DynamicMapping;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace DDApp.Extensions
{

    public static class DynamicMappingExtensions
    {
        private const string QueryContextKey = "__QueryContext_";

        #region Pre Computed
        private static readonly Type _mappingContext = typeof(MappingContext<>);
        private static readonly Type _reducerContext = typeof(ReducerContext<>);
        private static readonly Type _typeofObject = typeof(object);
        private static readonly Type _expression = typeof(Expression<>);
        private static readonly Type _action = typeof(Action<>);
        private static readonly Type _func = typeof(Func<,>);
        private static readonly Type _func2Params = typeof(Func<,,>);
        private static readonly Type _mappingExpression = typeof(IMappingExpression<,>);
        private static readonly Type _memberConfigurationExpression = typeof(IMemberConfigurationExpression<,,>);
        private static readonly MethodInfo _createMap = typeof(IProfileExpression).GetMethod("CreateMap", 2, new Type[] { typeof(MemberList) });
        #endregion Pre Computed

        public static object MapWithQueryContext(
            this IMapper mapper,
            object source,
            Type sourceType,
            Type destinationType,
            object queryContext)
        {
            return mapper.Map(
                source,
                sourceType,
                destinationType,
                opt => opt.Items[QueryContextKey] = queryContext
                );
        }

        public static async Task CreateMapAsync(this IProfileExpression cfg,
            DataProviderMapping mapping,
            Type sourceType,
            Type destinationType,
            IRuntimeProvider runtimeProvider)
        {
            // Create default map
            var map = (dynamic)_createMap
                .MakeGenericMethod(sourceType, destinationType)
                .Invoke(cfg, new object[] { MemberList.None });

            // Continue with custom mappings
            if (mapping.Mapping == null) return;
            
            var destinationMappingResultType = _expression.MakeGenericType(_func.MakeGenericType(destinationType, _typeofObject));
            
            var mapFromResultType = _action.MakeGenericType(_memberConfigurationExpression.MakeGenericType(sourceType, destinationType, _typeofObject));

            var forMember = _mappingExpression
                .MakeGenericType(sourceType, destinationType)
                .GetMethods().SingleOrDefault(x => x.Name == "ForMember" && x.IsGenericMethod)
                .MakeGenericMethod(_typeofObject);

            var mappingContextType = _mappingContext.MakeGenericType(sourceType);
            var mappingContext = Activator.CreateInstance(mappingContextType);
            // define custom mappings
            foreach (var customMapping in mapping.Mapping)
            {
                var destinationMapping = await runtimeProvider.CreateExecutionAsync(customMapping.Key, sourceType, destinationMappingResultType);
                
                var mapFrom = await runtimeProvider.CreateExecutionAsync(
                    $"_o => _o.ResolveUsing((_sc, _dest, _destMember, _context) => _GetExecutionTree(_sc, _context.Items.ContainsKey(\"{QueryContextKey}\") ? _context.Items[\"{QueryContextKey}\"] : null, {customMapping.Value}) )",
                    sourceType,
                    mapFromResultType,
                    mappingContext);

                map = forMember.Invoke(map, new object[] { destinationMapping, mapFrom });
                // Note: Force GC to execute to reduce RAM usage to minimum on Claud HW
                GC.Collect();
            }
        }

        public static async Task<IReducerContext> CreateReducerContextAsync(
            this DataProvider provider,
            Type instanceType,
            IRuntimeProvider runtimeProvider)
        {
            var mappingContextType = _mappingContext.MakeGenericType(instanceType);
            var mappingContext = Activator.CreateInstance(mappingContextType);

            var resultType = _func2Params.MakeGenericType(instanceType, _typeofObject, _typeofObject);

            var lambdaMethod = await runtimeProvider.CreateExecutionAsync(
                $"(_sc, _qc) => _GetExecutionTree(_sc, _qc != null ? _qc : null, {provider.Reducer})",
                instanceType,
                resultType,
                mappingContext);

            var reducerContextType = _reducerContext.MakeGenericType(instanceType);
            var reducerContext = Activator.CreateInstance(reducerContextType, lambdaMethod) as IReducerContext;

            return reducerContext;
        }

        /// <summary>
        /// Create a dynamic object based on the dictionary structure provided
        /// </summary>
        /// <param name="self">the query parameters</param>
        /// <returns>dynamic object with properties coresponding to the query parameters</returns>
        public static object CreateQueryContext(this IDictionary<string, StringValues> self)
        {
            var eo = new ExpandoObject();
            var eoColl = (ICollection<KeyValuePair<string, object>>)eo;

            foreach (var kvp in self)
            {
                eoColl.Add(new KeyValuePair<string, object>(kvp.Key, kvp.Value));
            }
            return eo;
        }
    }
}
