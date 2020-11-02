using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace UserInterfaceRecognition
{
    public class RecognitionModel: INotifyPropertyChanged
    {
        private string classLabel;

        public event PropertyChangedEventHandler PropertyChanged;

        private BitmapImage image;
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

        public BitmapImage Image
        {
            get
            {
                return image;
            }

            set
            {
                image = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Image"));
            }
        }

        public RecognitionModel(string name, string label)
        {
            this.ClassLabel = label;
            this.Path = name;
            this.Image = new BitmapImage(new Uri(name));
        }
    }

}
