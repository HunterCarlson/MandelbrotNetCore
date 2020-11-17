using System.Numerics;
using System.Threading.Tasks;

namespace MandelbrotNetCore
{
    public class Mandelbrot
    {
        private const double MAX_R = 2;
        private readonly double minX = -2;
        private readonly double maxX = 2;
        private readonly double minY = -2;
        private readonly double maxY = 2;

        private readonly double _numericWidth;
        private readonly double _numericHeight;

        private int[][] _escapeValues;

        public Mandelbrot()
        {
            _numericWidth = maxX - minX;
            _numericHeight = maxY - minY;
        }

        public int[][] Generate(int pxSize, int maxIterations)
        {
            _escapeValues = new int[pxSize][];

            double deltaX = _numericWidth / pxSize;
            double deltaY = _numericHeight / pxSize;

            for (int i = 0; i < pxSize; i++)
            {
                _escapeValues[i] = new int[pxSize];
                double re = minX + i * deltaX;

                for (int j = 0; j < pxSize; j++)
                {
                    double im = minY + j * deltaY;
                    var c = new Complex(re, im);
                    int iterations = IterateZ(c, maxIterations);
                    _escapeValues[i][j] = iterations;
                }
            }

            return _escapeValues;
        }

        public int[][] GenerateParallel(int pxSize, int maxIterations)
        {
            _escapeValues = new int[pxSize][];

            double deltaX = _numericWidth / pxSize;
            double deltaY = _numericHeight / pxSize;

            Parallel.For(
                0,
                pxSize,
                i =>
                {
                    _escapeValues[i] = new int[pxSize];
                    double re = minX + i * deltaX;

                    for (int j = 0; j < pxSize; j++)
                    {
                        double im = minY + j * deltaY;
                        var c = new Complex(re, im);
                        int iterations = IterateZ(c, maxIterations);
                        _escapeValues[i][j] = iterations;
                    }
                }
            );

            return _escapeValues;
        }

        private static int IterateZ(Complex c, int maxIterations)
        {
            int iterations = 0;
            var z = new Complex(0, 0);

            while (iterations < maxIterations)
            {
                Complex z2 = z.Magnitude + c;
                double r = z2.Magnitude;

                if (r > MAX_R)
                {
                    break;
                }

                z = z2;
                iterations++;
            }

            return iterations;
        }
    }
}
