using System;
using TennisMatch.Events;

namespace TennisMatch.Events
{
    public class SetCompleted : IEvent
    {
        public Guid MatchGuid { get; set; }
        public Guid SetGuid { get; set; }
        public int PlayerOneoints { get; set; }
        public int PlayerTwoPoints { get; set; }
        public int PlayerOneTieBreakPoints { get; set; }
        public int PlayerTwoTieBreakPoints { get; set; }
    }
}