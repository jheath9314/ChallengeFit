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
        void Update(Message category);

        IEnumerable<Message> GetAllUserMessages(string sort, string search, string box, string userId);

        MessageViewModel CreateNewMesage(string fromUserId);
    }
}
