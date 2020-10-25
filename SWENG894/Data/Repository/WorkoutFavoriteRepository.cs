using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository
{
    public class WorkoutFavoriteRepository : Repository<WorkoutFavorite>, IWorkoutFavoriteRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkoutFavoriteRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public override bool ObjectExists(int id)
        {
            throw new NotImplementedException();
        }

        public override Task<WorkoutFavorite> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public override Task RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
