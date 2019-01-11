using System;
using TennisMatch.Events;

namespace TennisMatch.Events
{
    public class GameCompleted : IEvent
    {
        public Guid MatchGuid { get; internal set; }
        public Guid SetGuid { get; internal set; }
        public Guid GameGuid { get; internal set; }
        public int PlayerOnePoints { get; internal set; }
        public int PlayerTwoPoints { get; internal set; }
    }
}
