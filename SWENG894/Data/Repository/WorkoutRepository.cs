using Microsoft.EntityFrameworkCore;
using MimeKit.Cryptography;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using SWENG894.ViewModels;
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
            var workouts = _context.Workouts.Where(m => m.Published);


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

        public IEnumerable<Workout> GetUserWorkouts(string sort, string search, string userId, bool published)
        {
            var user = _context.ApplicationUsers
                .Include(x => x.Workouts)
                .Include(x => x.WorkoutFavorites)
                .ThenInclude(x => x.Workout)
                .FirstOrDefault(x => x.Id == userId);

            var wkList = user.Workouts.ToList();

            foreach(var wrkFave in user.WorkoutFavorites)
            {
                if(!user.Workouts.Contains(wrkFave.Workout))
                {
                    wkList.Add(wrkFave.Workout);
                }
            }

            if (!String.IsNullOrEmpty(search))
            {
                wkList = wkList.Where(m => m.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            wkList = sort switch
            {
                "desc" => wkList.OrderByDescending(m => m.Name).ToList(),
                _ => wkList.OrderBy(m => m.Name).ToList()
            };

            wkList = published switch
            {
                true => wkList.Where(w => w.Published).ToList(),
                _ => wkList.Where(w => !w.Published).ToList()
            };

            return wkList;
        }

        public IEnumerable<Workout> FindNewWorkouts(string sort, string search, string userId)
        {
            var user = _context.ApplicationUsers
                .Include(x => x.Workouts)
                .Include(x => x.WorkoutFavorites)
                .ThenInclude(x => x.Workout)
                .FirstOrDefault(x => x.Id == userId);

            var matchingWorkouts = _context.Workouts.Where(u => u.Published).ToList();

            if (!String.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                matchingWorkouts = matchingWorkouts.Where(u => u.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            matchingWorkouts = sort switch
            {
                "desc" => matchingWorkouts.OrderByDescending(s => s.Name).ToList(),
                _ => matchingWorkouts.OrderBy(s => s.Name).ToList(),
            };


            foreach(var fave in user.WorkoutFavorites)
            {
                if(matchingWorkouts.Contains(fave.Workout))
                {
                    matchingWorkouts.ElementAt(matchingWorkouts.IndexOf(fave.Workout)).IsFavorite = true;
                }               
            }

            return matchingWorkouts;
        }
    }
}
