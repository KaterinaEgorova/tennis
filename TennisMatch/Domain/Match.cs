using System;
using System.Collections.Generic;
using System.Linq;
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
                this.OnSetStarted(evt as MatchSetStarted);
            }
            else if (evt is MatchSetGameStarted)
            {
                this.OnMatchSetGameStarted(evt as MatchSetGameStarted);
            }
            else if (evt is PlayerOneWonPoint)
            {
                this.OnPlayerOneWonPoint(evt as PlayerOneWonPoint);
            }
            else if (evt is PlayerTwoWonPoint)
            {
                this.OnPlayerTwoWonPoint(evt as PlayerTwoWonPoint);
            }
            else if (evt is GameCompleted)
            {
                this.OnGameCompleted(evt as GameCompleted);
            }
            else if (evt is SetCompleted)
            {
                this.OnSetCompleted(evt as SetCompleted);
            }
            else if (evt is TiebreakPointWonByPlayerOne)
            {
                this.OnTieBreackPointWonByPlayerOne(evt as TiebreakPointWonByPlayerOne);
            }
            else if (evt is TiebreakPointWonByPlayerTwo)
            {
                this.OnTieBreakPointWonByPlayerOne(evt as TiebreakPointWonByPlayerTwo);
            }
        }

        private void OnTieBreakPointWonByPlayerOne(TiebreakPointWonByPlayerTwo evt)
        {
            this.Sets.First(x => x.SetGuid == evt.SetGuid).PlayerTwoTieBreakPoints++;
        }

        private void OnTieBreackPointWonByPlayerOne(TiebreakPointWonByPlayerOne evt)
        {
            this.Sets.First(x => x.SetGuid == evt.SetGuid).PlayerOneTieBreakPoints++;
        }

        private void OnSetCompleted(SetCompleted evt)
        {
            this.Sets.First(x => x.SetGuid == evt.SetGuid).Status = Finished;
        }

        private void OnGameCompleted(GameCompleted evt)
        {
            this.Sets.First(x => x.SetGuid == evt.SetGuid).
                Games.First(x => x.GameGuid == evt.GameGuid).Status = Finished;
        }

        private void OnPlayerTwoWonPoint(PlayerTwoWonPoint evt)
        {
            this.Sets.First(x => x.SetGuid == evt.SetGuid).
                Games.First(x => x.GameGuid == evt.GameGuid).PlayerTwoPoints++;
        }

        private void OnPlayerOneWonPoint(PlayerOneWonPoint evt)
        {
            this.Sets.First(x => x.SetGuid == evt.SetGuid).
                Games.First(x => x.GameGuid == evt.GameGuid).PlayerOnePoints++;
        }

        private void OnMatchSetGameStarted(MatchSetGameStarted evt)
        {
            this.Sets.First(x => x.SetGuid == evt.SetGuid).
                Games.Add(new Game {
                    Status = InProgress,
                    SetGuid = evt.SetGuid,
                    MatchGuid = evt.MatchGuid,
                    GameGuid = evt.GameGuid });
        }

        private void OnSetStarted(MatchSetStarted evt)
        {
            if (this.Sets == null)
            {
                this.Sets = new List<Set>();
            }
            this.Sets.Add(new Set {
                Status = InProgress,
                SetGuid = evt.SetGuid,
                MatchGuid = evt.MatchGuid });
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
            else if (command is ScorePointForPlayerOne)
            {
                return ScorePointForPlayerOne(command as ScorePointForPlayerOne);
            }
            else if (command is ScorePointForPlayerTwo)
            {
                return ScorePointForPlayerTwo(command as ScorePointForPlayerTwo);
            }
            throw new NotImplementedException();
        }

        private List<IEvent> ScorePointForPlayerTwo(ScorePointForPlayerTwo command)
        {
            var errors = new List<string>();
            if (this.Status != InProgress)
            {
                errors.Add("Status: Match status must be in progress.");
            }

            var result = new List<IEvent>();
            Set currentSet;
            if (this.Sets == null || !this.Sets.Any(x => x.Status == InProgress))
            {
                currentSet = new Set
                {
                    MatchGuid = this.MatchGuid,
                    SetGuid = Guid.NewGuid()
                };
                result.Add(new MatchSetStarted
                {
                    MatchGuid = this.MatchGuid,
                    SetGuid = currentSet.SetGuid
                });
            }
            else
            {
                currentSet = Sets.First(x => x.Status == InProgress);
            }
            result.AddRange(currentSet.HandlePlayerTwoScorePoint());
            if (IsComplete())
            {
                result.Add(new MatchComplete {
                    MatchGuid = this.MatchGuid
                });
            }
            return result;

        }

        private bool IsComplete()
        {
            //todo: implement 
            return false;
        }

        private List<IEvent> ScorePointForPlayerOne(ScorePointForPlayerOne command)
        {
            var errors = new List<string>();
            if (this.Status != InProgress)
            {
                errors.Add("Status: Match status must be in progress.");
            }
            
            var result = new List<IEvent>();
            Set currentSet;
            if (this.Sets == null || !this.Sets.Any(x=>x.Status ==InProgress))
            {
                currentSet = new Set
                {
                    MatchGuid = this.MatchGuid,
                    SetGuid = Guid.NewGuid()
                };
                result.Add(new MatchSetStarted {
                    MatchGuid = this.MatchGuid,
                    SetGuid = currentSet.SetGuid
                });
            }
            else
            {
                currentSet = Sets.First(x => x.Status == InProgress);
            }
            result.AddRange(currentSet.HandlePlayerOneScorePoint());
            
            return result;
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
