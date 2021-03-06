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
            Rounds
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "You must provide a name")]
        public String Name { get; set; }

        //If scoring is reps, would this still be time?
        [Display(Name = "Time (Minutes and Seconds)")]
        public int Time { get; set; }

        [DataType(DataType.MultilineText)]
        public String Notes { get; set; }

        [Display(Name = "Scoring Type")]
        public Scoring ScoringType {get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Scaling Options")]
        public String ScalingOptions { get; set; }

        public List<Exercise> Exercises { get; set; }

        public bool Published { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string GetTimeDisplayString()
        {
            if(ScoringType == Scoring.Time)
            {
                string minutes = (Time / 60).ToString();
                string seconds = (Time % 60).ToString();
                if((Time % 60) < 10)
                {
                    seconds = "0" + seconds;
                }
                string displayValue = minutes + ":" + seconds;
                return displayValue;
            }
            else
            {
                return Time.ToString();
            }
        }

        [NotMapped]
        public bool IsFavorite { get; set; }

    }
}
