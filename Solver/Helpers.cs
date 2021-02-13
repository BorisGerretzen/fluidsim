using System;
using System.Collections.Generic;
using System.Text;

namespace SolverLib
{
    public static class Helpers {
        public static (int, int)[] _directionsCoordinates = new (int, int)[9];

        public static void print(LatticeVector[,] arr)
        {
            int rowLength = arr.GetLength(0);
            int colLength = arr.GetLength(1);
            double sum = 0;
            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    Console.Write(String.Format("{0} ", new Decimal(arr[i, j].GetVelocitySum()).ToString("0.00")));
                    sum += arr[i, j].GetVelocitySum();
                }
                Console.Write(Environment.NewLine);

            }
            Console.WriteLine(sum);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// Gets the relative coordinates from a direction index.
        /// </summary>
        /// <param name="direction">Direction index.</param>
        /// <returns>Tuple of (x,y) coordinates</returns>
        public static (int, int) CoordinatesFromDirectionIndex(int direction) {
            int x = 0;
            int y = 0;

            // Set X
            if (direction == 1 || direction == 5 || direction == 8) {
                x = -1;
            }
            else if (direction == 6 || direction == 3 || direction == 7) {
                x = 1;
            }

            // Set Y
            if (direction == 2 || direction == 5 || direction == 6) {
                y = -1;
            }
            else if (direction == 7 || direction == 4 || direction == 8) {
                y = 1;
            }

            return (x, y);
        }

        public static double[] Invert(double[] arr, int n) {
            for (int i = 0; i < n; i++)
            {
                int j;
                double last;
                //Stores the last element of array  
                last = arr[arr.Length - 1];

                for (j = arr.Length - 1; j > 0; j--)
                {
                    //Shift element of array by one  
                    arr[j] = arr[j - 1];
                }
                //Last element of array will be added to the start of array.  
                arr[0] = last;
            }

            return arr;
        }
    }
}