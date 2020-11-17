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
        public List<RecognitionModel> RecogModel { get; set; }
        public ClassInfo(string name = "", RecognitionModel rec = null)
        {
            this.Name = name;
            this.RecogModel = new List<RecognitionModel>();
            this.RecogModel.Add(rec);
        }
        public ClassInfo()
        { }


    }
}
