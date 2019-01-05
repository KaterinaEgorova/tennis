using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisMatch.Commands
{
    public class CreateMatch: ICommand
    {
        public Guid MatchGuid { get; set; }
    }
}
