using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public int Reps { get; set; }

    }
}
