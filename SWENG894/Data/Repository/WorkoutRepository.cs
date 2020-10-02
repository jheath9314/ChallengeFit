using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository
{
    public class WorkoutRepository : Repository<Workout>, IWorkoutRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkoutRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void UpdateAsync(Workout workout)
        {
            _context.Workouts.Update(workout);
        }

        public IEnumerable<Workout> GetAllWorkouts(string sort, string search)
        {
            var workouts = _context.Workouts.Where(m => m.Name != null);


            if (!String.IsNullOrEmpty(search))
            {
                workouts = workouts.Where(m => m.Name.ToLower().Contains(search.ToLower()));
            }

            workouts = sort switch
            {
                "desc" => workouts.OrderByDescending(m => m.Name),
                _ => workouts.OrderBy(m => m.Name)
            };

            return workouts;
        }
    }
}
