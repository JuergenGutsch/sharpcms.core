﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SharpCms.Core.Controllers
{
    public class SitesController : Controller
    {
        //
        // GET: /Sites/

        public IActionResult Index()
        {
            var items = new List<Site>
                {
                    new Site {Name = "Main Page", State = "running", Url = "http://www.gutsch-online.de"},
                    new Site {Name = "Lizzy", State = "running", Url = "http://www.lizzy.net"},
                    new Site {Name = "CityGuide", State = "stopped", Url = "http://www.cityguide.net"}
                };
            return View(items);
        }

    }

    public class Site
    {
        public string Name { get; set; }

        public string State { get; set; }

        public string Url { get; set; }
    }
}
