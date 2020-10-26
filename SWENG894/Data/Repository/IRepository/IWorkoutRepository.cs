using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository.IRepository
{
    public interface IWorkoutRepository : IRepository<Workout>
    {
        public void UpdateAsync(Workout workout);

        public IEnumerable<Workout> GetAllWorkouts(string sort, string search);

        public IEnumerable<Workout> GetUserWorkouts(string sort, string search, string userId, bool published);

        public IEnumerable<Workout> FindNewWorkouts(string sort, string search, string userId);

    }
}
