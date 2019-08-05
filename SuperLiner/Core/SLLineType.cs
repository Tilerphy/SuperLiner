using System;
using System.Collections.Generic;
using System.Text;

namespace SuperLiner.Core
{
    public class SLLineDescription
    {
        public SLLineType LineType { get; set; }
        public string LeftScript { get; set; }
    }

    public enum SLLineType
    {
        Line =0 ,
        DefiningFunc =1,
        EndingFunc =2,
        NotSupport = 3,
        Timeline = 4
    }
}
