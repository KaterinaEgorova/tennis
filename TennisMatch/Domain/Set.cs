using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisMatch.Domain
{
    public class Set
    {
        private const int MarginToWin = 2;
        private const int NumGamesToWinASet = 6;
        private const int TieBreakPointsToWin = 7;

        private readonly List<Game> games = new List<Game>();
        private int tiebreakPoints1 = 0;
        private int tiebreakPoints2 = 0;

        IReadOnlyList<Game> Games { get { return games; } }

        public Game AddGame()
        {
            if (IsComplete())
                throw new ApplicationException("Cannot add a game. Set is complete.");
            if (games.Any(x=>!x.IsComplete()))
                throw new ApplicationException("Cannot start a game while another game is in progress.");

            var game = new Game();
            games.Add(game);
            return game;
        }

        public bool IsTiebreak()
        {
            var points = GetCurrentPoints();
            return IsTiebreak(points.Item1, points.Item2);
        }

        public void ScoreTiebreakP1()
        {
            if (IsTiebreak())
                tiebreakPoints1++;
        }

        public void ScoreTiebreakP2()
        {
            if (IsTiebreak())
                tiebreakPoints2++;
        }

        public bool IsComplete()
        {
            var points = GetCurrentPoints();
            return false
                || IsSetWonByPlayer(points.Item1, tiebreakPoints1, points.Item2, tiebreakPoints2)
                || IsSetWonByPlayer(points.Item2, tiebreakPoints2, points.Item1, tiebreakPoints1);
        }

        public Winners GetWinner()
        {
            var result = Winners.Unknown;
            var points = GetCurrentPoints();
            if (IsSetWonByPlayer(points.Item1, tiebreakPoints1, points.Item2, tiebreakPoints2))
                result = Winners.P1;
            else if (IsSetWonByPlayer(points.Item2, tiebreakPoints2, points.Item1, tiebreakPoints1))
                result = Winners.P2;
            return result;
        }

        public override string ToString()
        {
            var points = GetCurrentPoints();
            int s1 = 0;
            int s2 = 0;

            // add a point before display if tiebreak is resolved
            if ((tiebreakPoints1 - tiebreakPoints2) >= 2)
                s1 = 1;
            else if ((tiebreakPoints2 - tiebreakPoints1) >= 2)
                s2 = 1;

            return $"{GetScoreAsString(points.Item1 + s1, tiebreakPoints1)}-{GetScoreAsString(points.Item2 + s2, tiebreakPoints2)}";
        }

        private Tuple<int, int> GetCurrentPoints()
        {
            int points1 = 0;
            int points2 = 0;

            foreach (var g in games)
            {
                var winner = g.GetWinner();
                if (winner == Winners.P1)
                    points1++;
                else if (winner == Winners.P2)
                    points2++;
            }

            return new Tuple<int, int>(points1, points2);
        }

        private static bool IsTiebreak(int points1, int points2)
        {
            return ((points1 == points2) && (points1 >= NumGamesToWinASet));
        }

        private static bool IsSetWonByPlayer(int gamePoints1, int tiebreakPoints1, int gamePoints2, int tiebreakPoints2)
        {
            return
                ((gamePoints1 >= NumGamesToWinASet) && ((gamePoints1 - gamePoints2) >= MarginToWin)) // set won by 6 games
                || (IsTiebreak(gamePoints1, gamePoints2) // or tiebreak was required
                    && (tiebreakPoints1 >= TieBreakPointsToWin) && ((tiebreakPoints1 - tiebreakPoints2) >= MarginToWin)); // and set won by tiebreak
        }

        private static string GetScoreAsString(int gamePoints, int tiebreakPoints)
        {
            if (tiebreakPoints == 0)
                return $"{gamePoints}";
            else
                return $"{gamePoints}({tiebreakPoints})";
        }
    }
}
