using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.ViewModels
{
    public class ShareViewModel
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [Required]
        public string ShareWithUserId { get; set; }
        public ApplicationUser ShareWithUser { get; set; }
        [Required]
        public int WorkoutId { get; set; }
        public Workout Workout { get; set; }
        public List<WorkoutFavorite> WorkoutFavorites { get; set; }
        public IEnumerable<ApplicationUser> Friends { get; set; }
    }
}
