using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Ranking
{
    public class RankingSystem
    {
        public List<int> GenerateRankings(int NumRankings, List<int> Ratings)
        {
            Ratings.Sort();
            return null;
            //Add the ratings to the struct
            List<KMeansRating> KMeansRatings = new List<KMeansRating>();
            for(int i = 0; i < Ratings.Count; i++)
            {
                KMeansRating rating;
                rating.ClosestCluster = 0;
                rating.Rating = Ratings.ElementAt(0);
                KMeansRatings.Add(rating);
            }

            var Centers = GetInitialClusterCenters(Ratings, NumRankings);
        }

        public int GetClosestCluster(int Rating, List<int> Clusters)
        {
            int ClosestCluster = 0;
            int ClosestClusterDistance = Math.Abs(Rating - Clusters.ElementAt(0));

            for (int i = 1; i < Clusters.Count; i++)
            { 
                if(Math.Abs(Rating - Clusters.ElementAt(i)) < ClosestClusterDistance)
                {
                    ClosestClusterDistance = Math.Abs(Rating - Clusters.ElementAt(i));
                    ClosestCluster = i;
                }
            }

            return ClosestCluster;
        }

        public List<int> GetInitialClusterCenters(List<int> Ratings, int NumRankings)
        {
            List<int> Centers = new List<int>();

            Double Difference = Ratings.Count / NumRankings;

            for(int i = 0; i < NumRankings; i++)
            {
                int val = (int)(Math.Round(i * Difference));
                Centers.Add(val);
            }

            return Centers;
        }

        public struct KMeansRating
        {
            public int Rating;
            public int ClosestCluster;
        }
    }

}
