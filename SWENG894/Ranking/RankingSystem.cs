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
            //return null;
            //Add the ratings to the struct
            List<KMeansRating> KMeansRatings = new List<KMeansRating>();
            for(int i = 0; i < Ratings.Count; i++)
            {
                KMeansRating rating = new KMeansRating();
                rating.ClosestCluster = 0;
                rating.Rating = Ratings.ElementAt(0);
                KMeansRatings.Add(rating);
            }

            List<int> Centers = GetInitialClusterCenters(Ratings, NumRankings);

            //Do outer loops here for repeating the steps
            //Assign each point to the closest cluster center
            for(int i = 0; i < KMeansRatings.Count; i++)
            {
                int UpdatedCenter = GetClosestCluster(KMeansRatings.ElementAt(i).Rating, Centers);
                KMeansRatings.ElementAt(i).ClosestCluster = UpdatedCenter;
            }

            //This code could be made more efficient
            //Calculate the new cluster average
            for(int i = 0; i < KMeansRatings.Count; i++)
            {
                Centers[i] = CalculateAverageForClusterCenter(KMeansRatings, Centers, i);
            }

            //TODO: Check for a change, if it occurs, continue to run the algorithm
            for (int i = 0; i < KMeansRatings.Count; i++)
            {
                int UpdatedCenter = GetClosestCluster(KMeansRatings.ElementAt(i).Rating, Centers);
                KMeansRatings.ElementAt(i).ClosestCluster = UpdatedCenter;
            }


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

        public int CalculateAverageForClusterCenter(List<KMeansRating> Ratings, List<int> Clusters, int ClusterIndex)
        {
            int count = 0;
            int runningValue = 0;
            for(int i = 0; i < Ratings.Count; i++)
            {
                if(Ratings.ElementAt(i).ClosestCluster == ClusterIndex)
                {
                    count++;
                    runningValue += Ratings.ElementAt(i).Rating;

                }
            }

            //Not sure how to handle this case. It probably shouldn't happen
            if(count == 0)
            {
                return Clusters.ElementAt(ClusterIndex);
            }
            return (int)Math.Round((decimal)runningValue / (decimal)count);
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

        public class KMeansRating
        {
            public int Rating;
            public int ClosestCluster;
        }
    }

}
