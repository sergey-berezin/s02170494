using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserInterfaceRecognition;

namespace ImageServer.DataBase
{
    public interface IInMemoryDataBase
    {
        StatisticInfo GetStatistic();
        void ClearDataBase();
        void AddToDataBase(RecognitionModel recog);
        Context DataBaseContext { get; set; }
    }
    public class InMemoryDataBase: IInMemoryDataBase
    {
        public Context DataBaseContext { get; set; }
        public InMemoryDataBase()
        {
            this.DataBaseContext = new Context();
        }

        public StatisticInfo GetStatistic()
        {
            StatisticInfo statistic = new StatisticInfo();
            foreach (var item in DataBaseContext.ClassLabelsInfo.Include(p => p.RecogModel))
            {
                statistic.stat.Add(item.Name, item.RecogModel.Count);
            }
            return statistic;
        }

        public void ClearDataBase()
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
        }

        public void AddToDataBase(RecognitionModel recog)
        {
            var dbNewElem = new DbRecognitionModel(recog);
            DataBaseContext.DataBaseInfo.Add(dbNewElem);
            DataBaseContext.SaveChanges();
            if (DataBaseContext.ClassLabelsInfo.Find(dbNewElem.ClassLabel) == null)
            {
                DataBaseContext.ClassLabelsInfo.Add(new ClassInfo(dbNewElem.ClassLabel, dbNewElem));
            }
            else
            {

                DataBaseContext.ClassLabelsInfo.Include(p => p.RecogModel).ToList().Find(l => l.Name ==dbNewElem.ClassLabel).RecogModel.Add(dbNewElem);
            }
            DataBaseContext.SaveChanges();
        }
    }
}
