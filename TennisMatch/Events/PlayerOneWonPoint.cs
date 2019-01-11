using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisMatch.Events
{
    public class PlayerOneWonPoint: IEvent
    {
        public Guid MatchGuid { get; set; }
        public Guid SetGuid { get; set; }
        public Guid GameGuid { get; set; }
    }
}
