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
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Messages.Include(m => m.SentBy).Include(m => m.SentTo);
            return View(await applicationDbContext.ToListAsync());
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
            if(id == null)
            {
                return NotFound();
            }

            var receiver = _context.ApplicationUsers.FirstOrDefault(u => u.Id.Equals(id));

            if(receiver == null)
            {
                return NotFound();
            }

            MessageViewModel message = new MessageViewModel()
            {
                SentById = User.FindFirstValue(ClaimTypes.NameIdentifier),//User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value,
                SentTo = receiver,
                SentToId = receiver.Id,
                FullName = receiver.FullName
            };

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
                .FirstOrDefaultAsync(m => m.Id == id && m.SentToId == User.FindFirstValue(ClaimTypes.NameIdentifier));
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
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id && m.SentToId == User.FindFirstValue(ClaimTypes.NameIdentifier));

            if(message != null)
            {
                _context.Messages.Remove(message);
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
