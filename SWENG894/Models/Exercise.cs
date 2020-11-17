using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SWENG894.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public enum Exercises
        {
            Pushup,
            Benchpress,
            Burpee,
            Dip,
            MuscleUp,
            PullUp,
            AirSquat,
            FrontSquat,
            BackSquat,
            OverheadSquare,
            Deadlift,
            SitUp,
            JumpRope,
            Run,
            Swim,
            Clean,
            Snatch,
            Jerk
        }

        public Exercises Exer { get; set; }

        [DataType(DataType.Duration)]
        public int Reps { get; set; }

        public int WorkoutId { get; set; }
        public Workout Workout { get; set; }

        public int Order { get; set; }
    }
}
