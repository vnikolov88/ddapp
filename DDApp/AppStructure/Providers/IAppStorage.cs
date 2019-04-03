using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDApp.AppStructure.Providers
{
    public interface IAppStorage
    {
        IChangeToken GetAppChangeToken(string appCode);
        IEnumerable<string> GetAllAppCodes();
        IEnumerable<string> GetAllAppModuleNames();
        Task<string> GetAppAsync(string appCode);
        Task SetAppAsync(string appCode, string appContent);
        Task<string> GetAppModuleAsync(string moduleName);
    }
}
