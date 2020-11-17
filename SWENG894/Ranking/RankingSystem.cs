using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Ranking
{
    public class RankingSystem
    {
        public List<int> GenerateRankings(int NumRankings, List<int> Ratings, int maxIterations)
        {
            int currentIteration = 0;
            bool updateOccured = true;
            Ratings.Sort();
            //Add the ratings to the struct to easily correlate a rating and closest cluster center
            List<KMeansRating> KMeansRatings = new List<KMeansRating>();
            for(int i = 0; i < Ratings.Count; i++)
            {
                KMeansRating rating = new KMeansRating();
                rating.ClosestCluster = 0;
                rating.Rating = Ratings.ElementAt(i);
                KMeansRatings.Add(rating);
            }

            List<int> Centers = GetInitialClusterCenters(Ratings, NumRankings);

            //Run the algorithm until no updates occur or we've exceeded our max iteration count
            while (currentIteration < maxIterations && updateOccured)
            {
                currentIteration++;
                updateOccured = false;
                for (int i = 0; i < KMeansRatings.Count; i++)
                {
                    int UpdatedCenter = GetClosestCluster(KMeansRatings.ElementAt(i).Rating, Centers);
                    KMeansRatings.ElementAt(i).ClosestCluster = UpdatedCenter;
                }

                //This code could be made more efficient
                //Calculate the new cluster average
                for (int i = 0; i < Centers.Count; i++)
                {
                    Centers[i] = CalculateAverageForClusterCenter(KMeansRatings, Centers, i);
                }

                for (int i = 0; i < KMeansRatings.Count; i++)
                {
                    int UpdatedCenter = GetClosestCluster(KMeansRatings.ElementAt(i).Rating, Centers);
                    if(KMeansRatings.ElementAt(i).ClosestCluster != UpdatedCenter)
                    {
                        updateOccured = true;
                    }

                    KMeansRatings.ElementAt(i).ClosestCluster = UpdatedCenter;
                }
            }

            return Centers;
        }

        //Returns the closest clusters for a given rating
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

        //Given a cluster index, clusters, and ratings, average all the ratings that are closed to a given cluster.
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

            //This case is very unlikely to happen. If we find a cluster with no elements tied to it
            //move on. It will be adjusted as other clusters are adjusted. The case that a cluster center
            //has no data points tied to it is very unlikely with any significant data set.
            if(count == 0)
            {
                return Clusters.ElementAt(ClusterIndex);
            }
            return (int)Math.Round((decimal)runningValue / (decimal)count);
        }

        //Returns a roughly uniform division of cluster centers. It's not exact as it starts at 0, but that
        //is quickly corrected by the execution of the algorithm. This function assumes a sorted rating list. 
        //The lack of a sorted rating list will not break the function, but results in slower execution and requires
        //more iterations for convergence.
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

        //Helper class for tying a Rating and closest cluster together
        public class KMeansRating
        {
            public int Rating;
            public int ClosestCluster;
        }
    }

}
