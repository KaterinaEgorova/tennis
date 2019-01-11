using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisMatch.Events;

namespace TennisMatch.Domain
{
    public class Set
    {
        private const string Finished = "Finished";
        private const string InProgress = "In Progress";

        public Set()
        {
            Games = new List<Game>();
        }
        private const int MarginToWin = 2;
        private const int NumGamesToWinASet = 6;
        private const int TieBreakPointsToWin = 7;

        public string Status { get; set; }
        public Guid MatchGuid { get; set; }
        public Guid SetGuid { get; set; }
        public List<Game> Games { get; set; }
        public int PlayerOnePoints { get; set; }
        public int PlayerOneTieBreakPoints { get; set; }
        public int PlayerTwoPoints { get; set; }
        public int PlayerTwoTieBreakPoints { get; set; }

        internal Game StartGame(Guid gameGuid)
        {
            var game = new Game
            {
                MatchGuid = this.MatchGuid,
                SetGuid = this.SetGuid,
                GameGuid = gameGuid,
            };
            this.Games.Add(game);
            return game;
        }

        internal List<IEvent> HandlePlayerOneScorePoint()
        {
            var result = new List<IEvent>();
            var points = GetCurrentPoints();
            
            if (IsTiebreak(points.Item1, points.Item2))
            {
                PlayerOneTieBreakPoints++;
                result.Add(new TiebreakPointWonByPlayerOne
                {
                    MatchGuid = this.MatchGuid,
                    SetGuid = this.SetGuid,
                });
            }
            else
            {
                Game currentGame;
                if (this.Games == null ||
                    !this.Games.Any(x => x.Status == InProgress))
                {
                    currentGame = this.StartGame(Guid.NewGuid());

                    result.Add(new MatchSetGameStarted
                    {
                        MatchGuid = this.MatchGuid,
                        SetGuid = this.SetGuid,
                        GameGuid = currentGame.GameGuid,
                    });
                }
                else
                {
                    currentGame = this.Games.First(x => x.Status == InProgress);
                }
                result.AddRange(currentGame.HandlePlayerOneScorePoint());
            }
            if (IsSetComplete())
            {
                result.Add(new SetCompleted
                {
                    MatchGuid = this.MatchGuid,
                    SetGuid = this.SetGuid,
                    PlayerOneoints = this.PlayerOnePoints,
                    PlayerTwoPoints = this.PlayerTwoPoints,
                    PlayerOneTieBreakPoints = this.PlayerOneTieBreakPoints,
                    PlayerTwoTieBreakPoints = this.PlayerTwoTieBreakPoints
                });
            }
            return result;
        }

        private bool IsSetComplete()
        {
            var points = GetCurrentPoints();
            return false
                || IsSetWonByPlayer(points.Item1, this.PlayerOneTieBreakPoints, points.Item2, this.PlayerTwoTieBreakPoints)
                || IsSetWonByPlayer(points.Item2, this.PlayerTwoTieBreakPoints, points.Item1, this.PlayerOneTieBreakPoints);
        }

        private static bool IsSetWonByPlayer(int gamePoints1, int tiebreakPoints1, int gamePoints2, int tiebreakPoints2)

        {
            return
                ((gamePoints1 >= NumGamesToWinASet) && ((gamePoints1 - gamePoints2) >= MarginToWin)) // set won by 6 games
                || (IsTiebreak(gamePoints1, gamePoints2) // or tiebreak was required
                    && (tiebreakPoints1 >= TieBreakPointsToWin) && ((tiebreakPoints1 - tiebreakPoints2) >= MarginToWin)); // and set won by tiebreak
        }

        internal IEnumerable<IEvent> HandlePlayerTwoScorePoint()
        {
            var result = new List<IEvent>();
            var points = GetCurrentPoints();

            if (IsTiebreak(points.Item1, points.Item2))
            {
                PlayerTwoTieBreakPoints++;
                result.Add(new TiebreakPointWonByPlayerTwo
                {
                    MatchGuid = this.MatchGuid,
                    SetGuid = this.SetGuid,
                });
            }
            else
            {
                Game currentGame;
                if (this.Games == null ||
                    !this.Games.Any(x => x.Status == InProgress))
                {
                    currentGame = this.StartGame(Guid.NewGuid());

                    result.Add(new MatchSetGameStarted
                    {
                        MatchGuid = this.MatchGuid,
                        SetGuid = this.SetGuid,
                        GameGuid = currentGame.GameGuid,
                    });
                }
                else
                {
                    currentGame = this.Games.First(x => x.Status == InProgress);
                }
                result.AddRange(currentGame.HandlePlayerTwoScorePoint());
            }
            if (IsSetComplete())
            {
                result.Add(new SetCompleted
                {
                    MatchGuid = this.MatchGuid,
                    SetGuid = this.SetGuid,
                    PlayerOneoints = this.PlayerOnePoints,
                    PlayerTwoPoints = this.PlayerTwoPoints,
                    PlayerOneTieBreakPoints = this.PlayerOneTieBreakPoints,
                    PlayerTwoTieBreakPoints = this.PlayerTwoTieBreakPoints
                });
            }
            return result;
        }

        private static bool IsTiebreak(int points1, int points2)
        {
            return ((points1 == points2) && (points1 >= NumGamesToWinASet));
        }

        private Tuple<int, int> GetCurrentPoints()
        {
            int points1 = 0;
            int points2 = 0;

            foreach (var g in Games)
            {
                var winner = g.GetWinner();
                if (winner == Winners.P1)
                    points1++;
                else if (winner == Winners.P2)
                    points2++;
            }



            return new Tuple<int, int>(points1, points2);

        }
    }
}
