using System;
using System.Collections.Generic;
using System.Text;

namespace UserInterfaceRecognition
{
    public class StatisticInfo
    {
        public Dictionary<string, int> stat { get; set; }
        public StatisticInfo()
        {
            this.stat = new Dictionary<string, int>();
        }

        public override string ToString()
        {
            string result = "";
            if (stat != null)
            {
                foreach (var item in stat)
                {
                    result += item.Key + " " + item.Value.ToString() + '\n';

                }
            }
            return result;
        }
    }
}
