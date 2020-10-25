using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Forms;
using ImageRecognitionLibrary;
using System.ComponentModel;
using System.Threading.Tasks;

namespace UserInterfaceRecognition
{
    public class RecognitionViewModel: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public Commands StartRecognitionCommand { get; set; }
        public Commands CancelRecognitionCommand { get; set; }
        public ObservableCollection<Tuple<string, int>> ClassObserv { get; set; }
        public ObservableCollection<RecognitionModel> Observ { get; set; }
        public ObservableCollection<RecognitionModel> SelectedClassObserv { get; set; }
        public Recognize Recon { get; set; }
        public Dispatcher Dispatcherr { get; set; }

        public RecognitionViewModel()
        {
            this.Observ = new ObservableCollection<RecognitionModel>();
            this.ClassObserv = new ObservableCollection<Tuple<string, int>>();
            this.SelectedClassObserv = new ObservableCollection<RecognitionModel>();
            this.Recon = new Recognize();
            this.Recon.Notify += OnPredictionCome;
            this.Dispatcherr = Dispatcher.CurrentDispatcher;

            this.StartRecognitionCommand = new Commands(Start);
            this.CancelRecognitionCommand = new Commands(Cancel);
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private Tuple<string, int> selectedClass;
        public  Tuple<string, int> SelectedClass
        {
            get
            {
                return selectedClass;
            }
            set 
            { 
                selectedClass = value;
                if (value != null)
                {
                    this.SelectedClassObserv = new ObservableCollection<RecognitionModel>(Observ.Where(p => p.ClassLabel == selectedClass.Item1));
                    OnPropertyChanged("SelectedClassObserv");
                }
                
            }
        }

        private  void OnPredictionCome(PredictionResult pr, EventArgs e)
        {

            Dispatcherr.BeginInvoke(DispatcherPriority.Background,new Action(() =>
            {
                Observ.Add(new RecognitionModel(pr.Path, pr.ClassLabel));
                int index = -1;
                foreach (var tmp in ClassObserv)
                {
                    if (tmp.Item1.Equals(pr.ClassLabel))
                    {
                        index = ClassObserv.IndexOf(tmp);
                        break;
                    }
                }
                if (index != -1)
                    ClassObserv[index] = new Tuple<string, int>(ClassObserv[index].Item1, ClassObserv[index].Item2 + 1);
                else
                    ClassObserv.Add(new Tuple<string, int>(pr.ClassLabel, 1));
            }));
            
        }

        private void Start(object sender)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                ClassObserv.Clear();
                Observ.Clear();
                SelectedClassObserv.Clear();
                Recognition(fbd.SelectedPath); 
                
            }
        }

        private void Recognition(string path)
        {
            Task.Run(() =>
                {
                    Recognize.cts = new CancellationTokenSource();
                    Recon.ParallelProcess(path);
                });
        }

        private void Cancel(object sender)
        {
            this.Recon.Stop();
        }
        
        


    }
}
