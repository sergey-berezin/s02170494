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
using System.IO;
using Microsoft.EntityFrameworkCore;


namespace UserInterfaceRecognition
{
    public class RecognitionViewModel: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public Commands StartRecognitionCommand { get; set; }
        public Commands CancelRecognitionCommand { get; set; }
        public Commands ClearDataBaseCommand { get; set; }
        public Commands StatisticCommand { get; set; }
        public ObservableCollection<Tuple<string, int>> ClassObserv { get; set; }
        public ObservableCollection<RecognitionModel> Observ { get; set; }
        public ObservableCollection<RecognitionModel> SelectedClassObserv { get; set; }
        public Recognize Recon { get; set; }
        public Dispatcher Dispatcherr { get; set; }
        public Context DataBaseContext { get; set; }
        public RecognitionViewModel()
        {
            this.Observ = new ObservableCollection<RecognitionModel>();
            this.ClassObserv = new ObservableCollection<Tuple<string, int>>();
            this.SelectedClassObserv = new ObservableCollection<RecognitionModel>();
            this.Recon = new Recognize();
            this.Recon.Notify += OnPredictionCome;
            this.Dispatcherr = Dispatcher.CurrentDispatcher;
            this.DataBaseContext = new Context();

            this.StartRecognitionCommand = new Commands(Start);
            this.CancelRecognitionCommand = new Commands(Cancel);
            this.ClearDataBaseCommand = new Commands(Clear);
            this.StatisticCommand = new Commands(Statistic);
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

        private  void OnPredictionCome(PredictionResult pr, EventArgs e, bool isInBase = false)
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
                {
                    ClassObserv[index] = new Tuple<string, int>(ClassObserv[index].Item1, ClassObserv[index].Item2 + 1);

                }
                else
                    ClassObserv.Add(new Tuple<string, int>(pr.ClassLabel, 1));
                if (!isInBase)
                {
                    var newElem = new RecognitionModel(pr.Path, pr.ClassLabel);
                    DataBaseContext.DataBaseInfo.Add(newElem);
                    DataBaseContext.SaveChanges();
                    if (DataBaseContext.ClassLabelsInfo.Find(newElem.ClassLabel) == null)
                    {
                        DataBaseContext.ClassLabelsInfo.Add(new ClassInfo(newElem.ClassLabel, newElem));
                    }
                    else
                    {
                        DataBaseContext.ClassLabelsInfo.Find(newElem.ClassLabel).RecogModel.Add(newElem);
                    }
                    DataBaseContext.SaveChanges();
                   
                }


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
                DirectoryInfo dirInfo = new DirectoryInfo(fbd.SelectedPath);
                FileInfo[] files = dirInfo.GetFiles("*.jpg");
                List<string> imagesNotInBase = new List<string>();
                var tmp = DataBaseContext.DataBaseInfo.Include(p => p.Image);
                foreach (var item in files)
                {
                    foreach (var it in tmp.Where(p => p.Path == item.FullName && p.Image.ImageBlob.SequenceEqual(File.ReadAllBytes(item.FullName))))
                    {
                        PredictionResult findPred = new PredictionResult(it.Path, it.ClassLabel);
                        OnPredictionCome(findPred, null, true);
                    }
                    
                    if (!tmp.Any(p => p.Path == item.FullName && p.Image.ImageBlob.SequenceEqual(File.ReadAllBytes(item.FullName))))
                    {
                        imagesNotInBase.Add(item.FullName);
                    }
                }

                Recognition(imagesNotInBase); 
                
            }
        }

        private void Recognition(List<string> path)
        {
            Task.Run(() =>
                {
                    Recognize.cts = new CancellationTokenSource();
                    Recon.ParallelProcess(path);
                    this.Dispatcherr.BeginInvoke(new Action(() => { DataBaseContext.SaveChanges(); }));
                });
            
        }

        private void Cancel(object sender)
        {
            this.Recon.Stop();
        }
        
        private void Clear(object sender)
        {
            foreach (var item in DataBaseContext.DataBaseInfo)
            {
                DataBaseContext.DataBaseInfo.Remove(item);
            }
            foreach (var item in DataBaseContext.ClassLabelsInfo)
            {
                DataBaseContext.ClassLabelsInfo.Remove(item);
            }
            DataBaseContext.SaveChanges();
            ClassObserv.Clear();
            Observ.Clear();
            SelectedClassObserv.Clear();

        }

        private void Statistic(object sender)
        {
            StatisticInfo statistic = new StatisticInfo();
            foreach (var item in DataBaseContext.ClassLabelsInfo.Include(p => p.RecogModel))
            {
                statistic.stat.Add(item.Name, item.RecogModel.Count);
            }
            MessageBox.Show(statistic.ToString());

        }

        

        
        


    }
}
