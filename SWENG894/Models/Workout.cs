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
        public int Id { get; set; }

        public String Name { get; set; }

        public int Time { get; set; }

        public String Notes { get; set; }

        public List<Exercise> Exercises { get; set; }


    }
}
