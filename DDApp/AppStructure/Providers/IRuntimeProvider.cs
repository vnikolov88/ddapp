using System;
using System.Threading.Tasks;

namespace DDApp.AppStructure.Providers
{
    public interface IRuntimeProvider
    {
        void RegisterRuntimeTypes(DataDrivenApp app);
        Type GetTypeInApp(string typeName);
        Task<object> CreateExecutionAsync(string lambdaExpression, Type instanceType, Type resultType, object globals = null);
    }
}
