using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace DDApp.Infrastructure
{
    public class RedisCache : IRedisCache
    {
        private readonly IConnectionMultiplexer _connectionMultiplaxer;

        public RedisCache(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplaxer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        }

        public async Task StringSetAsync(string key, string val)
        {
            var conn = _connectionMultiplaxer.GetDatabase();
            await conn.StringSetAsync(key, val);
        }

        public async Task<string> StringGetAsync(string key)
        {
            var conn = _connectionMultiplaxer.GetDatabase();
            return await conn.StringGetAsync(key);
        }
    }
}
