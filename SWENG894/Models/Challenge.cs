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

        [Display(Name = "Challenger Result")]
        public WorkoutResult ChallengerResult { get; set; }
        public int? ContenderResultId { get; set; }

        [Display(Name = "Contender Result")]
        public WorkoutResult ContenderResult { get; set; }

        [Display(Name = "Challenge Status")]
        public ChallengeStatus ChallengeProgress { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }

        public enum ChallengeStatus
        {
            New,
            Accepted,          
            CompletedByChallenger,
            CompletedByContender,
            Rejected,
            Completed,
            Canceled
        }
    }
}
