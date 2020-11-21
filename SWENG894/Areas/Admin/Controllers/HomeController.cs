using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SWENG894.Data.Repository.IRepository;
using SWENG894.DataGenerationUtility;
using SWENG894.Models;
using SWENG894.Utility;
using SWENG894.Ranking;

namespace SWENG894.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _pageSize;
        private readonly ITestDataGenerator _testGenerator;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, ITestDataGenerator testGeneratot)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _pageSize = 10;
            _testGenerator = testGeneratot;
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

            var ranking = _unitOfWork.Ranking.GetRankings();
            if (ranking != null)
            {
                var timeString = "Rankings generated on " + ranking.Timestamp + " UTC";
                ViewData["Timestamp"] = timeString;
            }
            else
            {
                ViewData["Timestamp"] = "";
            }

            return View(ranking);
        }

        public async Task<IActionResult> GenerateTestData()
        {
            _testGenerator.GenerateTestData();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveTestData()
        {
            _testGenerator.RemoveTestData();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GenerateRanks()
        {
            var ratings = _unitOfWork.ApplicationUser.GetAllUserRatings();
            var rankSys = new RankingSystem();
            var rankCenters = rankSys.GenerateRankings(5, ratings, 1000);

            var ranking = _unitOfWork.Ranking.GetRankings();
            if (ranking != null)
            {
                ranking.BronzeValue = rankCenters[0];
                ranking.SilverValue = rankCenters[1];
                ranking.GoldValue = rankCenters[2];
                ranking.PlatinumValue = rankCenters[3];
                ranking.DiamondValue = rankCenters[4];
                ranking.Timestamp = DateTime.UtcNow.ToString();

                _unitOfWork.Ranking.UpdateAsync(ranking);
                await _unitOfWork.Save();
            }
            else
            {
                var newRanking = new Models.Ranking()
                {
                    BronzeValue = rankCenters[0],
                    SilverValue = rankCenters[1],
                    GoldValue = rankCenters[2],
                    PlatinumValue = rankCenters[3],
                    DiamondValue = rankCenters[4],
                    Timestamp = DateTime.UtcNow.ToString()
                };

                await _unitOfWork.Ranking.AddAsync(newRanking);
                await _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
