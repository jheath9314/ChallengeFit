using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository
{
    public class ExerciseRepository : Repository<Exercise>, IExerciseRepository
    {
        private readonly ApplicationDbContext _context;

        public ExerciseRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void UpdateAsync(Exercise exercise)
        {
            _context.Exercises.Update(exercise);
        }
    }
}
