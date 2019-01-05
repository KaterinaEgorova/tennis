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
    public class MatchTests
    {
        [Test]
        public void WhenThreeSetsArePlayedThenMatchShouldBeComplete()
        {
            var match = new Match();
            var set = match.AddSet();
            for (int i = 0; i < 7; i++)
            {
                var game = set.AddGame();
                for (int j = 0; j < 4; j++)
                {
                    game.ScoreP1();
                }
            }
            set = match.AddSet();
            for (int i = 0; i < 7; i++)
            {
                var game = set.AddGame();
                for (int j = 0; j < 4; j++)
                {
                    game.ScoreP2();
                }
            }
            set = match.AddSet();
            for (int i = 0; i < 7; i++)
            {
                var game = set.AddGame();
                for (int j = 0; j < 4; j++)
                {
                    game.ScoreP2();
                }
            }
            Assert.IsTrue(match.IsComplete());
        }

        [Test]
        public void WhenAddingSetToCompletedMatchShouldThrow()
        {
            var match = new Match();
            var set = match.AddSet();
            for (int i = 0; i < 7; i++)
            {
                var game = set.AddGame();
                for (int j = 0; j < 4; j++)
                {
                    game.ScoreP1();
                }
            }
            set = match.AddSet();
            for (int i = 0; i < 7; i++)
            {
                var game = set.AddGame();
                for (int j = 0; j < 4; j++)
                {
                    game.ScoreP2();
                }
            }
            set = match.AddSet();
            for (int i = 0; i < 7; i++)
            {
                var game = set.AddGame();
                for (int j = 0; j < 4; j++)
                {
                    game.ScoreP2();
                }
            }
            Assert.Throws<ApplicationException>(() => match.AddSet(), "Cannot start set. Match is complete.");
        }

        [Test]
        public void WhenStartingSetWhenAnotherSetIsNotCompletedYet()
        {
            var match = new Match();
            var set = match.AddSet();
            Assert.Throws<ApplicationException>(() => match.AddSet(), "Cannot start a set while another set is in progress.");
        }
    }
}
