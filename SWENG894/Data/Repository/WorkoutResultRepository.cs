using Microsoft.EntityFrameworkCore;
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

        public List<WorkoutResult> GetWorkoutResults(string userId, int workoutId)
        {
            var workoutResults = _context.WorkoutResults.Where(w => w.UserId == userId && w.WorkoutId == workoutId);
            var workout = _context.Workouts.FirstOrDefault(w => w.Id == workoutId);
            var workoutResultsList = workoutResults.ToList();

            //For efficiency, only load the first name
            //they should all be the same
            if(workoutResultsList.Count > 0)
            {
                if(workout.Name == null)
                {
                    workoutResultsList[0].WorkoutName = "Error retrieving name";
                }
                else
                {
                    workoutResultsList[0].WorkoutName = workout.Name;
                }
            }
            return workoutResultsList;
        }
    }
}
