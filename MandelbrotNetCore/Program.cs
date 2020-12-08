using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace MandelbrotNetCore
{
    internal class Program
    {
        private static async Task Main()
        {
            Console.WriteLine("Hello Mandelbrot!");

            TimeAlgs();

            //var renderer = new Renderer(1000);
            //Bitmap bmp = renderer.Draw();
            //bmp.Save("MandelbrotRender.png", ImageFormat.Png);

            //const int pxSize = 1080;
            //RenderFrames(pxSize);
            //await RenderVideo(pxSize);

            Console.WriteLine("Done");

            Console.Read();
        }

        private static void TimeAlgs()
        {
            var mandelbrot = new Mandelbrot();

            var stopwatch = new Stopwatch();

            const int pxSize = 1000;
            const int maxIterations = 10000;

            // Timing

            // TOO SLOW LOL
            //stopwatch.Start();
            //int[][] iterations1 = mandelbrot.Generate(pxSize, maxIterations);
            //stopwatch.Stop();
            //Console.WriteLine("Serial:");
            //Console.WriteLine(stopwatch.Elapsed);
            //Console.WriteLine();

            stopwatch.Restart();
            int[][] iterations2 = mandelbrot.GenerateParallelWithComplex(pxSize, maxIterations);
            stopwatch.Stop();
            Console.WriteLine("Parallel (with Complex type):");
            Console.WriteLine(stopwatch.Elapsed);
            Console.WriteLine();

            stopwatch.Restart();
            int[][] iterations3 = mandelbrot.GenerateParallelNaive(pxSize, maxIterations);
            stopwatch.Stop();
            Console.WriteLine("Parallel (naive):");
            Console.WriteLine(stopwatch.Elapsed);
            Console.WriteLine();

            stopwatch.Restart();
            int[][] iterations4 = mandelbrot.GenerateParallelOptimized(pxSize, maxIterations);
            stopwatch.Stop();
            Console.WriteLine("Parallel (optimized):");
            Console.WriteLine(stopwatch.Elapsed);
            Console.WriteLine();
        }

        private static void GenTestData()
        {
            var mandelbrot = new Mandelbrot();
            int[][] iterations = mandelbrot.Generate(40, 100);
            string json = JsonSerializer.Serialize(iterations);

            (List<Complex> iterationSteps, List<double> magnitude) iterationSteps =
                Mandelbrot.IterateZWithLogging(new Complex(-0.38, 0.44), 100);
        }

        /// <summary>
        ///     Render hue shift frames for animation
        /// </summary>
        private static void RenderFrames(int pxSize)
        {
            // Make a folder for the frames
            if (!Directory.Exists("Frames"))
            {
                Directory.CreateDirectory("Frames");
            }

            // Make a folder for the pxSize
            if (!Directory.Exists($"Frames/{pxSize}px"))
            {
                Directory.CreateDirectory($"Frames/{pxSize}px");
            }

            // Render
            var renderer = new Renderer(pxSize);

            for (int i = 0; i < 360; i++)
            {
                Bitmap bmp = renderer.Draw(hueOffset: i);
                // add leading 0s to file name
                string frameNum = $"{i}".PadLeft(3, '0');
                bmp.Save($"Frames/{pxSize}px/Frame_{frameNum}.png", ImageFormat.Png);

                if (i % 10 == 0)
                {
                    int percentComplete = (int) (i / 360.0 * 100);
                    Console.WriteLine($"Rendered frame {i} of 360 ({percentComplete}%)");
                }
            }
        }

        private static async Task RenderVideo(int pxSize)
        {
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);

            string dir = $"Frames/{pxSize}px";

            if (!Directory.Exists(dir))
            {
                throw new DirectoryNotFoundException(dir);
            }

            List<string> files = Directory.EnumerateFiles(dir).ToList();

            await FFmpeg.Conversions.New()
                .BuildVideoFromImages(files)
                .SetFrameRate(60)
                .SetOutput($"Mandelbrot{pxSize}px.mp4")
                .Start();

            Console.WriteLine("Video Rendered!");
        }
    }
}
