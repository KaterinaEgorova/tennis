using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisMatch.Events;
using TennisMatch.Commands;

namespace TennisMatch.Domain
{
    public class Match
    {
        private readonly string InProgress = "In Progress";

        private string Status { get; set; }
        private Guid MatchGuid { get; set; }
        private List<Set> Sets { get; set; }

        public void Hydrate(IEvent evt)
        {
            if (evt is MatchCreated)
            {
                this.OnMatchCreated(evt as MatchCreated);
            }
            if (evt is MatchSetStarted)
            {
                this.OnMatchStarted(evt as MatchSetStarted);
            }
        }

        private void OnMatchStarted(MatchSetStarted matchSetStarted)
        {
            if (this.Sets == null)
            {
                this.Sets = new List<Set>();
            }
            this.Sets.Add(new Set { Status = InProgress, SetGuid = matchSetStarted.SetGuid, MatchGuid = matchSetStarted.MatchGuid });
        }

        private void OnMatchCreated(MatchCreated matchCreated)
        {
            this.MatchGuid = matchCreated.MatchGuid;
            this.Status = InProgress;
        }

        public List<IEvent> HandleCommand(ICommand command)
        {
            if (command is CreateMatch)
            {
                return CreateMatch(command as CreateMatch);
            }
            else if (command is StartMatchSet)
            {
                return StartMatchSet(command as StartMatchSet);
            }
            throw new NotImplementedException();
        }

        private List<IEvent> StartMatchSet(StartMatchSet startSet)
        {
            var errors = new List<ValidationError>();
            if (this.Sets != null && this.Sets.Any(x=>x.Status == InProgress))
            {
                errors.Add(new ValidationError { FieldName = "Sets", ErrorMessage = "Cannot start Set while other set is in progress." });
            }
            if(this.Status == "Finished")
            {
                errors.Add(new ValidationError { FieldName = "MatchStatus", ErrorMessage = "Match is finished." });
            }
            return new List<IEvent> { new MatchSetStarted { MatchGuid = startSet.MatchGuid, SetGuid = startSet.SetGuid } };
        }

        private List<IEvent> CreateMatch(CreateMatch createMatchCommand)
        {

            var evt = new MatchCreated
            {
                MatchGuid = createMatchCommand.MatchGuid
            };
            return new List<IEvent> { evt };
        }
    }
}
