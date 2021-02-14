using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficientSolver {
    public static class ArrayHelpers {
        public static T[,] Populate<T>(this T[,] arr, T value) {
            int sizeX = arr.GetLength(0);
            int sizeY = arr.GetLength(1);
            for (int x = 0; x < sizeX; x++) {
                for (int y = 0; y < sizeY; y++) {
                    arr[x, y] = value;
                }
            }

            return arr;
        }

        public static double[,] Shift(this double[,] arr, (int, int) steps) {
            int sizeX = arr.GetLength(0);
            int sizeY = arr.GetLength(1);
            int shiftX = -steps.Item1;
            int shiftY = -steps.Item2;

            var newArr = (double[,]) arr.Clone();

            // Shift col
            if (shiftY != 0) {
                for (int x = 0; x < sizeX; x++) {
                    for (int y = 0; y < sizeY; y++) {
                        if (shiftY > 0) {
                            newArr[x, y] = arr[x, (y + shiftY) % sizeY];
                        }
                        else {
                            newArr[x, y] = arr[x, (sizeY + (y + shiftY)) % sizeY];
                        }
                    }
                }

                Array.Copy(newArr, arr, sizeX * sizeY);
            }

            // Shift rows
            if (shiftX != 0) {
                if (shiftX > 0) {
                    // Copy last n-shift rows
                    Array.Copy(arr, sizeX * shiftX, newArr, 0, sizeX * (sizeY - shiftX));

                    // Copy last rows
                    Array.Copy(arr, 0, newArr, sizeX * (sizeY - shiftX), sizeX * shiftX);
                }
                else {
                    shiftX *= -1;
                    // Copy first n-shift rows
                    Array.Copy(arr, 0, newArr, sizeX*shiftX, sizeX * (sizeY-shiftX));
                    // Copy last rows
                    Array.Copy(arr, sizeX * (sizeY - shiftX), newArr, 0, sizeX*shiftX);
                }
            }

            return newArr;
        }

        public static void Dump(this double[,] arr) {
            int rowLength = arr.GetLength(0);
            int colLength = arr.GetLength(1);
            for (int i = 0; i < colLength; i++) {
                for (int j = 0; j < rowLength; j++) {
                    Console.Write($"{arr[j, i]:00.00} ");
                }

                Console.Write(Environment.NewLine);
            }

            Console.WriteLine();
        }

        public static void DumpAll(this double[][,] arr) {
            for (int i = 0; i < arr.Length; i++) {
                arr[i].Dump();
            }
        }
    }
}