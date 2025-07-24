using Microsoft.AspNetCore.Mvc;

namespace SharpCms.Core.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public IActionResult Index()
        {
            return View();
        }

    }
}
