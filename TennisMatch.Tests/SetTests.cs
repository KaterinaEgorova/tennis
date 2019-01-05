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
    public class SetTests
    {
        [Test]
        public void WhenSetJustStaretdItShouldNotBeCompletedYet()
        {
            var set = new Set();
            Assert.IsFalse(set.IsComplete());
        }

        [Test]
        public void WhenSetJustStaretdItShouldHaveUnknownWinner()
        {
            var set = new Set();
            Assert.AreEqual(Winners.Unknown, set.GetWinner());
        }

        [Test]
        public void WhenSetHasThreGamesPlayed()
        {
            var set = new Set();
            Assert.AreEqual(Winners.Unknown, set.GetWinner());
        }

        [Test]
        public void When12GamesPlayedWithSixSixScoreThenShouldBeTieBreak()
        {
            var set = new Set();
            for (int i = 0; i < 6; i++)
            {
                var game = set.AddGame();
                for (int j = 0; j < 4; j++)
                {
                    game.ScoreP1();
                }
                var game1 = set.AddGame();
                for (int j = 0; j < 4; j++)
                {
                    game1.ScoreP2();
                }
            }
            Assert.IsTrue(set.IsTiebreak());
        }

        [Test]
        public void WhenGameIsNotCompleteAndTryingToStartAnotherGameShouldThrow()
        {
            var set = new Set();
            var game = set.AddGame();
            game.ScoreP1();

            Assert.Throws<ApplicationException>(()=>set.AddGame(), "Cannot start a game while another game is in progress.");
            
        }

        [Test]
        public void WhenTryToAddGameToCompletedSetShouldThrow()
        {
            var set = new Set();
            for (int i = 0; i < 7; i++)
            {
                var game = set.AddGame();
                for (int j = 0; j < 4; j++)
                {
                    game.ScoreP1();
                }
            }
            Assert.Throws<ApplicationException>(() => set.AddGame(), "Cannot add a game. Set is complete.");
        }
    }
}
