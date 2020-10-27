using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWENG894.Models
{
    public class WorkoutResult
    {
        [Key]
        public int Id { get; set; }

        public int WorkoutId { get; set; }
        public Workout Workout { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int Score { get; set; }

        public string ResultNotes { get; set; }

        //These properties can be pulled out of User and Workout

        [NotMapped]
        public String Username { get; set; }

        [NotMapped]
        public String WorkoutName { get; set; }

        [NotMapped]
        public Workout.Scoring ScoringType { get; set; }

        [NotMapped]
        public int? RelatedChallenge { get; set; }

        public string getTimeDisplayString()
        {
            if (ScoringType == Workout.Scoring.Reps)
            {
                
                string displayValue = getMinutesString() + ":" + getSecondsString();
                return displayValue;
            }
            else
            {
                return Score.ToString();
            }
        }

        public string getSecondsString()
        {
            string seconds = (Score % 60).ToString();
            if ((Score % 60) < 10)
            {
                seconds = "0" + seconds;
            }

            return seconds;
            
        }

        public string getMinutesString()
        {
            string minutes = (Score / 60).ToString();
            return minutes;
        }
    }
}
