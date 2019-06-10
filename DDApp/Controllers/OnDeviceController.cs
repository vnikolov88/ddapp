using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DDApp.Controllers
{
    [Route("/ondevice", Order = 1)]
    public class OnDeviceController : Controller
    {
        [Route("{*args}", Order = -1)]
        public async Task<IActionResult> OnDevice(string args)
        {
            switch(args)
            {
                case string arg when arg.StartsWith("location"):
                    await Task.Delay(3000);
                    return Content("51.2277411,6.7734556");
                default:
                    return NotFound();
            }
        }
    }
}