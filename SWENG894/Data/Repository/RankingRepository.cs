using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;

namespace SWENG894.Data.Repository
{
    public class RankingRepository : Repository<Models.Ranking>, IRankingRepository
    {
        private readonly ApplicationDbContext _context;

        public RankingRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void UpdateAsync(Models.Ranking ranking)
        {
            _context.Ranking.Update(ranking);
        }

        public Models.Ranking GetRankings()
        {
            return _context.Ranking.FirstOrDefault(); 
        }
    }
}
