using System;
using System.Collections.Generic;
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

        /// <summary>
        ///     Escape time alg with Complex type
        /// </summary>
        /// <param name="pxSize"></param>
        /// <param name="maxIterations"></param>
        /// <returns></returns>
        public int[][] GenerateParallelWithComplex(int pxSize, int maxIterations)
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

        /// <summary>
        ///     Unoptimized naive escape time algorithm
        ///     Does not use Complex type
        /// </summary>
        /// <param name="pxSize"></param>
        /// <param name="maxIterations"></param>
        /// <returns></returns>
        public int[][] GenerateParallelNaive(int pxSize, int maxIterations)
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
                    double x0 = minX + i * deltaX;

                    for (int j = 0; j < pxSize; j++)
                    {
                        double y0 = minY + j * deltaY;

                        double x = 0.0;
                        double y = 0.0;
                        int iterations = 0;

                        while (x * x + y * y <= 2 * 2 && iterations < maxIterations)
                        {
                            double xTemp = x * x - y * y + x0;
                            y = 2 * x * y + y0;
                            x = xTemp;
                            iterations++;
                        }

                        _escapeValues[i][j] = iterations;
                    }
                }
            );

            return _escapeValues;
        }

        /// <summary>
        ///     Optimized escape time algorithm
        ///     Does not use Complex type
        /// </summary>
        /// <param name="pxSize"></param>
        /// <param name="maxIterations"></param>
        /// <returns></returns>
        public int[][] GenerateParallelOptimized(int pxSize, int maxIterations)
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
                    double x0 = minX + i * deltaX;

                    for (int j = 0; j < pxSize; j++)
                    {
                        double y0 = minY + j * deltaY;

                        double x = 0.0;
                        double xSqr = 0.0;
                        double y = 0.0;
                        double ySqr = 0.0;
                        int iterations = 0;

                        while (xSqr + ySqr <= 4 && iterations < maxIterations)
                        {
                            y = (x + x) * y + y0;
                            x = xSqr - ySqr + x0;
                            xSqr = x * x;
                            ySqr = y * y;
                            iterations++;
                        }

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
                Complex z2 = Complex.Add(Complex.Pow(z, 2), c);
                z2 = new Complex(Math.Round(z2.Real, 10), Math.Round(z2.Imaginary, 10));
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

        public static (List<Complex> listZ, List<double> listR) IterateZWithLogging(Complex c, int maxIterations)
        {
            var listZ = new List<Complex>();
            var listR = new List<double>();

            int iterations = 0;
            var z = new Complex(0, 0);

            while (iterations < maxIterations)
            {
                Complex z2 = Complex.Add(Complex.Pow(z, 2), c);
                z2 = new Complex(Math.Round(z2.Real, 10), Math.Round(z2.Imaginary, 10));

                double r = z2.Magnitude;
                listZ.Add(z2);
                listR.Add(r);

                if (r > MAX_R)
                {
                    break;
                }

                z = z2;
                iterations++;
            }

            return (listZ, listR);
        }
    }
}
