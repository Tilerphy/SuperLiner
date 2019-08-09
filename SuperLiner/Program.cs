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
            SLLineLoader.InitScriptLoader();
            string currentTimeline = args.Length > 1 ? args[1] : Contants.Default_Timeline;
            c.RuntimeRegister.Values.Add(Contants.Current_Timeline_Key, currentTimeline);
            string defaultStopTimeline = args.Length > 2 ? args[2] : Contants.Default_Stop_Timeline;
            c.RuntimeRegister.Values.Add(Contants.Stop_Timeline_Key, defaultStopTimeline);
            //c.RuntimeRegister.Values.Add("__current_timeline__", "version2");
            if (args.Length > 0)
            {
                //the end of the command is script path
                string path = args[0];
                if (path == "--slaver" || path == "-s")
                {
                    SLSlaveListener l = new SLSlaveListener(args[1], int.Parse(args[2]), args[3]);
                }
                else
                {
                    string x = System.IO.File.ReadAllText(path);
                    SLLineLoader.ReadLinesFromScript(x);


                    List<string> tl = (SLContext.Current.ScriptRegister.Values[Contants.Timeline_List_Key] as List<string>);
                    if (!tl.Contains(currentTimeline))
                    {
                        throw new NotSupportedException(string.Format("Cannot find timeline {0}", currentTimeline));
                    }

                    if (!tl.Contains(defaultStopTimeline) && defaultStopTimeline != Contants.Default_Stop_Timeline)
                    {
                        throw new NotSupportedException(string.Format("Cannot find timeline {0}", defaultStopTimeline));
                    }
                (c.ScriptRegister.Values[Contants.Main_Func_Key] as SLFunction).Execute();
                }
                
            }
            else
            {
                while (true)
                {
                    Console.Write("SL >");
                    string line = Console.ReadLine();
                    SLLine slLine = SLLineLoader.LineToSLLine(line, Contants.Main_Func_Key);
                    slLine.Execute();
                }
            }
            Console.ReadLine();
        }
    }
}
