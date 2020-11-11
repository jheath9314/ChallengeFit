using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SWENG894.Ranking;
using Org.BouncyCastle.Asn1.Cmp;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crypto.Tls;

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
            List<int> Clusters = new List<int>();
            Clusters.Add(1);
            Clusters.Add(7);
            Clusters.Add(12);
            Clusters.Add(55);
            Clusters.Add(99);

            var closest = cut.GetClosestCluster(88, Clusters);
            Assert.True(closest == 4);
            closest = cut.GetClosestCluster(9, Clusters);
            Assert.True(closest == 1);
            closest = cut.GetClosestCluster(44, Clusters);
            Assert.True(closest == 3);
            closest = cut.GetClosestCluster(3, Clusters);
            Assert.True(closest == 0);


        }
    }
}
