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
using SWENG894.ViewModels;

namespace SWENG894.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "Admin, User")]
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User/Messages
        public async Task<IActionResult> Index(string sort, string search, string filter, int? page, string box)
        {
            ViewData["CurrentSort"] = sort;
            ViewData["SortOrder"] = String.IsNullOrEmpty(sort) ? "desc" : "";
            ViewData["Box"] = String.IsNullOrEmpty(box) ? "inbox" : "sent";

            if (search == null)
            {
                search = filter;
            }
            else
            {
                search = search.ToLower();
            }

            ViewData["CurrentFilter"] = search;

            var messages = _context.Messages
                .Include(m => m.SentBy)
                .Include(m => m.SentTo)
                .Where(m => (m.SentById == User.FindFirstValue(ClaimTypes.NameIdentifier) && !m.DeletedBySender) ||
                            (m.SentToId == User.FindFirstValue(ClaimTypes.NameIdentifier) && !m.DeletedByReceiver));

            if (!String.IsNullOrEmpty(search))
            {
                messages = messages.Where(m => m.Subject.ToLower().Contains(search) ||
                    m.Body.ToLower().Contains(search));
            }

            if (!String.IsNullOrEmpty(box))
            {
                messages = messages.Where(m => m.SentById == User.FindFirstValue(ClaimTypes.NameIdentifier) && !m.DeletedBySender);
            }
            else
            {
                messages = messages.Where(m => m.SentToId == User.FindFirstValue(ClaimTypes.NameIdentifier) && !m.DeletedByReceiver);
            }

            messages = sort switch
            {
                "desc" => messages.OrderByDescending(m => m.SentTime),
                _ => messages.OrderBy(m => m.SentTime)
            };

            int pageSize = 3;
            var personList = await PaginatedList<Message>.CreateAsync(messages, page ?? 1, pageSize);

            return View(personList);
        }

        // GET: User/Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.SentBy)
                .Include(m => m.SentTo)
                .FirstOrDefaultAsync(m => m.Id == id && (m.SentById == User.FindFirstValue(ClaimTypes.NameIdentifier) || m.SentToId == User.FindFirstValue(ClaimTypes.NameIdentifier)));

            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: User/Messages/Create
        public IActionResult Create(string id)
        {

            var sender = _context.ApplicationUsers
                .Include(u => u.SentFriendRequests)
                .ThenInclude(u => u.RequestedFor)
                .Include(u => u.ReceievedFriendRequests)
                .ThenInclude(u => u.RequestedBy)
                .FirstOrDefault(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (sender == null)
            {
                return NotFound();
            }

            MessageViewModel message = new MessageViewModel()
            {
                SentById = User.FindFirstValue(ClaimTypes.NameIdentifier),//User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value,
                SentBy = sender,
                Friends = new List<ApplicationUser>()
            };

            foreach(var request in sender.Friends)
            {
                if(request.RequestedById == User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    message.Friends.Add(request.RequestedFor);
                }
                else
                {
                    message.Friends.Add(request.RequestedBy);
                }
            }

            //if (id != null)
            //{
            //    var receiver = _context.ApplicationUsers.FirstOrDefault(u => u.Id.Equals(id));

            //    if (receiver == null)
            //    {
            //        return NotFound();
            //    }

            //    message.SentToId = id;
            //    message.SentTo = receiver;
            //}

            return View(message);
        }

        // POST: User/Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MessageViewModel message)
        {
            if (ModelState.IsValid)
            {
                Message msg = new Message()
                {
                    SentById = message.SentById,
                    SentToId = message.SentToId,
                    Subject = message.Subject,
                    Body = message.Body,
                    SentTime = DateTime.Now,
                    MessageType = Message.MessageTypes.Correspondence,
                    SendStatus = Message.MessageSendStatud.New,
                    ReadStatus = Message.MessageReadStatud.New,
                    DeletedByReceiver = false,
                    DeletedBySender = false
                };

                _context.Add(msg);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(message);
        }

        // GET: User/Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.SentBy)
                .Include(m => m.SentTo)
                .FirstOrDefaultAsync(m => m.Id == id && ((m.SentToId == User.FindFirstValue(ClaimTypes.NameIdentifier) && !m.DeletedByReceiver) ||
                                                         (m.SentById == User.FindFirstValue(ClaimTypes.NameIdentifier) && !m.DeletedBySender)));
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: User/Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == id && ((m.SentToId == User.FindFirstValue(ClaimTypes.NameIdentifier) && !m.DeletedByReceiver) ||
                                                         (m.SentById == User.FindFirstValue(ClaimTypes.NameIdentifier) && !m.DeletedBySender)));

            if (message != null)
            {
                if (message.SentById == User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    message.DeletedBySender = true;
                }
                else if (message.SentToId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    message.DeletedByReceiver = true;
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
