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

        //K factor determines the maximum amount a score can change. It is possible for this to be dynamic. In other
        //works, a higher rating may carry a lower k factor. For now, assume a uniform K factor
        private int GetKFactor()
        {
            return 30;
        }

        //Taking in the rating of both players, calculate the new rating based on the 
        //outcome of the challenge. The more unlikely the outcome, the more drastic
        //the rating change
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

        //Returns the probability of P1 winning a match based on the ratings of P1 and P2
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
