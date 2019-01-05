using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisMatch.Domain
{
    public class Match
    {
        private List<Set> sets = new List<Set>();

        public Set AddSet()
        {
            if (IsComplete())
                throw new ApplicationException("Cannot start set. Match is complete.");
            if (sets.Any(x=>!x.IsComplete()))
                throw new ApplicationException("Cannot start a set while another set is in progress.");

            var set = new Set();
            sets.Add(set);
            return set;
        }

        public bool IsComplete()
        {
            var setScore = GetCurrentSetScore();
            return (setScore.Item1 >= 2) || (setScore.Item2 >= 2);
        }

        public override string ToString()
        {
            var result = string.Empty;
            foreach (var s in sets)
            {
                if (result != string.Empty)
                    result += ";";
                result += s.ToString();
            }

            return result;
        }

        private Tuple<int, int> GetCurrentSetScore()
        {
            int scoreP1 = 0;
            int scoreP2 = 0;
            foreach (var s in sets)
            {
                var winner = s.GetWinner();
                if (winner == Winners.P1)
                    scoreP1++;
                else if (winner == Winners.P2)
                    scoreP2++;
            }

            return new Tuple<int, int>(scoreP1, scoreP2);
        }
    }
}
