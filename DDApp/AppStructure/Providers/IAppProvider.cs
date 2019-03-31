using System.Threading.Tasks;

namespace DDApp.AppStructure.Providers
{
    public interface IAppProvider
    {
        Task<DataDrivenApp> GetAppAsync(string appCode);
        Task<AppContext> GetAppContextAsync(string appCode);
        Task<HydratedAppPage> GetAppPageAsync(string appCode, string page, string query);
        Task<ulong> GetAppPageVersionAsync(string appCode, string pageCode);
        Task RefreshAppStoreAsync(string appCode);
        Task CreateAppAsync(string appCode);
        Task PreCacheAppsAsync();
    }
}
