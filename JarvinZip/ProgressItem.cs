using System;
using System.Collections.Generic;
using System.Text;

namespace CL.IO.ZIP
{

    class ProcessItem
    {
        public ProcessItem(double NeedHandleCount)
        {
            this.NeedHandleCount = NeedHandleCount;
            this.HandledFiles = new List<string>();
        }
        public double NeedHandleCount { set; get; }
        public double HadHandleCount { set; get; }
        public List<string> HandledFiles { set; get; }
    }
}
