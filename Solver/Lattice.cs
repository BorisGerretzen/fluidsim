using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;

namespace SolverLib {
    public class Lattice {
        /// <summary>
        /// The coordinate system that golds the LatticeVectors.
        /// </summary>
        public LatticeVector[,] grid;


        List<LatticeVector[,]> OldGrids = new List<LatticeVector[,]>();

        /// <summary>
        /// Creates a new lattice of specified size.
        /// </summary>
        /// <param name="sizeX">X size of the lattice</param>
        /// <param name="sizeY">Y size of the lattice</param>
        public Lattice(int sizeX, int sizeY) {
            grid = new LatticeVector[sizeX, sizeY];
            Random rnd = new Random();
            for (int i = 0; i < sizeX; i++) {
                for (int j = 0; j < sizeY; j++) {
                    grid[i, j] = new LatticeVector();
                }
            }
        }

        /// <summary>
        /// Gets the LatticeVector at specified coordinates.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns></returns>
        public LatticeVector GetVector(int x, int y) {
            return grid[x, y];
        }

        /// <summary>
        /// Sets the LatticeVector at specified coordinates.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="vector">Target vector</param>
        public void SetVector(int x, int y, LatticeVector vector) {
            grid[x, y] = vector;
        }

        // for (int y = 0; y<grid.GetLength(0)-1; y++) {
        //     for (int x = 0; x<grid.GetLength(1)-1; x++) {
        public void Flow() {
            LatticeVector[,] newGrid = (LatticeVector[,]) grid.Clone();
            for (int x = 0; x < grid.GetLength(0); x++) {
                for (int y = 0; y < grid.GetLength(1); y++) {
                    HashSet<int> skipX = new HashSet<int>();
                    HashSet<int> skipY = new HashSet<int>();
                    if (x == grid.GetLength(0) - 1) {
                        skipX.Add(1);
                        skipX.Add(5);
                        skipX.Add(8);
                    }

                    if (y == 0) {
                        skipY.Add(4);
                        skipY.Add(7);
                        skipY.Add(8);
                    }

                    if (x == 0) {
                        skipX.Add(3);
                        skipX.Add(6);
                        skipX.Add(7);
                    }

                    if (y == grid.GetLength(1) - 1) {
                        skipY.Add(2);
                        skipY.Add(6);
                        skipY.Add(5);
                    }

                    newGrid[x, y] = new LatticeVector();
                    for (int direction = 0; direction < 9; direction++) {
                        int cx = x - Helpers._directionsCoordinates[direction].Item1;
                        int cy = y - Helpers._directionsCoordinates[direction].Item2;

                        if (skipX.Contains(direction)) {
                            cx = x == 0 ? grid.GetLength(0) - 1 : 0;
                        }

                        if (skipY.Contains(direction)) {
                            cy = y == 0 ? grid.GetLength(1) - 1 : 0;
                        }

                        newGrid[x, y]._directions[direction] = grid[cx, cy]._directions[direction];
                    }
                }
            }
            OldGrids.Add(grid);
            grid = newGrid;
        }

        public void Collide(double relaxationTime) {
            for (int x = 0; x < grid.GetLength(0); x++) {
                for (int y = 0; y < grid.GetLength(1); y++) {
                    grid[x, y].Collide(relaxationTime);
                }
            }
        }

        public void ToImages() {
            double max = Double.MinValue;
            double min = Double.MaxValue;

            for (int i = 0; i < OldGrids.Count; i++) {
                var grid = OldGrids[i];

                for (int x = 0; x < grid.GetLength(0); x++) {
                    for (int y = 0; y < grid.GetLength(1); y++) {
                        var vec = grid[x, y];
                        var sum = vec.GetVelocitySum();
                        if (sum > max) {
                            max = sum;
                        }
                        else if (sum < min) {
                            min = sum;
                        }
                    }
                }
            }

            if (max == min) {
                max = min + 1;
            }

            for (int i = 0; i < OldGrids.Count; i++) {
                Console.WriteLine($"Creating image {i}");
                gridToTBitmap(OldGrids[i], min, max).Save($"img/{i}.bmp");
            }
        }

        private static Bitmap gridToTBitmap(LatticeVector[,] grid, double min, double max) {
            Bitmap bmp = new Bitmap(grid.GetLength(0), grid.GetLength(1));
            for (int x = 0; x < grid.GetLength(0); x++) {
                for (int y = 0; y < grid.GetLength(1); y++) {
                    var v = grid[x, y].GetVelocitySum();
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

        // public void ToImage(int i) {
        //     OldGrids.Add(grid);
        //
        //     double max = Double.MinValue;
        //     double min = Double.MaxValue;
        //     for (int x = 0; x < grid.GetLength(0); x++) {
        //         for (int y = 0; y < grid.GetLength(1); y++) {
        //             var vec = grid[x, y];
        //             var sum = vec.GetVelocitySum();
        //             if (sum > max) {
        //                 max = sum;
        //             }
        //             else if (sum < min) {
        //                 min = sum;
        //             }
        //         }
        //     }
        //
        //     if (max == min) {
        //         max = min + 1;
        //     }
        //
        //     var ratio = 50 / 255.0;
        //     var tmin = 0;
        //     var tmax = 255;
        //     Bitmap sample = new Bitmap(grid.GetLength(0), grid.GetLength(1));
        //
        //     for (int x = 0; x < grid.GetLength(0); x++) {
        //         for (int y = 0; y < grid.GetLength(1); y++) {
        //             var v = grid[x, y].GetMomentum();
        //             var v_mapped = (255 / (max - min)) * (v - min);
        //
        //             var c = Color.FromArgb((int) (Math.Round(v_mapped)), 0, 0);
        //             sample.SetPixel(x, y, c);
        //         }
        //     }
        //
        //     Random rnd = new Random();
        //     sample.Save($"img/{i}.bmp");
        // }
    }
}