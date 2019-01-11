using System;
using TennisMatch.Events;
using System.Collections.Generic;
using System.Linq;

namespace TennisMatch.Domain
{
    public class Game
    {
        public string Status { get; set; }
        public Guid SetGuid { get; set; }
        public Guid MatchGuid { get; set; }
        public Guid GameGuid { get; set; }
        public int PlayerOnePoints { get; set; }
        public int PlayerTwoPoints { get; set; }

        internal List<IEvent> HandlePlayerOneScorePoint()
        {
            var result = new List<IEvent>();
            PlayerOnePoints++;
            result.Add(new PlayerOneWonPoint
            {
                MatchGuid = this.MatchGuid,
                SetGuid = this.SetGuid,
                GameGuid = this.GameGuid
            });
            if (IsGameComplete())
            {
                result.Add(new GameCompleted
                {
                    MatchGuid = this.MatchGuid,
                    SetGuid = this.SetGuid,
                    GameGuid = this.GameGuid,
                    PlayerOnePoints = this.PlayerOnePoints,
                    PlayerTwoPoints = this.PlayerTwoPoints,
                });
            }
            return result;
        }

        public bool IsGameComplete()
        {
            return false
                || IsGameWonByPlayer(PlayerOnePoints, PlayerTwoPoints)
                || IsGameWonByPlayer(PlayerTwoPoints, PlayerOnePoints);
        }

        private static bool IsGameWonByPlayer(int points1, int points2)
        {
            return (points1 >= 4) && ((points1 - points2) >= 2);
        }

        public Winners GetWinner()
        {
            if (!IsGameComplete())
                return Winners.Unknown;
            Winners result = Winners.Unknown;
            if (PlayerOnePoints > PlayerTwoPoints)
                result = Winners.P1;
            else if (PlayerTwoPoints > PlayerOnePoints)
                result = Winners.P2;
            return result;
        }
    }
}