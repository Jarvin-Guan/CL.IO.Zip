using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JarvinZip
{

    class ProcessItem
    {
        public ProcessItem(double NeedHandleCount)
        {
            this.NeedHandleCount = NeedHandleCount;
        }
        public double NeedHandleCount { set; get; }
        public double HadHandleCount { set; get; }
    }
}
