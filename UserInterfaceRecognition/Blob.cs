using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UserInterfaceRecognition
{
    public class Blob
    {
        public int BlobId { get; set; }
        public byte[] ImageBlob { get; set; }
        public Blob() { }
        public Blob(string img)
        {
            this.ImageBlob = File.ReadAllBytes(img);
        }
    }
}
