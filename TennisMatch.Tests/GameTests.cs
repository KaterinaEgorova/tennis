using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisMatch.Domain;

namespace TennisMatch.Tests
{
    [TestFixture]
    public class GameTests
    {
        [Test]
        public void WhenGameStartedIsCOmpleteShouldBeFalse()
        {
            var game = new Game();
            Assert.IsFalse(game.IsComplete());
        }

        [Test]
        public void WhenFirstPlayerScoredFourTimesInARowGameShouldBeComplete()
        {
            var game = new Game();
            for (int i = 0; i < 4; i++)
            {
                game.ScoreP1();
            }
            Assert.IsTrue(game.IsComplete());
        }

        [Test]
        public void WhenSecondPlayerScoredFourTimesInARowGameShouldBeComplete()
        {
            var game = new Game();
            for (int i = 0; i < 4; i++)
            {
                game.ScoreP2();
            }
            Assert.IsTrue(game.IsComplete());
        }

        [Test]
        public void WhenDifferenceInPointsLessThanTwoGameShouldNotBeComplete()
        {
            var game = new Game();
            game.ScoreP1();
            game.ScoreP1();
            game.ScoreP1();
            game.ScoreP2();
            game.ScoreP2();
            game.ScoreP2();
            game.ScoreP1();
            Assert.IsFalse(game.IsComplete());
        }

        [Test]
        public void WhenTryingToScoreForCompleteGameShouldThrowException()
        {
            var game = new Game();
            game.ScoreP1();
            game.ScoreP1();
            game.ScoreP1();
            game.ScoreP1();
            Assert.Throws<ApplicationException>(() => game.ScoreP2(), "Cannot score. Game is complete.");
        }

        [Test]
        public void WhenFirstAndSecondPlayersScoredShouldReturnCorrectScoresAsAString()
        {
            var game = new Game();
            game.ScoreP1();
            game.ScoreP2();
            Assert.AreEqual("P1: 15 - P2: 15", game.ToString());
        }
    }
}
