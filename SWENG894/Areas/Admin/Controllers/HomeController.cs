using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using SWENG894.Utility;

namespace SWENG894.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _pageSize;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _pageSize = 10;
        }

        public async Task<IActionResult> Index(string sort, string search, string filter, string list, string currentList, int? page)
        {
            ViewData["CurrentSort"] = sort;
            ViewData["SortOrder"] = String.IsNullOrEmpty(sort) ? "desc" : "";
            if (list == null)
            {
                list = currentList;
            }
            ViewData["CurrentList"] = list;

            if (search == null)
            {
                search = filter;
            }
            else
            {
                search = search.ToLower();
            }

            ViewData["CurrentFilter"] = search;

            //var matchingUsers = _unitOfWork.FriendRequest.GetUserFriends(sort, search, User.FindFirstValue(ClaimTypes.NameIdentifier), !string.IsNullOrEmpty(list)).ToList();
           // var personList = await PaginatedList<ApplicationUser>.Create(matchingUsers, page ?? 1, _pageSize);

            return View();
        }

        public void GenerateTestData()
        {
            SWENG894.DataGenerationUtility.TestDataGenerator dataGen = new SWENG894.DataGenerationUtility.TestDataGenerator();
            dataGen.GenerateTestData(_unitOfWork);
            //return View();
        }

        public void RemoveTestData()
        {
            //This function causes a crash and hours of debugging have not determined why
            //SWENG894.DataGenerationUtility.TestDataGenerator dataGen = new SWENG894.DataGenerationUtility.TestDataGenerator();
            //dataGen.RemoveTestData(_unitOfWork);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
