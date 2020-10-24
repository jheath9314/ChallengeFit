using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SWENG894.Data;
using SWENG894.Data.Repository;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using SWENG894.Utility;
using SWENG894.ViewModels;

namespace SWENG894.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "Admin, User")]
    public class MessagesController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _pageSize;

        public MessagesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _pageSize = 5;
        }

        // GET: User/Messages
        [ExcludeFromCodeCoverage]
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

            var personList = await PaginatedList<Message>.Create(_unitOfWork.Message.GetAllUserMessages(sort, search, box, User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList(), page ?? 1, _pageSize);

            return View(personList);
        }

        // GET: User/Messages/Details/5
        public async Task<IActionResult> Details(int? id, string box)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["Box"] = box;
            var message = await _unitOfWork.Message.GetFirstOrDefaultAsync
                (m => m.Id == id && (m.SentById == User.FindFirstValue(ClaimTypes.NameIdentifier) || 
                 m.SentToId == User.FindFirstValue(ClaimTypes.NameIdentifier)),
                 includeProperties: "SentBy,SentTo");

            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: User/Messages/Create
        [ExcludeFromCodeCoverage]
        public IActionResult Create(string id, string origin)
        {

            var message = _unitOfWork.Message.CreateNewMesage(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (message == null)
            {
                return NotFound();
            }
            else if (id != null)
            {
                message.SentToId = id;
            }

            ViewData["Origin"] = origin;


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

                await _unitOfWork.Message.AddAsync(msg);
                await _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(message);
        }

        // GET: User/Messages/Delete/5
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _unitOfWork.Message.GetFirstOrDefaultAsync
                (m => m.Id == id && ((m.SentToId == User.FindFirstValue(ClaimTypes.NameIdentifier) && !m.DeletedByReceiver) ||
                (m.SentById == User.FindFirstValue(ClaimTypes.NameIdentifier) && !m.DeletedBySender)),
                includeProperties: "SentBy,SentTo");

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
            var message = await _unitOfWork.Message.GetFirstOrDefaultAsync
                (m => m.Id == id && ((m.SentToId == User.FindFirstValue(ClaimTypes.NameIdentifier) && !m.DeletedByReceiver) ||
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
                await _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        //private bool MessageExists(int id)
        //{
        //    return _unitOfWork.Message.ObjectExists(id);
        //}
    }
}
