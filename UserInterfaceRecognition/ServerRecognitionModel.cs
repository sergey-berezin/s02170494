using System;
using System.Collections.Generic;
using System.Text;

namespace UserInterfaceRecognition
{
    public class ServerRecognitionModel
    {
        public string Path { get; set; }
        public string ImageString { get; set; }
        public string ClassLabel { get; set; }
        public ServerRecognitionModel(string p, byte[] im, string cl)
        {
            this.Path = p;
            this.ImageString = Convert.ToBase64String(im);
            this.ClassLabel = cl;
        }

        public ServerRecognitionModel()
        {

        }
    }
}
