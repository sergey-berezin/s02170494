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
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

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
        public HttpClient Client { get; set; }
        public string ServerUrl { get; set; } = "http://localhost:5000/imagerecognition";

        public CancellationTokenSource cts { get; set; } = new CancellationTokenSource();
        public RecognitionViewModel()
        {
            this.Observ = new ObservableCollection<RecognitionModel>();
            this.ClassObserv = new ObservableCollection<Tuple<string, int>>();
            this.SelectedClassObserv = new ObservableCollection<RecognitionModel>();
            this.Recon = new Recognize();

            this.Dispatcherr = Dispatcher.CurrentDispatcher;
            this.DataBaseContext = new Context();

            this.StartRecognitionCommand = new Commands(Start);
            this.CancelRecognitionCommand = new Commands(Cancel);
            this.ClearDataBaseCommand = new Commands(Clear);
            this.StatisticCommand = new Commands(Statistic);

            this.Client = new HttpClient();
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

        

        private async void Start(object sender)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                ClassObserv.Clear();
                Observ.Clear();
                SelectedClassObserv.Clear();
                try
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(fbd.SelectedPath);
                    FileInfo[] files = dirInfo.GetFiles("*.jpg");
                    var list = new List<DataForServer>();
                    foreach (var ffile in files)
                    {
                        list.Add(new DataForServer(ffile.FullName, Convert.ToBase64String(File.ReadAllBytes(ffile.FullName))));
                    }
                    var jsonString = JsonConvert.SerializeObject(list);
                    var content = new StringContent(jsonString);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await Client.PostAsync(ServerUrl + "/InBase", content, cts.Token);

                    var result = JsonConvert.DeserializeObject<List<ServerRecognitionModel>>(response.Content.ReadAsStringAsync().Result);


                    foreach (var item in result)
                    {
                        Observ.Add(new RecognitionModel(item));
                        int index = -1;
                        foreach (var tmp in ClassObserv)
                        {
                            if (tmp.Item1.Equals(item.ClassLabel))
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
                            ClassObserv.Add(new Tuple<string, int>(item.ClassLabel, 1));

                    }

                    var jsonString1 = JsonConvert.SerializeObject(list);
                    var content1 = new StringContent(jsonString1);
                    content1.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var responsenot = await Client.PostAsync(ServerUrl + "/NotInBase", content1, cts.Token);
                    var h = responsenot.Content.ReadAsStringAsync().Result;
                    var resultnot = JsonConvert.DeserializeObject<List<PredictionResult>>(h);
                    if (resultnot != null)
                    {
                        foreach (var item in resultnot)
                        {
                            Observ.Add(new RecognitionModel(item.Path, item.ClassLabel));

                            int index = -1;
                            foreach (var tmp in ClassObserv)
                            {
                                if (tmp.Item1.Equals(item.ClassLabel))
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
                                ClassObserv.Add(new Tuple<string, int>(item.ClassLabel, 1));

                        }
                    }

                }

                catch (TaskCanceledException) 
                {
                    MessageBox.Show("It was cancelling");
                }
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
            cts.Cancel();
            cts = new CancellationTokenSource();

        }
        
        private void Clear(object sender)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(param =>
            {
                try
                {
                    var httpResponse = Client.DeleteAsync(ServerUrl).Result;
                    
                }
                catch (AggregateException)
                {
                    Dispatcherr.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show("Error");
                    }));
                }
            }));

        }

        private void Statistic(object sender)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(param =>
            {
                try
                {
                    var httpResponse = Client.GetAsync(ServerUrl).Result;
                    var statistic = JsonConvert.DeserializeObject<StatisticInfo>(httpResponse.Content.ReadAsStringAsync().Result);
                    MessageBox.Show(statistic.ToString());
                }
                catch (AggregateException)
                {
                    Dispatcherr.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show("Error");
                    }));
                }
            }));

        }

        

        
        


    }
}
