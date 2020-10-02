using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWENG894.Data;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using SWENG894.Utility;
using SWENG894.ViewModels;
using static SWENG894.Models.FriendRequest;

namespace SWENG894.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "Admin, User")]
    public class FriendsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _pageSize;

        public FriendsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _pageSize = 5;
        }

        // GET: User/Friends
        public async Task<IActionResult> Index(string sort, string search, string filter, int? page)
        {
            ViewData["CurrentSort"] = sort;
            ViewData["SortOrder"] = String.IsNullOrEmpty(sort) ? "desc" : "";

            if (search == null)
            {
                search = filter;
            }
            else
            {
                search = search.ToLower();
            }

            ViewData["CurrentFilter"] = search;
            var personList = await PaginatedList<ApplicationUser>.Create(_unitOfWork.FriendRequest.GetUserFriends(sort, search, User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList(), page ?? 1, _pageSize);

            return View(personList);
        }

        // GET: User/Friends/Find
        public async Task<IActionResult> Find(string sort, string search, string filter, int? page)
        {
            ViewData["CurrentSort"] = sort;
            ViewData["SortOrder"] = String.IsNullOrEmpty(sort) ? "desc" : "";

            if (search == null)
            {
                search = filter;
            }
            else
            {
                search = search.ToLower();
            }

            ViewData["CurrentFilter"] = search;

            var resultList = await PaginatedList<ApplicationUser>.Create(_unitOfWork.FriendRequest.FindNewFriends(sort, search, User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList(), page ?? 1, _pageSize);

            return View(resultList);
        }

        // GET: User/Friends/SendRequest/5
        public async Task<IActionResult> SendRequest(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personToFriend = await _unitOfWork.FriendRequest.GetPersonToFriend(id);

            if (personToFriend == null)
            {
                return NotFound();
            }

            return View(personToFriend);
        }

        // POST: User/Friends/SendRequest
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("SendRequest")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendRequestPost(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userToFriend = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Id == id);

            if (userToFriend == null || User.FindFirstValue(ClaimTypes.NameIdentifier) == null)
            {
                return NotFound();
            }

            var friendRequest = await _unitOfWork.FriendRequest
                .GetFirstOrDefaultAsync(m => m.RequestedById == User.FindFirstValue(ClaimTypes.NameIdentifier) && m.RequestedForId == id);

            if (friendRequest == null)
            {
                FriendRequest request = new FriendRequest
                {
                    RequestedById = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    RequestedForId = userToFriend.Id,
                    RequestTime = DateTime.Now,
                    Status = FriendRequestStatus.New
                };
                await _unitOfWork.FriendRequest.AddAsync(request);


                Message msg = new Message()
                {
                    SentById = request.RequestedById,
                    SentToId = request.RequestedForId,
                    Subject = "New friend request.",
                    Body = "Click here to view the request.",
                    SentTime = DateTime.Now,
                    MessageType = Message.MessageTypes.FriendRequest,
                    SendStatus = Message.MessageSendStatud.New,
                    ReadStatus = Message.MessageReadStatud.New,
                    DeletedByReceiver = false,
                    DeletedBySender = false
                };
                await _unitOfWork.Message.AddAsync(msg);

                await _unitOfWork.Save();
                return View(userToFriend);
            }

            return View(userToFriend);
            //return RedirectToAction(nameof(Index));
        }

        // GET: User/Friends/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friendRequest = await _unitOfWork.FriendRequest
                .GetFirstOrDefaultAsync(m => m.RequestedById == id,
                includeProperties: "RequestedBy,RequestedFor");

            if (friendRequest == null)
            {
                return NotFound();
            }

            return View(friendRequest);
        }

        // GET: User/Friends/ViewRequest/5
        public async Task<IActionResult> ViewRequest(string sender, string receiver)
        {
            if (sender == null || receiver == null)
            {
                return NotFound();
            }

            var friendRequest = await _unitOfWork.FriendRequest
                .GetFirstOrDefaultAsync(m => m.RequestedById == sender && m.RequestedForId == receiver,
                includeProperties: "RequestedBy,RequestedFor");

            if (friendRequest == null)
            {
                return NotFound();
            }

            if (friendRequest.RequestedForId != User.FindFirstValue(ClaimTypes.NameIdentifier) && friendRequest.RequestedById != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            return View(friendRequest);
        }

        // POST: User/Friends/ViewRequest
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("ViewRequest")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewRequestPost(string sender, string receiver, FriendRequestStatus status)
        {
            var friendRequest = await _unitOfWork.FriendRequest
                .GetFirstOrDefaultAsync(m => m.RequestedById == sender && m.RequestedForId == receiver,
                includeProperties: "RequestedBy,RequestedFor");

            if (friendRequest == null || friendRequest.RequestedForId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            friendRequest.Status = status;

            if(status == FriendRequestStatus.Approved)
            {
                friendRequest.BecameFriendsTime = DateTime.Now;
            }

            await _unitOfWork.Save();
            return View(friendRequest);
        }

        // GET: User/Friends/Profile/5
        public async Task<IActionResult> Profile(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _unitOfWork.ApplicationUser
                .GetFirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            UserProfileViewModel userView = new UserProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ZipCode = user.ZipCode
            };

            return View(userView);
        }
    }
}
