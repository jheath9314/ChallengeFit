using Microsoft.EntityFrameworkCore;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using SWENG894.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository
{
    public class MessageRepository : MessageRepository<Message>, IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public MessageViewModel CreateNewMesage(string fromUserId)
        {
            var sender = _context.ApplicationUsers
                .Include(u => u.SentFriendRequests)
                .ThenInclude(u => u.RequestedFor)
                .Include(u => u.ReceievedFriendRequests)
                .ThenInclude(u => u.RequestedBy)
                .FirstOrDefault(u => u.Id == fromUserId);

            if (sender == null)
            {
                return null;
            }

            MessageViewModel message = new MessageViewModel()
            {
                SentById = fromUserId,//User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value,
                SentBy = sender,
                Friends = new List<ApplicationUser>()
            };

            foreach (var request in sender.Friends)
            {
                if (request.RequestedById == fromUserId)
                {
                    message.Friends.Add(request.RequestedFor);
                }
                else
                {
                    message.Friends.Add(request.RequestedBy);
                }
            }

            return message;
        }

        public IEnumerable<Message> GetAllUserMessages(string sort, string search, string box, string userId)
        {
            var messages = _context.Messages
                .Include(m => m.SentBy)
                .Include(m => m.SentTo)
                .Where(m => (m.SentById == userId && !m.DeletedBySender) || (m.SentToId == userId && !m.DeletedByReceiver));


            if (!String.IsNullOrEmpty(search))
            {
                messages = messages.Where(m => m.Subject.ToLower().Contains(search) ||
                    m.Body.ToLower().Contains(search));
            }

            if (!String.IsNullOrEmpty(box))
            {
                messages = messages.Where(m => m.SentById == userId && !m.DeletedBySender);
            }
            else
            {
                messages = messages.Where(m => m.SentToId == userId && !m.DeletedByReceiver);
            }

            messages = sort switch
            {
                "desc" => messages.OrderByDescending(m => m.SentTime),
                _ => messages.OrderBy(m => m.SentTime)
            };

            return messages;
        }

        public void UpdateAsync(Message category)
        {
            throw new NotImplementedException();
        }
    }
}
