using System.Threading.Tasks;

namespace DDApp.Infrastructure
{
    public interface IRedisCache
    {
        Task StringSetAsync(string key, string val);
        Task<string> StringGetAsync(string key);
    }
}
