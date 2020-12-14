using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ImageRecognitionLibrary;
using ImageServer.DataBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserInterfaceRecognition;

namespace ImageServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageRecognitionController : ControllerBase
    {

        private IInMemoryDataBase dataBase;
        private Recognize recog;
        //private ConcurrentQueue<PredictionResult> res { get; set; } = new ConcurrentQueue<PredictionResult>();
        public List<DataForServer> dfsint = new List<DataForServer>();



        private readonly ILogger<ImageRecognitionController> _logger;

        public ImageRecognitionController(ILogger<ImageRecognitionController> logger)
        {
            _logger = logger;
            this.recog = new Recognize();
            this.dataBase = new InMemoryDataBase();
        }

        [HttpGet]
        public ActionResult<StatisticInfo> Get()
        {
           return this.dataBase.GetStatistic();
        }
        [HttpDelete]
        public void Clear()
        {
            if (this.dataBase != null)
                this.dataBase.ClearDataBase();
        }

        [HttpPost("InBase")]
        public ActionResult<List<ServerRecognitionModel>> PostImageInBase([FromBody] List<DataForServer> dfs)
        {
            
            var tmp = dataBase.DataBaseContext.DataBaseInfo.Include(p => p.BlobImage);
            var list = new List<ServerRecognitionModel>();
            foreach (var item in dfs)
            {
                foreach (var it in tmp.Where(p => p.Path == item.Path && p.BlobImage.ImageBlob.SequenceEqual(Convert.FromBase64String(item.ImageString))))
                {
                    
                    list.Add(new ServerRecognitionModel(it.Path, it.BlobImage.ImageBlob, it.ClassLabel));
                }
                
            }
            return list;
        }

        [HttpPost("NotInBase")]
        public ActionResult<List<PredictionResult>> PostImageNotInBase([FromBody] List<DataForServer> dfs)
        {
            var tmp = dataBase.DataBaseContext.DataBaseInfo.Include(p => p.BlobImage);
            var list = new List<Tuple<string, byte[]>>();
            var resultt = new List<PredictionResult>();
            ConcurrentQueue<PredictionResult> res = new ConcurrentQueue<PredictionResult>();
            dfsint = dfs;
            foreach (var item in dfs)
            {
                if (!tmp.Any(p => p.Path == item.Path && p.BlobImage.ImageBlob.SequenceEqual(Convert.FromBase64String(item.ImageString))))
                {
                    list.Add(new Tuple<string, byte[]>(item.Path, Convert.FromBase64String(item.ImageString)));
                }
            }
            recog.Notify += (PredictionResult pr, EventArgs e, bool isInBase) => {
               
                res.Enqueue(new PredictionResult(pr.Path, pr.ClassLabel));
                Console.WriteLine(pr.Path);
                var newElem = dfsint.FirstOrDefault(p => p.Path == pr.Path);
            };
            recog.ParallelProcessFromServer(list);
            foreach(var item in res)
            {
                resultt.Add(item);
                dataBase.AddToDataBase(new RecognitionModel(item.Path, item.ClassLabel, dfsint.FirstOrDefault(p => p.Path == item.Path).ImageString));
            
            }
            return resultt;
                
        }


    }
}
