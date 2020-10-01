using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository.IRepository
{
    public interface IWorkoutResultRepository : IRepository<WorkoutResult>
    {
        public void UpdateAsync(WorkoutResult workoutResult);
    }
}
