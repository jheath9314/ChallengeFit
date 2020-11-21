using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Models
{
    public class Ranking
    {
        public int Id { get; set; }
        public int BronzeValue { get; set; }
        public int SilverValue { get; set; }
        public int GoldValue { get; set; }
        public int PlatinumValue { get; set; }
        public int DiamondValue { get; set; }
        public string Timestamp { get; set; }
    }
}
