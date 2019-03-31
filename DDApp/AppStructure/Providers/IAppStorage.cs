using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDApp.AppStructure.Providers
{
    public interface IAppStorage
    {
        IChangeToken GetAppChangeToken(string appCode);
        IEnumerable<string> GetAllAppCodes();
        Task<string> GetAppAsync(string appCode);
        Task SetAppAsync(string appCode, string appContent);
        Task<IList<string>> GetAppModulesAsync(string appCode);
    }
}
