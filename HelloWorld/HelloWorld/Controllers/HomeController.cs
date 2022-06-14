using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HelloWorld.Models;
using HelloWorld.Options;

namespace HelloWorld.Controllers
{
    public class HomeController : Controller
    {
        private ConfigurationOptions options;

        public HomeController(ConfigurationOptions options)
        {
            this.options = options;
        }

        public IActionResult Index()
        {
            ViewData["Message"] = options.Message;
            return View();
        }

        public IActionResult About()
        {
            ViewData["DeploymentEnvironment"] = options.DeploymentEnvironment;

            return View();
        }

        public IActionResult Contact()
        {
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
