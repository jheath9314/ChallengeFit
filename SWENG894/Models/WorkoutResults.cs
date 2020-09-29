using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWENG894.Models
{
    public class WorkoutResults
    {
        [Key]
        public int Id { get; set; }

        public Workout workout { get; set; }
        public int WorkoutId { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public int Score { get; set; }

        [NotMapped]
        public String username { get; set; }

        [NotMapped]
        public String workoutName { get; set; }
    }
}
