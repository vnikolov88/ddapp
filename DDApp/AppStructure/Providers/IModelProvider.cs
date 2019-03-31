using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace DDApp.AppStructure.Providers
{
    public interface IModelProvider
    {
        Task<dynamic> HydrateAsync(DataDrivenAppComponent component, IDictionary<string, StringValues> query);
        void BuildAndCacheMappings(DataDrivenApp app);

        IList<string> RenderModels { get; }
    }
}
