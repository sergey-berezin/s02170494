using System;
using System.Collections.Generic;
using System.Text;

namespace UserInterfaceRecognition
{
    public class DataForServer
    {
        public string Path { get; set; }
        public string ImageString { get; set; }

        public DataForServer(string p, byte[] im)
        {
            this.Path = p;
            this.ImageString = Convert.ToBase64String(im);
        }

        public DataForServer(string p, string i)
        {
            this.Path = p;
            this.ImageString = i;
        }

        public DataForServer()
        {

        }
    }
}
