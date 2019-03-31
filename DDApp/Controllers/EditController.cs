using DDApp.AppStructure;
using DDApp.AppStructure.Providers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDApp.Controllers
{
    [Route("edit/")]
    public class EditController : Controller
    {
        private readonly IAppProvider _appProvider;

        public EditController(IAppProvider appProvider, IHostingEnvironment env)
        {
            _appProvider = appProvider ?? throw new ArgumentNullException(nameof(appProvider));
        }

        #region Create functionality

        [Route("create/{appCode}")]
        public async Task<IActionResult> CreateApp(string appCode)
        {
            await _appProvider.CreateAppAsync(appCode);
            return RedirectToAction("EditApp");
        }


        [HttpPost("createpage/{appCode}")]
        public async Task<IActionResult> CreateAppPage(string appCode, string page)
        {
            var app = await _appProvider.GetAppAsync(appCode);
            app.Pages.Add(page, DataDrivenAppPage.Empty);
            await _appProvider.RefreshAppStoreAsync(appCode);
            return RedirectToAction("EditPage", new { page });
        }

        [Route("createcomponent/{appCode}/{page}")]
        public async Task<IActionResult> CreateAppComponent(string appCode, string page)
        {
            var app = await _appProvider.GetAppAsync(appCode);
            var pageObject = app.Pages[page];
            if (pageObject.Components == null)
                pageObject.Components = new List<DataDrivenAppComponent>();
            pageObject.Components.Add(DataDrivenAppComponent.Empty);
            await _appProvider.RefreshAppStoreAsync(appCode);
            return RedirectToAction("EditPage");
        }

        #endregion Create functionality
        
        #region Edit functionality

        [Route("{appCode}")]
        public async Task<IActionResult> EditApp(string appCode)
        {
            var model = await _appProvider.GetAppAsync(appCode);
            ViewBag.AppCode = appCode;

            return View(model);
        }

        [HttpPost("{appCode}")]
        public async Task<IActionResult> EditAppPost(string appCode, string page, DataDrivenApp newModel)
        {
            var app = await _appProvider.GetAppAsync(appCode);
            app.Logo = newModel.Logo;
            await _appProvider.RefreshAppStoreAsync(appCode);
            return RedirectToAction("EditApp");
        }

        [Route("{appCode}/{page}")]
        public async Task<IActionResult> EditPage(string appCode, string page)
        {
            var app = await _appProvider.GetAppAsync(appCode);
            var model = app.Pages[page];
            ViewBag.AppCode = appCode;
            ViewBag.Page = page;

            return View(model);
        }

        [HttpPost("{appCode}/{page}")]
        public async Task<IActionResult> EditPagePost(string appCode, string page, DataDrivenAppPage newModel)
        {
            var app = await _appProvider.GetAppAsync(appCode);
            // Delete removed components
            newModel.Components = newModel.Components?.Where(component => !string.IsNullOrWhiteSpace(component.RenderType)).ToList();
            app.Pages[page] = newModel;
            await _appProvider.RefreshAppStoreAsync(appCode);
            return RedirectToAction("EditPage");
        }

        #endregion Edit functionality
    }
}