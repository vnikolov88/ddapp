using DDApp.Infrastructure;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDApp.AppStructure.Providers
{
    public class RedisAppStorage : IAppStorage
    {
        private readonly IRedisCache _cache;

        public RedisAppStorage(IRedisCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }
        
        public IChangeToken GetAppChangeToken(string appCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAllAppCodes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAllAppModuleNames()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetAppAsync(string appCode)
        {
            var appKey = $"DDApp:{appCode}";
            return await _cache.StringGetAsync(appKey);
        }

        public async Task SetAppAsync(string appCode, string appContent)
        {
            var appKey = $"DDApp:{appCode}";
            await _cache.StringSetAsync(appKey, appContent);
        }

        public async Task<string> GetAppModuleAsync(string moduleName)
        {
            throw new NotImplementedException();
        }
    }
}
