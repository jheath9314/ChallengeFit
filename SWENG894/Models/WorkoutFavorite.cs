using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Models
{
    public class WorkoutFavorite
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int WorkoutId { get; set; }
        public Workout Workout { get; set; }
    }
}
