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
        private const string Finished = "Finished";
        private const string InProgress = "In Progress";

        private string Status { get; set; }
        private Guid MatchGuid { get; set; }
        private List<Set> Sets { get; set; }

        public void Hydrate(IEvent evt)
        {
            if (evt is MatchCreated)
            {
                this.OnMatchCreated(evt as MatchCreated);
            }
            else if (evt is MatchSetStarted)
            {
                this.OnMatchStarted(evt as MatchSetStarted);
            }
            else if (evt is MatchSetGameStarted)
            {
                this.OnMatchSetGameStarted(evt as MatchSetGameStarted);
            }
        }

        private void OnMatchSetGameStarted(MatchSetGameStarted evt)
        {
            this.Sets.First(x => x.SetGuid == evt.SetGuid).
                Games.Add(new Game {Status = InProgress, SetGuid = evt.SetGuid, MatchGuid = evt.MatchGuid, GameGuid = evt.GameGuid });
        }

        private void OnMatchStarted(MatchSetStarted evt)
        {
            if (this.Sets == null)
            {
                this.Sets = new List<Set>();
            }
            this.Sets.Add(new Set { Status = InProgress, SetGuid = evt.SetGuid, MatchGuid = evt.MatchGuid });
        }

        private void OnMatchCreated(MatchCreated evt)
        {
            this.MatchGuid = evt.MatchGuid;
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
            else if (command is StartMatchSetGame)
            {
                return StartSetGame(command as StartMatchSetGame);
            }
            throw new NotImplementedException();
        }

        private List<IEvent> StartSetGame(StartMatchSetGame command)
        {
            var errors = new List<string>();
            var set = this.Sets.FirstOrDefault(x => x.SetGuid == command.SetGuid);
            if (set == null)
            {
                errors.Add("Set: Set not found");
            }
            if (errors.Any())
                throw new ApplicationException(string.Join(";", errors));
            if (set.Status == Finished)
            {
                errors.Add("Set: Set is finished");
            }
            if (errors.Any())
                throw new ApplicationException(string.Join(";", errors));
            return new List<IEvent> { new MatchSetGameStarted{
                MatchGuid = command.MatchGuid,
                SetGuid = command.SetGuid,
                GameGuid = command.GameGuid
            } };
        }

        private List<IEvent> StartMatchSet(StartMatchSet command)
        {
            var errors = new List<string>();
            if (this.Sets != null && this.Sets.Any(x=>x.Status == InProgress))
            {
                errors.Add("Sets: Cannot start Set while other set is in progress" );
            }
            if(this.Status == Finished)
            {
                errors.Add("MatchStatus: Match is finished.");
            }

            if (errors.Any())
                throw new ApplicationException(string.Join(";", errors));

            return new List<IEvent> { new MatchSetStarted { MatchGuid = command.MatchGuid, SetGuid = command.SetGuid } };
        }

        private List<IEvent> CreateMatch(CreateMatch command)
        {

            var evt = new MatchCreated
            {
                MatchGuid = command.MatchGuid
            };
            return new List<IEvent> { evt };
        }
    }
}
