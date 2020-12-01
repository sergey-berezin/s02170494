using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace UserInterfaceRecognition
{
    public class DbRecognitionModel
    {
        public Blob BlobImage { get; set; }
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        public string ClassLabel { get; set; }
        public DbRecognitionModel()
        {
        }

        public DbRecognitionModel(RecognitionModel recog)
        {
            this.BlobImage = new Blob();
            this.Path = recog.Path;
            this.BlobImage.ImageBlob = recog.Image;
            this.ClassLabel = recog.ClassLabel;
        }


    }
}
