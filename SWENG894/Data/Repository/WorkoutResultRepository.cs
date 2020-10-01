using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository
{
    public class WorkoutResultRepository : Repository<WorkoutResult>, IWorkoutResultRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkoutResultRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void UpdateAsync(WorkoutResult workoutResult)
        {
            _context.WorkoutResults.Update(workoutResult);
        }
    }
}
