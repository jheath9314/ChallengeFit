using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IApplicationUserRepository ApplicationUser { get; }        
        IMessageRepository Message { get; }
        IFriendRequestRepository FriendRequest { get; }
        IWorkoutRepository Workout { get; }
        IExerciseRepository Exercise { get; }
        IWorkoutResultRepository WorkoutResult { get; }

        Task Save();
    }
}
