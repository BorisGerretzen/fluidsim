using System;
using System.Collections.Generic;
using System.Text;

namespace SolverLib
{
    public class Solver {
        public Lattice lattice;
        private double viscosity;

        public Solver(int sizeX, int sizeY, double viscosity) {
            for (int i = 0; i < 9; i++)
            {
                Helpers._directionsCoordinates[i] = Helpers.CoordinatesFromDirectionIndex(i);
            }
            this.lattice = new Lattice(sizeX, sizeY);
            this.viscosity = viscosity;
        }

        public void AddWave(int xMin, int xMax, int yMin, int yMax, int direction, double magnitude) {
            for (int x = xMin; x < xMax; x++) {
                for (int y = yMin; y < yMax; y++) {
                    lattice.grid[x, y]._directions[direction] = magnitude;
                }
            }
        }

        public void Do(int n) {
            for (int i = 0; i < n; i++) {
                // Console.WriteLine(i);
                lattice.Flow();
                lattice.Collide(1 / (3 * viscosity + 0.5));
            }
            // lattice.ToImages();
        }
    }
}
