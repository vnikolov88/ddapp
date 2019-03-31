using DDApp.AppStructure.Providers;
using System;

namespace DDApp.AppStructure
{
    public class AppContext : IAppContext
    {
        public IRuntimeProvider RuntimeProvider { get; protected set; }
        public IModelProvider ModelProvider { get; protected set; }
        
        private AppContext(IModelProvider modelProvider, IRuntimeProvider runtimeProvider)
        {
            RuntimeProvider = runtimeProvider ?? throw new ArgumentNullException(nameof(runtimeProvider));
            ModelProvider = modelProvider ?? throw new ArgumentNullException(nameof(modelProvider));;
        }

        public static AppContext Create(
            DataDrivenApp app,
            IModelProvider modelProvider,
            IRuntimeProvider runtimeProvider)
        {
            var context = new AppContext(modelProvider, runtimeProvider);
            context.RuntimeProvider.RegisterRuntimeTypes(app);
            context.ModelProvider.BuildAndCacheMappings(app);

            return context;
        }
    }
}
