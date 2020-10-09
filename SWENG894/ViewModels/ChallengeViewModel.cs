using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.ViewModels
{
    public class ChallengeViewModel
    {
        public string ChallengerId { get; set; }
        public ApplicationUser Challenger { get; set; }

        [Display(Name = "Send To")]
        public string ContenderId { get; set; }
        public ApplicationUser Contender { get; set; }

        [Display(Name = "Workout")]
        public int WorkoutId { get; set; }
        public List<WorkoutFavorite> WorkoutFavorites { get; set; }
        public IEnumerable<ApplicationUser> Friends { get; set; }
    }
}
