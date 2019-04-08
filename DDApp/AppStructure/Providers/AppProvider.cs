using DDApp.Extensions;
using DDApp.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace DDApp.AppStructure.Providers
{
    public class AppProvider : IAppProvider
    {
        private readonly TimeSpan _appPageTTL = TimeSpan.FromHours(160);
        private readonly JsonSerializerSettings _jsonSettings;
        private readonly IServerState _serverState;
        private readonly IMemoryCache _memoryCache;
        private readonly IAppStorage _appStorage;
        private readonly ILogger _logger;

        public AppProvider(
            IAppStorage appStorage,
            IServerState serverState,
            IMemoryCache memoryCache,
            ILogger<AppProvider> logger
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appStorage = appStorage ?? throw new ArgumentNullException(nameof(appStorage));
            _serverState = serverState ?? throw new ArgumentNullException(nameof(serverState));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
        }

        /// <summary>
        /// Return a DataDrivenApp from the storage without hydrating the components
        /// </summary>
        /// <param name="appCode">the short name of the application</param>
        /// <returns>the application or a DataDrivenApp.Empty application</returns>
        public async Task<DataDrivenApp> GetAppAsync(string appCode)
        {
            try
            {
                if (_memoryCache.TryGetValue(appCode, out DataDrivenApp app))
                    return app;

                var changeToken = _appStorage.GetAppChangeToken(appCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .AddExpirationToken(changeToken);

                var appJson = await _appStorage.GetAppAsync(appCode).ConfigureAwait(false);

                app = JsonConvert.DeserializeObject<DataDrivenApp>(appJson, _jsonSettings);
                #region Load app modules
                foreach(var moduleName in app.Modules)
                {
                    var moduleChangeToken = _appStorage.GetAppModuleChangeToken(moduleName);
                    cacheEntryOptions.AddExpirationToken(moduleChangeToken);
                    var module = await _appStorage.GetAppModuleAsync(moduleName).ConfigureAwait(false);
                    app.LoadAppModule( JsonConvert.DeserializeObject<DataDrivenApp>(module, _jsonSettings) );
                }
                #endregion Load app modules
                _memoryCache.Set(appCode, app, cacheEntryOptions);
                return app;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"No app found with code {appCode}");
                return DataDrivenApp.Empty;
            }
        }

        /// <summary>
        /// Return the AppContext for the given app or creates it if the app exist.
        /// </summary>
        /// <param name="appCode">the short name of the application</param>
        /// <returns>the application context or null</returns>
        public async Task<AppContext> GetAppContextAsync(string appCode)
        {
            try
            {
                if (_memoryCache.TryGetValue($"context:{appCode}", out AppContext appContext))
                    return appContext;

                var changeToken = _appStorage.GetAppChangeToken(appCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .AddExpirationToken(changeToken);

                var runtimeProvider = new RuntimeProvider();
                var modelProvider = new ModelProvider(_logger, _memoryCache, runtimeProvider);
                var app = await GetAppAsync(appCode);
                appContext = AppContext.Create(app, modelProvider, runtimeProvider);

                _memoryCache.Set($"context:{appCode}", appContext, cacheEntryOptions);
                return appContext;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"No app context found with code {appCode}");
                return null;
            }
        }

        /// <summary>
        /// Look for all present apps and try to preload them
        /// </summary>
        /// <returns>TaskAwaiter</returns>
        public async Task PreCacheAppsAsync()
        {
            var appCodes = _appStorage.GetAllAppCodes();
            foreach (var appCode in appCodes)
            {
                await GetAppAsync(appCode).ConfigureAwait(false);
                await GetAppContextAsync(appCode).ConfigureAwait(false);
                _logger.LogInformation($"PreCached app {appCode}");
            }
        }

        /// <summary>
        /// Return a page from a application already in storage
        /// Note: this will hydrate the page components
        /// </summary>
        /// <param name="appCode">the short name of the application</param>
        /// <param name="pageCode">the name of the page</param>
        /// <param name="query">the query parameters passed when loading the page</param>
        /// <returns>the hydrated page or a DataDrivenAppPage.Empty page</returns>
        public async Task<HydratedAppPage> GetAppPageAsync(
            string appCode,
            string pageCode,
            string query)
        {
            var appPageKey = $"DDApp:{appCode}:{pageCode}:{query}";
            if (_memoryCache.TryGetValue(appPageKey, out HydratedAppPage hydratedPage))
                return hydratedPage;

            var app = await GetAppAsync(appCode).ConfigureAwait(false);
            var appContext = await GetAppContextAsync(appCode).ConfigureAwait(false);

            hydratedPage = await HydrateAsync(app, appContext, pageCode, query);

            if (hydratedPage?.CanCache == true)
            {
                var changeToken = _appStorage.GetAppChangeToken(appCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .AddExpirationToken(changeToken)
                        .SetSlidingExpiration(_appPageTTL);
                foreach (var moduleName in app.Modules)
                {
                    var moduleChangeToken = _appStorage.GetAppModuleChangeToken(moduleName);
                    cacheEntryOptions.AddExpirationToken(moduleChangeToken);
                }
                _memoryCache.Set(appPageKey, hydratedPage, cacheEntryOptions);
            }

            return hydratedPage;
        }

        /// <summary>
        /// Return the GUID for the given app page, a number that will change if the page is modified in the configuration.
        /// </summary>
        /// <param name="appCode">the short name of the application</param>
        /// <param name="pageCode">the name of the page</param>
        /// <returns>the GUID for the given app page or GUID_Empty if the app or the page does not exist</returns>
        public async Task<ulong> GetAppPageVersionAsync(string appCode, string pageCode)
        {
            var app = await GetAppAsync(appCode).ConfigureAwait(false);
            if (app == null || !app.Pages.ContainsKey(pageCode)) return WithGUID.GUID_Empty;

            var appPage = app.Pages[pageCode];

            return appPage.GUID.MixGUID(_serverState.GetViewsPackageVersion());
        }

        public async Task RefreshAppStoreAsync(string appCode)
        {
            if (_memoryCache.TryGetValue(appCode, out DataDrivenApp app))
            {
                await _appStorage.SetAppAsync(appCode, JsonConvert.SerializeObject(app, _jsonSettings)).ConfigureAwait(false);
            }
        }

        public async Task CreateAppAsync(string appCode)
        {
            if(string.IsNullOrWhiteSpace(await _appStorage.GetAppAsync(appCode)))
                await _appStorage.SetAppAsync(appCode, JsonConvert.SerializeObject(DataDrivenApp.Empty, _jsonSettings)).ConfigureAwait(false);
        }

        private async Task<HydratedAppPage> HydrateAsync(
            DataDrivenApp app,
            IAppContext appContext,
            string pageCode,
            string query)
        {
            if (app == null || appContext == null || !app.Pages.ContainsKey(pageCode))
                return HydratedAppPage.Empty;

            var appPage = app.Pages[pageCode];

            var queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(query);

            var hydratedPage = new HydratedAppPage(appPage)
            {
                // Pass down app properties
                AppLogo = app.Logo,
                AppQuickCallNumber = app.QuickCallNumber,
                AppQuickCallNumberIcon = app.QuickCallNumberIcon,
                AppQuickCallNumberText = app.QuickCallNumberText,
                Navigation = app.Navigation
            };
            
            foreach (var component in appPage.Components)
            {
                try
                {
                    hydratedPage.Components.Add(await appContext.ModelProvider.HydrateAsync(component, queryDictionary).ConfigureAwait(false));
                }
                catch(AutoMapper.AutoMapperMappingException ex)
                {
                    hydratedPage.CanCache = false;
                    hydratedPage.Components.Add(new RenderModels.ErrorIndicator
                    {
                        MainEvent = GetMappingExceptionInfo(ex),
                        InnerEvent = ex.GetBaseException()?.Message,
                        StackTrace = ex.StackTrace,
                    });
                }
                catch(Exception ex)
                {
                    hydratedPage.CanCache = false;
                    hydratedPage.Components.Add(new RenderModels.ErrorIndicator {
                        MainEvent = ex.Message,
                        InnerEvent = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace,
                    });
                }
            }
            return hydratedPage;
        }

        private string GetMappingExceptionInfo(AutoMapper.AutoMapperMappingException ex)
        {
            var currentError = string.Empty;
            var nextError = string.Empty;
            var innerEx = (ex.InnerException as AutoMapper.AutoMapperMappingException);
            if (innerEx != null)
                nextError += GetMappingExceptionInfo(innerEx);

            if (ex.TypeMap != null)
            {
                currentError = $"Error in Mapping {ex.TypeMap.Types.SourceType} -> {ex.TypeMap.Types.DestinationType}\nFor property:\n{ex.PropertyMap?.DestinationProperty?.Name}\n\n";
            }
            return currentError + nextError;
        }
    }
}
