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

        [Test]
        public void WhenPlayerTwoWonVeryFirstPontThenThreEventsShouldBePublished()
        {
            var matchGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209445");
            //given Match
            var match = new Match();
            //when Create match command is executed
            var matchCreated = new MatchCreated { MatchGuid = matchGuid };
            // then Match created event has to be published
            match.Hydrate(matchCreated);
            // and player 1 won first point
            var result = match.HandleCommand(new ScorePointForPlayerTwo
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
            Assert.IsTrue(result[2] is PlayerTwoWonPoint);
            Assert.AreEqual(((PlayerTwoWonPoint)result[2]).MatchGuid, matchGuid);
        }

        [Test]
        public void WhenPlayerOneWon6PointsInARowThenGameShouldBeComplete()
        {
            var matchGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209445");
            //given Match
            var match = new Match();
            //when match created
            var matchCreated = new MatchCreated { MatchGuid = matchGuid };
            match.Hydrate(matchCreated);
            // and set started
            var setGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209446");
            match.Hydrate(new MatchSetStarted
            {
                MatchGuid = matchGuid,
                SetGuid = setGuid,
            });
            // and game started
            var gameGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209447");
            match.Hydrate(new MatchSetGameStarted {
                MatchGuid = matchGuid,
                SetGuid = setGuid,
                GameGuid = gameGuid
            });
            // and player 1 won first 5 point
            for (int i = 0; i < 5; i++)
            {
                match.Hydrate(new PlayerOneWonPoint
                {
                    MatchGuid = matchGuid,
                    SetGuid = setGuid,
                    GameGuid = gameGuid,
                });
            }
            // when player one score sixth point 
            var result = match.HandleCommand(new ScorePointForPlayerOne
            {
                MatchGuid = matchGuid,
            });
            // then two events should be published
            Assert.AreEqual(2, result.Count);
            //first event player one won point
            Assert.IsTrue(result[0] is PlayerOneWonPoint);
            //second event game completed
            Assert.IsTrue(result[1] is GameCompleted);
            Assert.AreEqual(6, ((GameCompleted)result[1]).PlayerOnePoints);
        }

        [Test]
        public void WhenPlayerTwoWon6PointsInARowThenGameShouldBeComplete()
        {
            var matchGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209445");
            //given Match
            var match = new Match();
            //when match created
            var matchCreated = new MatchCreated { MatchGuid = matchGuid };
            match.Hydrate(matchCreated);
            // and set started
            var setGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209446");
            match.Hydrate(new MatchSetStarted
            {
                MatchGuid = matchGuid,
                SetGuid = setGuid,
            });
            // and game started
            var gameGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209447");
            match.Hydrate(new MatchSetGameStarted
            {
                MatchGuid = matchGuid,
                SetGuid = setGuid,
                GameGuid = gameGuid
            });
            // and player 1 won first 5 point
            for (int i = 0; i < 5; i++)
            {
                match.Hydrate(new PlayerTwoWonPoint
                {
                    MatchGuid = matchGuid,
                    SetGuid = setGuid,
                    GameGuid = gameGuid,
                });
            }
            // when player one score sixth point 
            var result = match.HandleCommand(new ScorePointForPlayerTwo
            {
                MatchGuid = matchGuid,
            });
            // then two events should be published
            Assert.AreEqual(2, result.Count);
            //first event player one won point
            Assert.IsTrue(result[0] is PlayerTwoWonPoint);
            //second event game completed
            Assert.IsTrue(result[1] is GameCompleted);
            Assert.AreEqual(6, ((GameCompleted)result[1]).PlayerTwoPoints);
        }

        [Test]
        public void WhenEachPlayerWon6GamesThenTiebreakEventShouldBePublished()
        {
            var matchGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209445");
            //given Match
            var match = new Match();
            //when match created
            var matchCreated = new MatchCreated { MatchGuid = matchGuid };
            match.Hydrate(matchCreated);
            // and set started
            var setGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209446");
            match.Hydrate(new MatchSetStarted
            {
                MatchGuid = matchGuid,
                SetGuid = setGuid,
            });
            CreateTieBreackScenario(matchGuid, match, setGuid);

            //when player one won a point
            var result = match.HandleCommand(new ScorePointForPlayerOne
            {
                MatchGuid = matchGuid
            });
            //then tie break point won event should be published
            Assert.IsTrue(result[0] is TiebreakPointWonByPlayerOne);
        }

        [Test]
        public void WhenTwoTiebreakPointsWonInARowByPlayerOneThenSetShouldBeCompleed()
        {
            var matchGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209445");
            //given Match
            var match = new Match();
            //when match created
            var matchCreated = new MatchCreated { MatchGuid = matchGuid };
            match.Hydrate(matchCreated);
            // and set started
            var setGuid = new Guid("779A16C7-E379-4BA1-8680-4BD0BC209446");
            match.Hydrate(new MatchSetStarted
            {
                MatchGuid = matchGuid,
                SetGuid = setGuid,
            });
            CreateTieBreackScenario(matchGuid, match, setGuid);
            for (int i = 0; i < 6; i++)
            {
                match.Hydrate(new TiebreakPointWonByPlayerOne
                {
                    MatchGuid = matchGuid,
                    SetGuid = setGuid,
                });
            }
           
            var result = match.HandleCommand(new ScorePointForPlayerOne
            {
                MatchGuid = matchGuid,
                SetGuid = setGuid,
            });
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result[0] is TiebreakPointWonByPlayerOne);
            Assert.IsTrue(result[1] is SetCompleted);
        }

        private static void CreateTieBreackScenario(Guid matchGuid, Match match, Guid setGuid)
        {
            Guid gameGuid;
            for (int j = 0; j < 6; j++) //each player won 6 games
            {
                gameGuid = Guid.NewGuid();
                match.Hydrate(new MatchSetGameStarted
                {
                    MatchGuid = matchGuid,
                    SetGuid = setGuid,
                    GameGuid = gameGuid
                });
                for (int i = 0; i < 6; i++)
                {
                    match.Hydrate(new PlayerOneWonPoint
                    {
                        MatchGuid = matchGuid,
                        SetGuid = setGuid,
                        GameGuid = gameGuid,
                    });
                }
                match.Hydrate(new GameCompleted
                {
                    MatchGuid = matchGuid,
                    SetGuid = setGuid,
                    GameGuid = gameGuid,
                    PlayerOnePoints = 6,
                    PlayerTwoPoints = 0
                });

                gameGuid = Guid.NewGuid();

                match.Hydrate(new MatchSetGameStarted
                {
                    MatchGuid = matchGuid,
                    SetGuid = setGuid,
                    GameGuid = gameGuid
                });

                for (int i = 0; i < 6; i++)
                {
                    match.Hydrate(new PlayerTwoWonPoint
                    {
                        MatchGuid = matchGuid,
                        SetGuid = setGuid,
                        GameGuid = gameGuid,
                    });
                }
                match.Hydrate(new GameCompleted
                {
                    MatchGuid = matchGuid,
                    SetGuid = setGuid,
                    GameGuid = gameGuid,
                    PlayerOnePoints = 0,
                    PlayerTwoPoints = 6
                });
            }
        }
    }
}
