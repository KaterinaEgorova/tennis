using System;

namespace TennisMatch.Commands
{
    public class StartMatchSet : ICommand
    {
        public Guid MatchGuid { get; set; }
        public Guid SetGuid { get; set; }
    }
}