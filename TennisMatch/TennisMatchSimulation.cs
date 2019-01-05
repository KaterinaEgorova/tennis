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
        public static Match Run()
        {
            var rnd = new Random();
            var match = new Match();
            while (!match.IsComplete())
            {
                var set = match.AddSet();
                Console.WriteLine("Set started.");
                while (!set.IsComplete())
                {
                    if (set.IsTiebreak())
                    {
                        // play tiebreak
                        if (rnd.Next(2) == 0)
                            set.ScoreTiebreakP1();
                        else
                            set.ScoreTiebreakP2();
                    }
                    else {
                        // else start and play new game
                        var game = set.AddGame();
                        Console.WriteLine("Game started.");
                        while (!game.IsComplete())
                        {
                            if (rnd.Next(2) == 0)
                                game.ScoreP1();
                            else
                                game.ScoreP2();
                            Console.WriteLine(game.ToString());
                        }
                        Console.WriteLine("Game completed.");
                    }
                }
                Console.WriteLine("Set completed. Score: ");
                Console.WriteLine(set.ToString());
            }
            Console.WriteLine("Match completed. Match results:");
            return match;
        }
    }
}
