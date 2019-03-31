using DDApp.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using EntityTagHeaderValue = System.Net.Http.Headers.EntityTagHeaderValue;

namespace DDApp.Controllers
{
    class CachedProxyResponse
    {
        public ReadOnlyMemory<byte> Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
        public EntityTagHeaderValue ETag { get; set; }
    }

    [Route("/proxyremote")]
    public class ProxyRemoteController : Controller
    {
        private readonly TimeSpan _remoteUrlTTL = TimeSpan.FromDays(20);
        private readonly IMemoryCache _memoryCache;

        public ProxyRemoteController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        [HttpGet("{*base64Url}")]
        public async Task Get(string base64Url)
        {
            var responseLocal = await GetRemoteResponseAsync(base64Url).ConfigureAwait(false);

            if (HttpContext.Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out var etag) &&
                etag == responseLocal.ETag.Tag)
            {
                Response.StatusCode = (int) HttpStatusCode.NotModified;
            }
            else
            {
                Response.StatusCode = (int) responseLocal.StatusCode;
                Response.ContentLength = responseLocal.ContentLength;
                Response.ContentType = responseLocal.ContentType;
                Response.Headers.Add(HeaderNames.ETag, responseLocal.ETag.ToString());
                await Response.Body.WriteAsync(responseLocal.Content).ConfigureAwait(false);
            }
        }

        private async Task<CachedProxyResponse> GetRemoteResponseAsync(string base64Url)
        {
            return await _memoryCache.GetOrCreateAsync(base64Url, async entry =>
            {
                entry.SlidingExpiration = _remoteUrlTTL;

                var requestUrl = new Uri(base64Url.Base64Decode(), UriKind.RelativeOrAbsolute);
                var remoteUrl = requestUrl.IsAbsoluteUri ?
                    requestUrl :
                    // Note: proxying local relative requests does not seem right, think of possible scenarios
                    new Uri(new Uri($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}"), requestUrl);

                var request = (HttpWebRequest)WebRequest.Create(remoteUrl);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                using (var responseRemote = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false))
                using (var stream = responseRemote.GetResponseStream())
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    var etag = responseRemote.GetResponseHeader(HeaderNames.ETag);

                    return new CachedProxyResponse
                    {
                        Content = memoryStream.ToArray(),
                        ContentType = responseRemote.ContentType,
                        ContentLength = responseRemote.ContentLength,
                        StatusCode = responseRemote.StatusCode,
                        ETag = string.IsNullOrWhiteSpace(etag) ? EntityTagHeaderValue.Any : EntityTagHeaderValue.Parse(etag)
                    };
                }
            }).ConfigureAwait(false);
        }
    }
}
