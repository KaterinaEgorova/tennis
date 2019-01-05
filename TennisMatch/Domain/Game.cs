using System;

namespace TennisMatch.Domain
{
    public class Game
    {
        public string Status { get; set; }
        public Guid SetGuid { get; set; }
        public Guid MatchGuid { get; set; }
        public Guid GameGuid { get; set; }
    }
}