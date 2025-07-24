using Microsoft.AspNetCore.Mvc;

namespace SharpCms.Core.Controllers
{
    public class SharpcmsController : Controller
    {
        //
        // GET: /Shell/

        public IActionResult Index()
        {
            return View();
        }
    }
}
