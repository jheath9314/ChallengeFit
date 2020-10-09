using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Models
{
    public class Challenge
    {
        public int Id { get; set; }
        [Required]
        public string ChallengerId { get; set; }
        public ApplicationUser Challenger { get; set; }
        [Required]
        public string ContenderId { get; set; }
        public ApplicationUser Contender { get; set; }
        public int WorkoutId { get; set; }
        public Workout Workout { get; set; }
        public int? ChallengerResultId { get; set; }
        public WorkoutResult ChallengerResult { get; set; }
        public int? ContenderResultId { get; set; }
        public WorkoutResult ContenderResult { get; set; }
        public ChallengeStatus ChallengeProgress { get; set; }

        public enum ChallengeStatus
        {
            New,
            Accepted,
            Rejected,
            CompletedByChallenger,
            CompletedByContender,
            Completed
        }
    }
}
