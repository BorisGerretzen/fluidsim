using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SolverLib;

namespace scratch2
{
    class Program
    {
        static void Main(string[] args) {
            var vec = new LatticeVector();
            for (int x = 0; x < 9; x++) {
                vec._directions[x] = 50;
            }
            for (int i = 0; i < 9; i++)
            {
                Helpers._directionsCoordinates[i] = Helpers.CoordinatesFromDirectionIndex(i);
            }
            Solver solver = new Solver(200, 100, 1);
            // solver.lattice.SetVector(50,50, vec);
            solver.AddWave(95, 105, 45, 55, 2, 10);
            //solver.AddWave(0, 10, 2, -5);
            solver.Do(500);
            //print(solver.lattice.grid);
            Environment.Exit(1);
            Console.ReadLine();

            //Setup
            Lattice lattice = new Lattice(5, 5);
            // var vec2 = new LatticeVector();
            // vec2._directions[4] = 5.0;
            // lattice.SetVector(1, 1, vec2);

            print(lattice.grid);
            Console.WriteLine("_________________");

            while (Console.ReadLine() != "\n") {
                // action
                Console.WriteLine("----");
                lattice.Flow();
                print(lattice.grid);


                Console.WriteLine("----");

                lattice.Collide(1.0 / .6);
                print(lattice.grid);


                Console.WriteLine("_________________");
            }

            Console.ReadLine();
        }
        static void dumpall(LatticeVector[,] arr)
        {
            int rowLength = arr.GetLength(0);
            int colLength = arr.GetLength(1);
            for (int x = 0; x < 9; x++) {
                double sum = 0;
                for (int i = 0; i < rowLength; i++) {
                    for (int j = 0; j < colLength; j++) {
                        Console.Write(string.Format("{0} ", new Decimal(arr[i, j]._directions[x]).ToString("0.00")));
                        sum += arr[i, j]._directions[x];
                    }

                    Console.Write(Environment.NewLine);

                }

                Console.WriteLine(sum);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }

        }
        static void print(LatticeVector[,] arr) {
            int rowLength = arr.GetLength(0);
            int colLength = arr.GetLength(1);
            double sum = 0;
            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    Console.Write(string.Format("{0} ", new Decimal(arr[i, j].GetVelocitySum()).ToString("0.00")));
                    sum += arr[i, j].GetVelocitySum();
                }
                Console.Write(Environment.NewLine);

            }
            Console.WriteLine(sum);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
