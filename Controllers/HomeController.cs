using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using aspnet_mongodb_session_sample.Models;
using Microsoft.AspNetCore.Http;

namespace aspnet_mongodb_session_sample.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            var auxInt = 1;
            var sessioNKey = "TotalLoadsInthisSession";
            var session = this.HttpContext.Session;
            var sessionLoads = session.GetInt32(sessioNKey);
            if(sessionLoads.HasValue)
            {
            auxInt = sessionLoads.Value + 1;
            }
            session.SetInt32(sessioNKey, auxInt);
            
            ViewData["TotalViewsMessage"] = auxInt;

            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
