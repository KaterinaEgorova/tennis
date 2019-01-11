using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisMatch.Domain;
using TennisMatch.Events;
using TennisMatch.Commands;


namespace TennisMatch.Tests.Domain
{
    [TestFixture]
    public class MatchTests
    {
        [Test]
        public void CreateMatchCommandShouldReturnMatchCreatedEvent()
        {
            var matchGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209445");
            //given Match
            var match = new Match();
            //when Create match command is executed
            var createMatchCommand = new CreateMatch { MatchGuid = matchGuid};
            // then Match created event has to be published
            var result = match.HandleCommand(createMatchCommand);
            //Then match created event has to be published
            Assert.IsTrue(result.First() is MatchCreated);
            Assert.AreEqual(((MatchCreated)result.First()).MatchGuid, matchGuid);
        }

        [Test]
        public void WhenFirstPoinWonThenSetStartedAmdGameStartedAndPlayerscoredShouldBePublished()
        {
            var matchGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209445");
            //given Match
            var match = new Match();
            //when Create match command is executed
            var matchCreated = new MatchCreated { MatchGuid = matchGuid };
            // then Match created event has to be published
            match.Hydrate(matchCreated);
            // and player 1 won first point
            var result = match.HandleCommand(new ScorePointForPlayerOne
            {
                MatchGuid = matchGuid                
            });
            //then tree events should be published 
            Assert.AreEqual(3, result.Count);
            //and first event should be Set started 
            Assert.IsTrue(result[0] is MatchSetStarted);
            //and second event should be Game Started
            Assert.IsTrue(result[1] is MatchSetGameStarted);
            //and third event should be  player one won point
            Assert.IsTrue(result[2] is PlayerOneWonPoint);
            Assert.AreEqual(((PlayerOneWonPoint)result[2]).MatchGuid, matchGuid);
        }

        [Test]
        public void StartSetCommandShouldReturnSetStartedEvent()
        {
            var matchGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209445");
            //given Match Created occured
            var match = new Match();
            var matchCreated = new MatchCreated { MatchGuid = matchGuid };
            match.Hydrate(matchCreated);
            //When start set for match command executed
            var setGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209446");
            var result = match.HandleCommand(new StartMatchSet { MatchGuid = matchGuid, SetGuid = setGuid });
            Assert.IsTrue(result.First() is MatchSetStarted);
            Assert.AreEqual(((MatchSetStarted)result.First()).MatchGuid, matchGuid);
            Assert.AreEqual(((MatchSetStarted)result.First()).SetGuid, setGuid);
        }

        [Test]
        public void StartGameCommandShouldReturnGameStarted()
        {
            var matchGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209445");
            //given Match Created occured
            var match = new Match();
            var matchCreated = new MatchCreated { MatchGuid = matchGuid };
            match.Hydrate(matchCreated);
            //and Set Started occurred
            var setGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209446");
            var setStarted = new MatchSetStarted { MatchGuid = matchGuid, SetGuid = setGuid};
            match.Hydrate(setStarted);
            //When start game command executed
            var gameGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209447");
            var result = match.HandleCommand(new StartMatchSetGame { MatchGuid = matchGuid, SetGuid = setGuid, GameGuid =  gameGuid});
            Assert.IsTrue(result.First() is MatchSetGameStarted);
            Assert.AreEqual(((MatchSetGameStarted)result.First()).MatchGuid, matchGuid);
            Assert.AreEqual(((MatchSetGameStarted)result.First()).SetGuid, setGuid);
            Assert.AreEqual(((MatchSetGameStarted)result.First()).GameGuid, gameGuid);
        }

        [Test]
        public void ScoreForPlayerOneCommandShouldReturnPlayerOneScoredEvent()
        {
            var matchGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209445");
            //given Match Created occured
            var match = new Match();
            var matchCreated = new MatchCreated { MatchGuid = matchGuid };
            match.Hydrate(matchCreated);
            //and Set Started occurred
            var setGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209446");
            var setStarted = new MatchSetStarted { MatchGuid = matchGuid, SetGuid = setGuid };
            match.Hydrate(setStarted);
            //and game started occurred
            var gameGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209447");
            match.Hydrate(new MatchSetGameStarted {
                MatchGuid = matchGuid,
                SetGuid = setGuid,
                GameGuid = gameGuid });
            //when sore for player one command executed
            var result = match.HandleCommand(new ScorePointForPlayerOne {
                MatchGuid = matchGuid,
                SetGuid = setGuid,
                GameGuid = gameGuid
            });
            //then player one won point event should be returned
            Assert.IsTrue(result.First() is PlayerOneWonPoint);
            Assert.AreEqual(((PlayerOneWonPoint)result.First()).MatchGuid, matchGuid);
            Assert.AreEqual(((PlayerOneWonPoint)result.First()).SetGuid, setGuid);
            Assert.AreEqual(((PlayerOneWonPoint)result.First()).GameGuid, gameGuid);
        }
    }
}
