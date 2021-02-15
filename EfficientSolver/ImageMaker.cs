using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficientSolver {
    public class ImageMaker {
        private int sizeX;
        private int sizeY;

        private double max = Double.MinValue;
        private double min = Double.MaxValue;

        public List<string> files = new List<string>();

        public ImageMaker(int sizeX, int sizeY) {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
        }


        public void StoreGrid(double[,] grid) {
            StringBuilder stringBuilder = new StringBuilder();
            for (int x = 0; x < sizeX; x++) {
                for (int y = 0; y < sizeY; y++) {
                    stringBuilder.Append(grid[x, y]);
                    stringBuilder.Append('-');

                    if (grid[x, y] > max) {
                        max = grid[x, y];
                    }
                    else if (grid[x, y] < min) {
                        min = grid[x, y];
                    }
                }
            }

            string fname = "grid/" + stringBuilder.ToString().GetHashCode() + ".grid";
            files.Add(fname);
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            if (!File.Exists(fname)) {
                File.WriteAllText(fname, stringBuilder.ToString());
            }
        }

        public double[] gridFromFile(string filename) {
            string[] data = File.ReadAllText(filename).Split('-');
            double[] grid = new double[data.Length];

            for (int i = 0; i < data.Length; i++) {
                grid[i] = Double.Parse(data[i]);
            }

            return grid;
        }

        public void MakeImages() {
            for (int i = 0; i < files.Count; i++) {
                Console.WriteLine($"Generating image {i} from {files[i]}");
                gridToTBitmap(gridFromFile(files[i])).Save($"img/{i}.bmp", ImageFormat.Bmp);
            }
        }


        private Bitmap gridToTBitmap(double[] grid) {
            Bitmap bmp = new Bitmap(sizeX, sizeY);
            double[,] grid2d = grid.Make2DArray(sizeY, sizeX);
            for (int x = 0; x < sizeX; x++) {
                for (int y = 0; y < sizeY; y++) {
                    var v = grid2d[x, y];
                    var c = MapRainbowColor(v);
                    bmp.SetPixel(x, y, c);
                }
            }

            return bmp;
        }

        private Color MapRainbowColor(double value) {
            // Convert into a value between 0 and 1023.
            int int_value = (int) (1023 * (value - max) /
                                   (min - max));

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