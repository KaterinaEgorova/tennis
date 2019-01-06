using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisMatch.Domain;

namespace TennisMatch
{
    public class TennisMatchSimulation
    {
        private static Random rnd = new Random();
        public static void Run(Action<string> log)
        {
            var match = new Match();
            while (!match.IsComplete())
            {
                var set = match.AddSet();
                log?.Invoke("Set started.");
                while (!set.IsComplete())
                {
                    if (set.IsTiebreak())
                    {
                        // play tiebreak
                        if (rnd.Next(2) == 0)
                            set.ScoreTiebreakP1();
                        else
                            set.ScoreTiebreakP2();
                         log?.Invoke($"Tiebreak: {set.ToString()}");
                    }
                    else {
                        // else start and play new game
                        var game = set.AddGame();
                         log?.Invoke("Game started.");
                        while (!game.IsComplete())
                        {
                            if (rnd.Next(2) == 0)
                                game.ScoreP1();
                            else
                                game.ScoreP2();
                             log?.Invoke(game.ToString());
                        }
                         log?.Invoke($"Game completed. Winner is {game.GetWinner()}.");
                    }
                }
                 log?.Invoke($"Set completed. Winner is {set.GetWinner()}. Score: {set.ToString()}");
            }
             log?.Invoke($"Match completed. Match results: {match.ToString()}");
        }
    }
}
