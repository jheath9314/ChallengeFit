using SWENG894.Models;
using SWENG894.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository.IRepository
{
    public interface IMessageRepository : IRepository<Message>
    {
        public void UpdateAsync(Message category);

        public IEnumerable<Message> GetAllUserMessages(string sort, string search, string box, string userId);

        public MessageViewModel CreateNewMesage(string fromUserId);
    }
}
