using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace EfficientSolver {
    public class Solver {
        public readonly double[][,] _grid;
        public readonly List<double[][,]> oldGrids = new List<double[][,]>();
        public readonly int sizeX;
        public readonly int sizeY;
        public readonly double viscosity;

        private static readonly (int, int)[] DirectionVectors = {
            (0, 0), // Here
            (1,0), //Right
            (0, -1), // Up
            (-1, 0), // Left
            (0, 1), // Down
            (1, -1), // NorthEast
            (-1, -1), // NorthWest
            (-1, 1), // SouthWest
            (1, 1) // SouthEast
        };

        private readonly double[] _weights = {
            4.0 / 9, // center
            1.0 / 9, // right
            1.0 / 9, // up
            1.0 / 9, //left
            1.0 / 9, //bottom
            1.0 / 36, // Right Up
            1.0 / 36, // Left Up
            1.0 / 36, // Left Down
            1.0 / 36, // Right down
        };

        public Solver(int sizeX, int sizeY, double viscosity) {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.viscosity = viscosity;

            _grid = new double[DirectionVectors.Length][,];

            // init all direction arrays and populate them
            for (int direction = 0; direction < DirectionVectors.Length; direction++) {
                _grid[direction] = new double[sizeX, sizeY].Populate(1.0);
            }

            // _grid[1][2, 2] = 5;
        }

        public void AddWave(int xMin, int xMax, int yMin, int yMax, int direction, double magnitude) {
            for (int x = xMin; x < xMax; x++) {
                for (int y = yMin; y < yMax; y++) {
                    _grid[direction][x, y] = magnitude;
                }
            }
        }

        public void Do(int n) {
            for (int i = 0; i < n; i++) {
                Console.WriteLine($"Generating frame {i}");
                Flow();
                oldGrids.Add((double[][,])_grid.Clone());
                Collide(1 / (3 * viscosity + 0.5));
            }

            MakeDaImages();
        }

        public void Flow() {
            for (int direction = 0; direction < DirectionVectors.Length; direction++) {
                _grid[direction] = _grid[direction].Shift(DirectionVectors[direction]);
            }
        }

        public double[,] Density() {
            return Density(_grid);
        }

        public double[,] Density(double[][,] inputGrid) {
            double[,] average = new double[sizeX, sizeY];
            for (int direction = 0; direction < DirectionVectors.Length; direction++) {
                for (int x = 0; x < sizeX; x++) {
                    for (int y = 0; y < sizeY; y++) {
                        average[x, y] += inputGrid[direction][x, y];
                    }
                }
            }

            return average;
        }

        private (double[,], double[,]) GetMomentum(double[,] velocitySum) {
            double[,] momentumX = new double[sizeX, sizeY];
            double[,] momentumY = new double[sizeX, sizeY];

            if (velocitySum[0, 0] > 0) {
                for (int x = 0; x < sizeX; x++) {
                    for (int y = 0; y < sizeY; y++) {
                        momentumX[x, y] =
                            Enumerable.Range(0, 9).Sum(i => _grid[i][x, y] * DirectionVectors[i].Item1) /
                            velocitySum[x, y];
                        momentumY[x, y] =
                            Enumerable.Range(0, 9).Sum(i => _grid[i][x, y] * DirectionVectors[i].Item2) /
                            velocitySum[x, y];
                    }
                }
            }

            return (momentumX, momentumY);
        }

        public void Collide(double relaxationTime) {
            double[,] density = Density();
            double[][,] Feqs = new double[DirectionVectors.Length][,];
            for (int i = 0; i < Feqs.Length; i++) {
                Feqs[i] = new double[sizeX, sizeY].Populate(1);
            }

            var momentum = GetMomentum(density);
            for (int direction = 0; direction < DirectionVectors.Length; direction++) {
                for (int x = 0; x < sizeX; x++) {
                    for (int y = 0; y < sizeY; y++) {
                        double v = _grid[direction][x, y];
                        double dirX = DirectionVectors[direction].Item1;
                        double dirY = DirectionVectors[direction].Item2;
                        double Feq = _weights[direction] * density[x, y] *
                                     (1 + 3 * (dirX * momentum.Item1[x, y] + dirY * momentum.Item2[x, y]) +
                                      4.5 * Math.Pow(dirX * momentum.Item1[x, y] + dirY * momentum.Item2[x, y], 2) -
                                      1.5 * (Math.Pow(momentum.Item1[x, y], 2) + Math.Pow(momentum.Item2[x, y], 2)));
                        Feqs[direction][x, y] = Feq;
                        _grid[direction][x, y] += -relaxationTime * (v - Feq);
                    }
                }
            }

            oldGrids.Add((double[][,]) _grid.Clone());
        }

        public void MakeDaImages() {
            double max = Double.MinValue;
            double min = Double.MaxValue;

            for (int i = 0; i < oldGrids.Count; i++) {
                var density = Density(oldGrids[i]);

                for (int x = 0; x < sizeX; x++) {
                    for (int y = 0; y < sizeY; y++) {
                        var sum = density[x, y];
                        if (sum > max) {
                            max = sum;
                        }
                        else if (sum < min) {
                            min = sum;
                        }
                    }
                }
            }

            if (Math.Abs(max - min) < 0.01) {
                max = min + 1;
            }

            Console.WriteLine($"Bounds [{min}, {max}]");
            for (int i = 0; i < oldGrids.Count; i++) {
                Console.WriteLine($"Creating image {i}");
                gridToTBitmap(oldGrids[i], min, max).Save($"img/{i}.bmp");
            }
        }

        private Bitmap gridToTBitmap(double[][,] grid, double min, double max) {
            Bitmap bmp = new Bitmap(sizeX, sizeY);
            var density = Density(grid);
            for (int x = 0; x < sizeX; x++) {
                for (int y = 0; y < sizeY; y++) {
                    var v = density[x, y];
                    var c = MapRainbowColor(v, max, min);
                    bmp.SetPixel(x, y, c);
                }
            }

            return bmp;
        }

        private static Color MapRainbowColor(
            double value, double red_value, double blue_value) {
            // Convert into a value between 0 and 1023.
            int int_value = (int) (1023 * (value - red_value) /
                                   (blue_value - red_value));

            // Map different color bands.
            if (int_value < 256) {
                // Red to yellow. (255, 0, 0) to (255, 255, 0).
                return Color.FromArgb(255, int_value, 0);
            }
            else if (int_value < 512) {
                // Yellow to green. (255, 255, 0) to (0, 255, 0).
                int_value -= 256;
                return Color.FromArgb(255 - int_value, 255, 0);
            }
            else if (int_value < 768) {
                // Green to aqua. (0, 255, 0) to (0, 255, 255).
                int_value -= 512;
                return Color.FromArgb(0, 255, int_value);
            }
            else {
                // Aqua to blue. (0, 255, 255) to (0, 0, 255).
                int_value -= 768;
                return Color.FromArgb(0, 255 - int_value, 255);
            }
        }
    }
}