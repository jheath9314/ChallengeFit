using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Models
{
    public class NewsFeed
    {
        public int Id { get; set; }
        public string Description { get; set; }
        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime CreateDate { get; set; }
        public FeedTypes FeedType { get; set; }
        public string RelatedUserId { get; set; }
        public ApplicationUser RelatedUser { get; set; }
        public int? RelatedWorkoutId { get; set; }
        public Workout RelatedWorkout { get; set; }
        public int? RelatedChallengeId { get; set; }
        public Challenge RelatedChallenge { get; set; }
        public bool Dismissed { get; set; }

        public enum FeedTypes
        {
            PublishedWorkout,
            CompletedWorkout,
            SentChallenge,
            AcceptedChallenge,
            CompletedChallenge
        }
    }
}
