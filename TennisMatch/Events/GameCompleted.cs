﻿using System;
using TennisMatch.Events;

namespace TennisMatch.Events
{
    public class GameCompleted : IEvent
    {
        public Guid MatchGuid { get; set; }
        public Guid SetGuid { get; set; }
        public Guid GameGuid { get; set; }
        public int PlayerOnePoints { get; set; }
        public int PlayerTwoPoints { get; set; }
    }
}
