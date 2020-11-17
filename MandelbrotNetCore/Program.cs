using System;
using System.Diagnostics;

namespace MandelbrotNetCore
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("Hello Mandelbrot!");

            var mandelbrot = new Mandelbrot();

            var stopwatch = new Stopwatch();

            const int pxSize = 1000;
            const int maxIterations = 1000;

            // Timing

            stopwatch.Start();
            int[][] iterations = mandelbrot.Generate(pxSize, maxIterations);
            stopwatch.Stop();
            Console.WriteLine("Serial:");
            Console.WriteLine(stopwatch.Elapsed);
            Console.WriteLine();

            stopwatch.Restart();
            int[][] iterations2 = mandelbrot.GenerateParallel(pxSize, maxIterations);
            stopwatch.Stop();
            Console.WriteLine("Parallel:");
            Console.WriteLine(stopwatch.Elapsed);
            Console.WriteLine();

            // Check data is correct

            for (int i = 0; i < pxSize; i++)
            {
                for (int j = 0; j < pxSize; j++)
                {
                    if (iterations[i][j] != iterations2[i][j])
                    {
                        throw new Exception("iteration values were different");
                    }
                }
            }

            Console.WriteLine("Data matches");
            Console.WriteLine();

            Console.WriteLine("Done");

            Console.Read();
        }
    }
}
