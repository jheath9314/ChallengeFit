using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Models
{
    public class Workout
    {
        public enum Scoring
        {
            Time,
            Reps
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "You must provide a name")]
        public String Name { get; set; }

        public int Time { get; set; }

        [DataType(DataType.MultilineText)]
        public String Notes { get; set; }

        public Scoring ScoringType {get; set; }

        [DataType(DataType.MultilineText)]
        public String ScalingOptions { get; set; }

        public List<Exercise> Exercises { get; set; }


    }
}
