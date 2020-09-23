using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Image_recog_lib;

namespace Image_recognition
{
    class Program
    {
        static void Pred(object sender, EventArgs e)
        {
            string item;
            ((ConcurrentQueue<string>)sender).TryDequeue(out item);
            Console.WriteLine(item);
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
            recon.ParallelProcess(dirpath);
            

        }
  
    }
}
