using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;

namespace UserInterfaceRecognition
{
    public class RecognitionModel: INotifyPropertyChanged
    {
        private string classLabel;


        public event PropertyChangedEventHandler PropertyChanged;

        private byte[] blobImage;
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }

        public string ClassLabel
        {
            get
            {
                return classLabel;
            }
            set
            {
                classLabel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClassLabel"));
            }
        }

        public byte[] Image
        {
            get
            {
                return blobImage;
            }

            set
            {
                blobImage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Image"));
            }
        }

        public RecognitionModel(string name, string label)
        {
            this.ClassLabel = label;
            this.Path = name;
            if (name != null)
                this.Image = File.ReadAllBytes(name);
        }

        public RecognitionModel(ServerRecognitionModel srm)
        {
            this.ClassLabel = srm.ClassLabel;
            this.Path = srm.Path;
            this.Image = Convert.FromBase64String(srm.ImageString);
        }



        public RecognitionModel()
        { }

        public RecognitionModel(string p, string cl, string img)
        {
            this.Path = p;
            this.ClassLabel = cl;
            this.Image = Convert.FromBase64String(img);
        }
    }

}
