using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SWENG894.Models;

namespace SWENG894.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "Admin, User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        [ExcludeFromCodeCoverage]
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [ExcludeFromCodeCoverage]
        public IActionResult Index()
        {
            return View();
        }

        [ExcludeFromCodeCoverage]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [ExcludeFromCodeCoverage]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
