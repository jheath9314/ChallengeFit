using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using SWENG894.Utility;

namespace SWENG894.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "Admin, User")]
    public class LeaderboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _pageSize;

        public LeaderboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _pageSize = 5;
        }

        // GET: User/Leaderboard
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> IndexAsync(string sort, string search, string filter, string list, string currentList, int? page)
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
            var matchingUsers = _unitOfWork.ApplicationUser.GetLeaderboard(!string.IsNullOrEmpty(list) ? "" : User.FindFirstValue(ClaimTypes.NameIdentifier));
            var personList = await PaginatedList<ApplicationUser>.Create(matchingUsers.ToList(), page ?? 1, _pageSize);

            return View(personList);
        }
    }
}
