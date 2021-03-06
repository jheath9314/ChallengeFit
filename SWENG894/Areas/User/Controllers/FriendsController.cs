using System;
using System.Diagnostics.CodeAnalysis;
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
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> Index(string sort, string search, string filter, string list, string currentList, int? page)
        {
            ViewData["CurrentSort"] = sort;
            ViewData["SortOrder"] = String.IsNullOrEmpty(sort) ? "desc" : "";

            if(list == null)
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

            var matchingUsers = _unitOfWork.FriendRequest.GetUserFriends(sort, search, User.FindFirstValue(ClaimTypes.NameIdentifier), !string.IsNullOrEmpty(list)).ToList();
            var personList = await PaginatedList<ApplicationUser>.Create(matchingUsers, page ?? 1, _pageSize);

            return View(personList);
        }

        // GET: User/Friends/Find
        [ExcludeFromCodeCoverage]
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
        [ExcludeFromCodeCoverage]
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
                    RequestStatus = FriendRequestStatus.New,
                    ReceiverStatus = FriendRequestStatus.New,
                    RequesterStatus = FriendRequestStatus.Approved
                };
                await _unitOfWork.FriendRequest.AddAsync(request);


                Message msg = new Message()
                {
                    SentById = request.RequestedById,
                    SentToId = request.RequestedForId,
                    Subject = "New friend request.",
                    Body = "Someone wants to be friends with you.",
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
                .GetFirstOrDefaultAsync(m => m.RequestedById == sender && m.RequestedForId == receiver || m.RequestedById == receiver && m.RequestedForId == sender,
                includeProperties: "RequestedBy,RequestedFor");

            if (friendRequest == null || (friendRequest.RequestedForId != User.FindFirstValue(ClaimTypes.NameIdentifier) && friendRequest.RequestedById != User.FindFirstValue(ClaimTypes.NameIdentifier)))
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
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> ViewRequestPost(string sender, string receiver, FriendRequestStatus status)
        {
            var friendRequest = await _unitOfWork.FriendRequest
                .GetFirstOrDefaultAsync(m => m.RequestedById == sender && m.RequestedForId == receiver || m.RequestedById == receiver && m.RequestedForId == sender,
                includeProperties: "RequestedBy,RequestedFor");

            if (friendRequest == null || (friendRequest.RequestedForId != User.FindFirstValue(ClaimTypes.NameIdentifier) && friendRequest.RequestedById != User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                return NotFound();
            }

            if(friendRequest.RequestedForId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {          
                switch(friendRequest.RequestStatus)
                {
                    case FriendRequestStatus.New:
                        
                        if(status == FriendRequestStatus.Approved)
                        {
                            friendRequest.RequestStatus = FriendRequestStatus.Approved;
                            friendRequest.ReceiverStatus = FriendRequestStatus.Approved;
                            friendRequest.BecameFriendsTime = DateTime.Now;
                        }
                        else if (status == FriendRequestStatus.Rejected)
                        {
                            friendRequest.RequestStatus = FriendRequestStatus.Rejected;
                            friendRequest.ReceiverStatus = FriendRequestStatus.Rejected;
                        }
                        break;
                    case FriendRequestStatus.Approved:

                        if (status == FriendRequestStatus.Rejected)
                        {
                            friendRequest.RequestStatus = FriendRequestStatus.Rejected;
                            friendRequest.ReceiverStatus = FriendRequestStatus.Rejected;
                        }
                        else if(status == FriendRequestStatus.Approved)
                        {
                            friendRequest.RequestStatus = SetFriendRequestStatus(friendRequest.RequesterStatus, FriendRequestStatus.Approved);
                            friendRequest.ReceiverStatus = FriendRequestStatus.Approved;
                        }
                        break;
                    case FriendRequestStatus.Rejected:

                        if (status == FriendRequestStatus.Approved)
                        {
                            friendRequest.RequestStatus = SetFriendRequestStatus(friendRequest.RequesterStatus, FriendRequestStatus.Approved);
                            friendRequest.ReceiverStatus = FriendRequestStatus.Approved;
                        }
                        else if(status == FriendRequestStatus.Rejected)
                        {
                            friendRequest.RequestStatus = FriendRequestStatus.Rejected;
                            friendRequest.ReceiverStatus = FriendRequestStatus.Rejected;
                        }
                        break;
                }
            }
            else
            {
                switch (friendRequest.RequestStatus)
                {
                    case FriendRequestStatus.Approved:
                        if (status == FriendRequestStatus.Rejected)
                        {
                            friendRequest.RequestStatus = FriendRequestStatus.Rejected;
                            friendRequest.RequesterStatus = FriendRequestStatus.Rejected;
                        }
                        else if(status == FriendRequestStatus.Approved)
                        {
                            friendRequest.RequestStatus = SetFriendRequestStatus(FriendRequestStatus.Approved, friendRequest.ReceiverStatus);
                            friendRequest.RequesterStatus = FriendRequestStatus.Approved;
                        }
                        break;
                    case FriendRequestStatus.Rejected:

                        if (status == FriendRequestStatus.Approved)
                        {
                            friendRequest.RequestStatus = SetFriendRequestStatus(FriendRequestStatus.Approved, friendRequest.ReceiverStatus);
                            friendRequest.RequesterStatus = FriendRequestStatus.Approved;
                        }
                        if (status == FriendRequestStatus.Rejected)
                        {
                            friendRequest.RequestStatus = FriendRequestStatus.Rejected;
                            friendRequest.RequesterStatus = FriendRequestStatus.Rejected;
                        }
                        break;
                }
            }

            _unitOfWork.FriendRequest.UpdateAsync(friendRequest);
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
                ZipCode = user.ZipCode,
                Rating = user.Rating

            };

            var RankingSystem = new Ranking.RankingSystem();
            var Clusters = await _unitOfWork.Ranking.GetFirstOrDefaultAsync();
            var ClusterIntList = new System.Collections.Generic.List<int>();

            ClusterIntList.Add(Clusters.BronzeValue);
            ClusterIntList.Add(Clusters.SilverValue);
            ClusterIntList.Add(Clusters.GoldValue);
            ClusterIntList.Add(Clusters.PlatinumValue);
            ClusterIntList.Add(Clusters.DiamondValue);
            ClusterIntList.Sort();

            var ranking = RankingSystem.GetClosestCluster((int)user.Rating, ClusterIntList);
            ViewBag.Ranking = ranking;

            return View(userView);
        }

        // GET: User/Friends/Delete/5
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> Delete(string sender, string receiver)
        {
            if (sender == null || receiver == null)
            {
                return NotFound();
            }

            var friendRequest = await _unitOfWork.FriendRequest.GetFirstOrDefaultAsync(x => (x.RequestedById == sender && x.RequestedForId == receiver) || (x.RequestedById == receiver && x.RequestedForId == sender), includeProperties: "RequestedBy,RequestedFor");
            if (friendRequest == null)
            {
                return NotFound();
            }

            if(friendRequest.RequestedById != User.FindFirstValue(ClaimTypes.NameIdentifier) && friendRequest.RequestedForId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            return View(friendRequest);
        }

        [ExcludeFromCodeCoverage]
        public async Task<IActionResult>Rankings()
        {
            return View();
        }

        // POST: User/Friends/Delete/5
        //Vladimir, need help with this one
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string RequestedById, string RequestedForId)
        {
            var friendRequest = await _unitOfWork.FriendRequest.GetFirstOrDefaultAsync(x => (x.RequestedById == RequestedById && x.RequestedForId == RequestedForId) || (x.RequestedById == RequestedForId && x.RequestedForId == RequestedById));
            if (friendRequest == null)
            {
                return NotFound();
            }

            if (friendRequest.RequestedById != User.FindFirstValue(ClaimTypes.NameIdentifier) && friendRequest.RequestedForId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }
            await _unitOfWork.FriendRequest.RemoveAsync(friendRequest);
            await _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        [ExcludeFromCodeCoverage]
        private FriendRequestStatus SetFriendRequestStatus(FriendRequestStatus senderStatus, FriendRequestStatus receiverStatus)
        {
            if(senderStatus == FriendRequestStatus.Rejected || receiverStatus == FriendRequestStatus.Rejected)
            {
                return FriendRequestStatus.Rejected;
            }

            return FriendRequestStatus.Approved;
        }
    }
}
