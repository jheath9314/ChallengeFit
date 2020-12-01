using System;
using Xunit;
using SWENG894.Ranking;


namespace SWENG894.Test.RankingTest
{
    public class EloTest
    {
        [Fact]
        public void TestVictoryProbability()
        {
            var Elo = new Elo();

            int rating1 = 500;
            int rating2 = 500;
            double prob = Elo.CalculateP1ProbabilityOfVictory(rating1, rating2);
            Assert.True(prob == 0.5);

            rating1 = 400;
            rating2 = 600;

            prob = Elo.CalculateP1ProbabilityOfVictory(rating1, rating2);

            Assert.True(prob < 0.5);

            rating1 = 2000;
            rating2 = 1999;

            prob = Elo.CalculateP1ProbabilityOfVictory(rating1, rating2);
            Assert.True(prob > 0.5);
        }

        [Fact]
        public void TestRatingSystem()
        {
            var Elo = new Elo();

            var P1OriginalRating = 1500;
            var P2OriginalRating = 500;

            var P1Rating = P1OriginalRating;
            var P2Rating = P2OriginalRating;

            Elo.CalculateRating(ref P1Rating, ref P2Rating, Elo.Winner.P1);

            //Ensure that when a rating is vastly different, that the expected outcome does not affect ratings
            Assert.True(P1Rating == P1OriginalRating);
            Assert.True(P2Rating == P2OriginalRating);

            P1Rating = P1OriginalRating;
            P2Rating = P2OriginalRating;

            Elo.CalculateRating(ref P1Rating, ref P2Rating, Elo.Winner.P2);

            Assert.True(P1Rating < P1OriginalRating);
            Assert.True(P2Rating > P2OriginalRating);

            var diff = Math.Abs(P1Rating - P1OriginalRating);

            P1Rating = P1OriginalRating - 500;
            P2Rating = P2OriginalRating;

            Elo.CalculateRating(ref P1Rating, ref P2Rating, Elo.Winner.P2);

            var diff2 = Math.Abs((P1OriginalRating - 500 - P1Rating));

            //Ensure that the difference between two ratings affects the rating adjustment
            Assert.True(diff > diff2);

        }
    }
}
