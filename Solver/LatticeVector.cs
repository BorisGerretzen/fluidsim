using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SolverLib {
    public class LatticeVector {
        public double[] _directions = new double[] {1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0};

        private readonly double[] weights = new double[] {
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

        public double GetVelocitySum() {
            return _directions.Sum();
        }

        public (double, double) GetMomentum() {
            return GetMomentum(GetVelocitySum());
        }

        private (double, double) GetMomentum(double velocitySum) {
            if (velocitySum == 0) {
                return (0, 0);
            }

            double momentumX = Enumerable.Range(0, 9).Sum(i => _directions[i] * Helpers._directionsCoordinates[i].Item1) / velocitySum;
            double momentumY = Enumerable.Range(0, 9).Sum(i => _directions[i] * Helpers._directionsCoordinates[i].Item2) / velocitySum;
            return (momentumX, momentumY);
        }

        public void Collide(double relaxationTime) {
            double density = GetVelocitySum();
            var momentum = GetMomentum(density);
            for (int i = 0; i < _directions.Length; i++) {
                double v = _directions[i];
                double dirX = Helpers._directionsCoordinates[i].Item1;
                double dirY = Helpers._directionsCoordinates[i].Item2;
                double Feq = weights[i] * density * (1 + 3 * (dirX * momentum.Item1 + dirY * momentum.Item2) + 4.5 * Math.Pow(dirX * momentum.Item1 + dirY * momentum.Item2, 2) -
                                                     1.5 * (Math.Pow(momentum.Item1, 2) + Math.Pow(momentum.Item2, 2)));

                _directions[i] += -relaxationTime * (v-Feq);
            }
        }
    }
}