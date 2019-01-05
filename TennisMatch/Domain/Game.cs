using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisMatch.Domain
{
    public class Game
    {
        private int points1 = 0; // points of player 1
        private int points2 = 0; // points of player 2

        public void ScoreP1()
        {
            if (IsComplete())
                throw new ApplicationException("Cannot score. Game is complete.");
            points1++;
        }

        public void ScoreP2()
        {
            if (IsComplete())
                throw new ApplicationException("Cannot score. Game is complete.");
            points2++;
        }

        public bool IsComplete()
        {
            return false
                || IsGameWonByPlayer(points1, points2)
                || IsGameWonByPlayer(points2, points1);
        }

        public Winners GetWinner()
        {
            if (!IsComplete())
                return Winners.Unknown;

            Winners result = Winners.Unknown;
            if (points1 > points2)
                result = Winners.P1;
            else if (points2 > points1)
                result = Winners.P2;

            return result;
        }

        public override string ToString()
        {
            return $"P1: {GetScoreAsString(points1)} - P2: {GetScoreAsString(points2)}";
        }

        private static bool IsGameWonByPlayer(int points1, int points2)
        {
            return (points1 >= 4) && ((points1 - points2) >= 2);
        }

        private static string GetScoreAsString(int points)
        {
            string result = string.Empty;
            switch (points)
            {
                case 0: result = "0"; break;
                case 1: result = "15"; break;
                case 2: result = "30"; break;
                case 3: result = "40"; break;
                default:
                    if (points > 3)
                        result = $"40 ({points})";
                    break;
            }

            return result;
        }
    }
}
