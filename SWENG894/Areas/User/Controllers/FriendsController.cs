using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SWENG894.Data;
using SWENG894.Models;
using SWENG894.Utility;
using static SWENG894.Models.FriendRequest;

namespace SWENG894.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "Admin, User")]
    public class FriendsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FriendsController(ApplicationDbContext context)
        {
            _context = context;
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

            var user = await _context.ApplicationUsers
                .Include(u => u.SentFriendRequests)
                .ThenInclude(u => u.RequestedFor)
                .Include(u => u.ReceievedFriendRequests)
                .ThenInclude(u => u.RequestedFor)
                .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            var matchingUsers = user.Friends;

            if (!String.IsNullOrEmpty(search))
            {
                matchingUsers = user.Friends.Where(u => u.RequestedFor.LastName.ToLower().Contains(search) ||
                    u.RequestedFor.FirstName.ToLower().Contains(search) ||
                    u.RequestedFor.Email.ToLower().Contains(search)).ToList();
            }

            matchingUsers = sort switch
            {
                "desc" => matchingUsers.OrderByDescending(s => s.RequestedFor.LastName).ToList(),
                _ => matchingUsers.OrderBy(s => s.RequestedFor.LastName).ToList(),
            };

            int pageSize = 3;
            var personList = await PaginatedList<FriendRequest>.Create(matchingUsers.ToList(), page ?? 1, pageSize);

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

            var user = await _context.ApplicationUsers
                    .Include(u => u.SentFriendRequests)
                    .ThenInclude(u => u.RequestedFor)
                    .Include(u => u.ReceievedFriendRequests)
                    .ThenInclude(u => u.RequestedFor)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            var matchingUsers = _context.ApplicationUsers.Where(u => u.EmailConfirmed == true).ToList();
            matchingUsers.Remove(user);

            if (!String.IsNullOrEmpty(search))
            {
                matchingUsers = matchingUsers.Where(u => u.LastName.ToLower().Contains(search) ||
                    u.FirstName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search)).ToList();
            }

            matchingUsers = sort switch
            {
                "desc" => matchingUsers.OrderByDescending(s => s.LastName).ToList(),
                _ => matchingUsers.OrderBy(s => s.LastName).ToList(),
            };

            foreach (var request in user.SentFriendRequests)
            {
                matchingUsers.Remove(request.RequestedFor);
            }

            int pageSize = 3;
            var resultList = await PaginatedList<ApplicationUser>.Create(matchingUsers.ToList(), page ?? 1, pageSize);

            return View(resultList);
        }

        // GET: User/Friends/SendRequest/5
        public async Task<IActionResult> SendRequest(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personToFriend = await _context.ApplicationUsers
                .Include(u => u.ReceievedFriendRequests)
                .ThenInclude(u => u.RequestedBy)
                .FirstOrDefaultAsync(m => m.Id == id && m.EmailConfirmed == true);

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

            var userToFriend = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == id);

            if (userToFriend == null || User.FindFirstValue(ClaimTypes.NameIdentifier) == null)
            {
                return NotFound();
            }

            var friendRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(m => m.RequestedById == User.FindFirstValue(ClaimTypes.NameIdentifier) && m.RequestedForId == id);

            if (friendRequest == null)
            {
                FriendRequest request = new FriendRequest
                {
                    RequestedById = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    RequestedForId = userToFriend.Id,
                    RequestTime = DateTime.Now,
                    Status = FriendRequestStatus.New
                };
                _context.Add(request);
                

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
                _context.Messages.Add(msg);

                await _context.SaveChangesAsync();
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

            var friendRequest = await _context.FriendRequests
                .Include(f => f.RequestedBy)
                .Include(f => f.RequestedFor)
                .FirstOrDefaultAsync(m => m.RequestedById == id);
            if (friendRequest == null)
            {
                return NotFound();
            }

            return View(friendRequest);
        }







        //// GET: User/Friends/Create
        //public IActionResult Create()
        //{
        //    ViewData["RequestedById"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
        //    ViewData["RequestedForId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
        //    return View();
        //}

        //// POST: User/Friends/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("RequestedById,RequestedForId,RequestTime,BecameFriendsTime,FriendRequestFlag")] FriendRequest friendRequest)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(friendRequest);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["RequestedById"] = new SelectList(_context.ApplicationUsers, "Id", "Id", friendRequest.RequestedById);
        //    ViewData["RequestedForId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", friendRequest.RequestedForId);
        //    return View(friendRequest);
        //}

        //// GET: User/Friends/Edit/5
        //public async Task<IActionResult> Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var friendRequest = await _context.FriendRequests.FindAsync(id);
        //    if (friendRequest == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["RequestedById"] = new SelectList(_context.ApplicationUsers, "Id", "Id", friendRequest.RequestedById);
        //    ViewData["RequestedForId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", friendRequest.RequestedForId);
        //    return View(friendRequest);
        //}

        //// POST: User/Friends/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(string id, [Bind("RequestedById,RequestedForId,RequestTime,BecameFriendsTime,FriendRequestFlag")] FriendRequest friendRequest)
        //{
        //    if (id != friendRequest.RequestedById)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(friendRequest);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!FriendRequestExists(friendRequest.RequestedById))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["RequestedById"] = new SelectList(_context.ApplicationUsers, "Id", "Id", friendRequest.RequestedById);
        //    ViewData["RequestedForId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", friendRequest.RequestedForId);
        //    return View(friendRequest);
        //}

        //// GET: User/Friends/Delete/5
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var friendRequest = await _context.FriendRequests
        //        .Include(f => f.RequestedBy)
        //        .Include(f => f.RequestedFor)
        //        .FirstOrDefaultAsync(m => m.RequestedById == id);
        //    if (friendRequest == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(friendRequest);
        //}

        //// POST: User/Friends/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    var friendRequest = await _context.FriendRequests.FindAsync(id);
        //    _context.FriendRequests.Remove(friendRequest);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool FriendRequestExists(string id)
        //{
        //    return _context.FriendRequests.Any(e => e.RequestedById == id);
        //}
    }
}
