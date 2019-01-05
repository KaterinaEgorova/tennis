using System;

namespace TennisMatch.Events
{
    public class MatchSetStarted : IEvent
    {
        public Guid MatchGuid { get;  set; }
        public Guid SetGuid { get;  set; }
    }
}