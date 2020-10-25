using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ImageRecognitionLibrary;

namespace Image_recognition
{
    class Program
    {
        static void Pred(object sender, EventArgs e)
        {
            PredictionResult item;
            if ((ConcurrentQueue<PredictionResult>)sender != null)
            {
                ((ConcurrentQueue<PredictionResult>)sender).TryDequeue(out item);
                Console.WriteLine($"file: {item.Path} result: {item.ClassLabel}");
            }
        }

        static void Main(string[] args)
        {
            string dirpath, modelpath;

            Console.WriteLine("Please enter images directory");
            dirpath = Console.ReadLine();

            Console.WriteLine("Please enter model directory");
            modelpath = Console.ReadLine();

            Recognize recon = new Recognize(modelpath);
            recon.Notify += Pred;
            Thread cancelTread = new Thread(() =>
            {
                while (true)
                {
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.C)
                    {
                        recon.Stop();
                        break;
                    }
                    if (Recognize.endSignal == Directory.GetFiles(dirpath, "*.jpg").Length)
                        break;
                }
            }
            );
            cancelTread.Start();
            recon.ParallelProcess(dirpath);
            

        }
  
    }
}
