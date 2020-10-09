using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using SWENG894.Utility;

namespace SWENG894.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "Admin, User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _pageSize;

        [ExcludeFromCodeCoverage]
        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _pageSize = 100;
        }

        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> Index()
        {
            var feed = await PaginatedList<NewsFeed>.Create(_unitOfWork.NewsFeed.GetUserFeed(User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList(), 1, _pageSize);
            return View(feed);
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
