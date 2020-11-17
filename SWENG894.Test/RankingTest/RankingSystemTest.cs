using SWENG894.Ranking;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SWENG894.Test.RankingTest
{
    public class RankingSystemTest
    {
        [Fact]
        public void GetClusterCentersTest()
        {
            List<int> Ratings = new List<int>();
            int NumRankings = 5;
            var cut = new RankingSystem();

            for(int i = 0; i < 20; i++)
            {
                Ratings.Add(i);
                Ratings.Add(i);
            }

            var Centers = cut.GetInitialClusterCenters(Ratings, NumRankings);

            Assert.True(Centers.Count == 5);
            Assert.True(Centers.ElementAt(0) == 0);
            Assert.True(Centers.ElementAt(4) == 32);

            Ratings.Add(21);
            Ratings.Sort();

            Centers = cut.GetInitialClusterCenters(Ratings, NumRankings);
            Assert.True(Centers.Count == 5);
            Assert.True(Centers.ElementAt(0) == 0);
            Assert.True(Centers.ElementAt(4) == 32);

        }
        [Fact]
        public void GetClosestClusterTest()
        {
            var cut = new RankingSystem();
            List<int> Clusters = new List<int>
            {
                1,
                7,
                12,
                55,
                99
            };

            var closest = cut.GetClosestCluster(88, Clusters);
            Assert.True(closest == 4);
            closest = cut.GetClosestCluster(9, Clusters);
            Assert.True(closest == 1);
            closest = cut.GetClosestCluster(44, Clusters);
            Assert.True(closest == 3);
            closest = cut.GetClosestCluster(3, Clusters);
            Assert.True(closest == 0);

        }

        [Fact]
        public void CalculateAverageForClusterCenterTest()
        {
            var cut = new RankingSystem();
            List<int> Clusters = new List<int>
            {
                1,
                7,
                12,
                55,
                99
            };

            List<RankingSystem.KMeansRating> Ratings = new List<RankingSystem.KMeansRating>();
            RankingSystem.KMeansRating rating = new RankingSystem.KMeansRating();
            rating.ClosestCluster = 1;
            rating.Rating = 33;
            RankingSystem.KMeansRating rating2 = new RankingSystem.KMeansRating();
            rating2.ClosestCluster = 0;
            rating2.Rating = 10;
            RankingSystem.KMeansRating rating3 = new RankingSystem.KMeansRating();
            rating3.ClosestCluster = 0;
            rating3.Rating = 20;

            Ratings.Add(rating);
            Ratings.Add(rating2);
            Ratings.Add(rating3);

            int avg = cut.CalculateAverageForClusterCenter(Ratings, Clusters, 0);
            Assert.True(avg == 15);

            avg = cut.CalculateAverageForClusterCenter(Ratings, Clusters, 1);
            Assert.True(avg == 33);

            avg = cut.CalculateAverageForClusterCenter(Ratings, Clusters, 3);
            Assert.True(avg == 55);
        }

        [Fact]
        public void GenerateRankingsTest()
        {

            Random r = new Random();
            List<int> Ratings = new List<int>();

            for(int i = 0; i < 2000; i++)
            {
                Ratings.Add(r.Next(0, 3000));
            }

            var cut = new RankingSystem();
            //The centers tend to converge after around 40 iterations
            var Centers = cut.GenerateRankings(5, Ratings, 10000);

            //These values should be visually inspected. The data set is too large to compute by hand
            //and since they are randomly generated, they may not always be the same. In general, the data set
            Assert.True(Centers.Count == 5);
        }
    }
}
