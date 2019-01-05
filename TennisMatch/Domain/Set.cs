using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisMatch.Domain
{
    public class Set
    {
        public Set()
        {
            Games = new List<Game>();
        }
        public string Status { get; internal set; }
        public Guid MatchGuid { get; internal set; }
        public Guid SetGuid { get; internal set; }
        public List<Game> Games { get; internal set; }
    }
}
