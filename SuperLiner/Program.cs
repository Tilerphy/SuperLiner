using SuperLiner.Core;
using System;
using System.Collections.Generic;

namespace SuperLiner
{
    class Program
    {
        static void Main(string[] args)
        {
            SLContext c = SLContext.Current;
            //c.RuntimeRegister.Values.Add("__current_timeline__", "version2");
            string x = System.IO.File.ReadAllText(@"C:\Users\Administrator\Desktop\scripthere.txt");
            SLLineLoader.ReadLinesFromSrcript(x);
            string currentTimeline = args.Length > 0 ? args[0] : "__default_timeline__";
            c.RuntimeRegister.Values.Add("__current_timeline__", currentTimeline);
            string defaultStopTimeline = args.Length > 1 ? args[1] : "__default_stop_timeline__";
            c.RuntimeRegister.Values.Add("__stop_timeline__", defaultStopTimeline);

            List<string> tl = (SLContext.Current.ScriptRegister.Values["__timeline__"] as List<string>);
            if (!tl.Contains(currentTimeline))
            {
                throw new NotSupportedException(string.Format("Cannot find timeline {0}", currentTimeline));
            }

            if (!tl.Contains(defaultStopTimeline) && defaultStopTimeline!= "__default_stop_timeline__")
            {
                throw new NotSupportedException(string.Format("Cannot find timeline {0}", defaultStopTimeline));
            }
            (c.ScriptRegister.Values["__main__"] as SLFunction).Execute();
        }
    }
}
