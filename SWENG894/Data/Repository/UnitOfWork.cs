using Microsoft.EntityFrameworkCore.Metadata;
using SWENG894.Data.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IMessageRepository Message { get; private set; }
        public IFriendRequestRepository FriendRequest { get; private set; }
        public IWorkoutRepository Workout { get; private set; }
        public IExerciseRepository Exercise { get; private set; }
        public IWorkoutResultRepository WorkoutResult { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            ApplicationUser = new ApplicationUserRepository(context);
            Message = new MessageRepository(context);
            FriendRequest = new FriendRequestRepository(context);
            Workout = new WorkoutRepository(context);
            Exercise = new ExerciseRepository(context);
            WorkoutResult = new WorkoutResultRepository(context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
