using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisMatch
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 1; i++)
            {
                Console.WriteLine($"********** MATCH {i+1} *******************");
                TennisMatchSimulation.Run(line => {
                    Console.WriteLine(line);
                });
            }
            Console.ReadLine();
        }
    }
}
