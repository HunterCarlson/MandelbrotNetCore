using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Text.Json;

namespace MandelbrotNetCore
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("Hello Mandelbrot!");

            //ParallelVsSerial();

            Draw();

            Console.WriteLine("Done");

            Console.Read();
        }

        private static void ParallelVsSerial()
        {
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
        }

        private static void Draw()
        {
            var mandelbrot = new Mandelbrot();

            const int pxSize = 1000;
            const int maxIterations = 100;

            var bitmap = new Bitmap(pxSize, pxSize);

            int[][] iterations = mandelbrot.Generate(pxSize, maxIterations);

            double hueStep = 36;
            double hueOffset = 0;

            for (int i = 0; i < pxSize; i++)
            {
                for (int j = 0; j < pxSize; j++)
                {
                    Color color;

                    if (iterations[i][j] == maxIterations || iterations[i][j] == 0)
                    {
                        color = Color.Black;
                    }
                    else
                    {
                        double hue = (hueStep * (iterations[i][j] - 1) + hueOffset) % 360;
                        ColorConverter.HsvToRgb(hue, 1, 1, out int r, out int g, out int b);
                        color = Color.FromArgb(255, r, g, b);
                    }

                    bitmap.SetPixel(i, j, color);
                }
            }

            bitmap.Save("MandelbrotRender.png", ImageFormat.Png);
        }

        private static void GenTestData()
        {
            var mandelbrot = new Mandelbrot();
            int[][] iterations = mandelbrot.Generate(40, 100);
            string json = JsonSerializer.Serialize(iterations);

            (List<Complex> iterationSteps, List<double> magnitude) iterationSteps =
                Mandelbrot.IterateZWithLogging(new Complex(-0.38, 0.44), 100);
        }
    }
}
