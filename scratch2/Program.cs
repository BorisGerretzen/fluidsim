using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EfficientSolver;
using SolverLib;
using Solver = SolverLib.Solver;

namespace scratch2
{
    class Program
    {
        static void manual(EfficientSolver.Solver solver)
        {
            solver.Density().Dump();
            while (Console.ReadLine() == "")
            {
                Console.WriteLine("__________");
                solver.Flow();
                solver.Collide(1/0.6);
                // solver.oldGrids.Add(solver._grid);
                solver.Density().Dump();
            }
            solver.MakeDaImages();

        }

        private static bool doManual = false;

        static void Main(string[] args) {
            EfficientSolver.Solver solver = new EfficientSolver.Solver(200,100, 1.0);
            solver.AddWave(95, 105, 45, 55, 1, 10);
            // solver._grid[3][2, 2] = 5;

            if (doManual) {
                manual(solver);
            } else
            {
                solver.Do(100);
            }
            Console.ReadLine();
        }
    }
}
