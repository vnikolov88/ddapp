using DDApp.AppStructure.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DDApp.Controllers
{
    [Route("/", Order = 1)]
    public class AppController : Controller
    {
        private readonly IAppProvider _appProvider;

        public AppController(IAppProvider appProvider)
        {
            _appProvider = appProvider ?? throw new ArgumentNullException(nameof(appProvider));
        }

        private async Task<IActionResult> GetPageAsync(string appCode, string pageCode, bool isPartial)
        {
            var etagHash = await _appProvider.GetAppPageVersionAsync(appCode, pageCode);
            var etagCurrent = $"\"{etagHash}\"";
            // Check if we have a custom page version header
            if (HttpContext.Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out var etag) &&
                etag == etagCurrent)
            {
                return StatusCode((int)HttpStatusCode.NotModified);
            }

            var query = HttpContext.Request.QueryString.ToString();
            var model = await _appProvider.GetAppPageAsync(appCode, pageCode, query);
            if(model?.CanCache == true)
                Request.HttpContext.Response.Headers.Add(HeaderNames.ETag, etagCurrent);

            if (isPartial)
            {
                return model.TabGroups?.Count > 1 ? PartialView("PageTabbed", model) : PartialView("Page", model);
            }

            return model.TabGroups?.Count > 1 ? View("PageTabbed", model) : View("Page", model);
        }

        [Route("{appCode}/{pageCode}")]
        public async Task<IActionResult> Page(string appCode, string pageCode)
        {
            return await GetPageAsync(appCode, pageCode, false);
        }

        [Route("partial/{appCode}/{pageCode}")]
        public async Task<IActionResult> PagePartial(string appCode, string pageCode)
        {
            return await GetPageAsync(appCode, pageCode, true);
        }
    }
}