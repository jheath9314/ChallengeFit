using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository.IRepository
{
    public interface IExerciseRepository : IRepository<Exercise>
    {
        public void UpdateAsync(Exercise exercise);
    }
}
