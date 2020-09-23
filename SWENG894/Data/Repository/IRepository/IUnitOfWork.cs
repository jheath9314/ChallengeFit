using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IMessageRepository Message { get; }

        Task Save();
    }
}
