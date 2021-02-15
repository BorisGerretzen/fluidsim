using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
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
            EfficientSolver.Solver solver = new EfficientSolver.Solver(1920,1080, 1.0);
            solver.AddWave(910, 1010, 490, 590, 1, 10);
            // solver._grid[3][2, 2] = 5;

            if (doManual) {
                manual(solver);
            }
            else {
                // benchmark();
                solver.Save = true;
                solver.Do(2);
            }
            Console.ReadLine();
        }

        static void benchmark() {
            EfficientSolver.Solver solver = new EfficientSolver.Solver(200, 100, 1.0);
            Solver solver2 = new Solver(200, 100, 1.0);
            solver2.AddWave(95, 106, 45, 55, 2, 10);
            solver.AddWave(95, 105, 45, 55, 1, 10);
            var watch = Stopwatch.StartNew();
            long totalEfficient = 0;
            long totalOld = 0;
            for (int i = 0; i < 10; i++)
            {
                solver.Save = false;
                watch.Start();
                solver.Do(200);
                watch.Stop();
                Console.WriteLine(watch.ElapsedMilliseconds);
                totalEfficient += watch.ElapsedMilliseconds;
                watch.Reset();

                watch.Start();
                solver2.Do(200);
                watch.Stop();
                Console.WriteLine(watch.ElapsedMilliseconds);
                totalOld += watch.ElapsedMilliseconds;
                watch.Reset();
            }

            Console.WriteLine("_____");
            Console.WriteLine($"Efficient:  {totalEfficient}");
            Console.WriteLine($"Old:        {totalEfficient}");
        }
    }
}
