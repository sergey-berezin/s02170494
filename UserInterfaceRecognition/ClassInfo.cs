using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserInterfaceRecognition
{
    public class ClassInfo
    {
        [Key]
        public string Name { get; set; }
        public List<DbRecognitionModel> RecogModel { get; set; }
        public ClassInfo(string name, DbRecognitionModel rec)
        {
            this.Name = name;
            this.RecogModel = new List<DbRecognitionModel>();
            this.RecogModel.Add(rec);
        }
        public ClassInfo()
        { }


    }
}
