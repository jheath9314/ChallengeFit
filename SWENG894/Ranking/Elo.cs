using System;

namespace SWENG894.Ranking
{
    public class Elo
    {
        public enum Winner
        {
            P1,
            P2
        }

        private int GetKFactor()
        {
            return 30;
        }
        public void CalculateRating(ref int P1Rating, ref int P2Rating, Winner winner)
        {
            var P1ProbWin = CalculateP1ProbabilityOfVictory(P1Rating, P2Rating);
            var P2ProbWin = 1.0 - P1ProbWin;

            if(winner == Winner.P1)
            {
                P1Rating = (int)Math.Round(P1Rating + GetKFactor() * (1 - P1ProbWin));
                P2Rating = (int)Math.Round(P2Rating + GetKFactor() * (0 - P2ProbWin));
            }
            else
            {
                P1Rating = (int)Math.Round(P1Rating + GetKFactor() * (0 - P1ProbWin));
                P2Rating = (int)Math.Round(P2Rating + GetKFactor() * (1 - P2ProbWin));
            }
        }

        public double CalculateP1ProbabilityOfVictory(int P1Rating, int P2Rating)
        {
            double Probability;
            double RatingDifference = P2Rating - P1Rating;
            double expValue = (RatingDifference / 400);

            Probability = 1 / (1 + Math.Pow(10, expValue));
            return Probability;
        }
    }
}
