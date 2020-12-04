using System.Drawing;

namespace MandelbrotNetCore
{
    public class Renderer
    {
        private readonly int _pxSize;
        private readonly int _maxIterations;

        private readonly int[][] _iterations;

        public Renderer(int pxSize, int maxIterations = 100)
        {
            _pxSize = pxSize;
            _maxIterations = maxIterations;

            var mandelbrot = new Mandelbrot();
            _iterations = mandelbrot.GenerateParallel(_pxSize, _maxIterations);
        }

        public Bitmap Draw(double hueStep = 36.0, double hueOffset = 0.0)
        {
            var bitmap = new Bitmap(_pxSize, _pxSize);

            //var stopwatch = new Stopwatch();
            //stopwatch.Start();

            for (int i = 0; i < _pxSize; i++)
            {
                for (int j = 0; j < _pxSize; j++)
                {
                    Color color;

                    if (_iterations[i][j] == _maxIterations || _iterations[i][j] == 0)
                    {
                        color = Color.Black;
                    }
                    else
                    {
                        double hue = (hueStep * (_iterations[i][j] - 1) + hueOffset) % 360;
                        ColorConverter.HsvToRgb(hue, 1, 1, out int r, out int g, out int b);
                        color = Color.FromArgb(255, r, g, b);
                    }

                    bitmap.SetPixel(i, j, color);
                }
            }

            //stopwatch.Stop();
            //Console.WriteLine(stopwatch.Elapsed);

            return bitmap;
        }
    }
}
