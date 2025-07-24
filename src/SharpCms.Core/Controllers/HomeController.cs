using Microsoft.AspNetCore.Mvc;

namespace SharpCms.Core.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public IActionResult Index()
        {
            return RedirectToRoute(new {Controller = "Sharpcms", Action = "Index"});
        }

    }
}
