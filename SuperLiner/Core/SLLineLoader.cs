using SuperLiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace SuperLiner
{
    public class SLLineLoader
    {
        public static void ReadLinesFromSrcript(string script)
        {
            StringReader sr = new StringReader(script);
            SLFunction mainFunc = new SLFunction();
            //__main__ cannot be endfunc
            mainFunc.Name = "__main__";
            string definingFunc = "__main__";
            SLContext.Current.ScriptRegister.Values.Add(mainFunc.Name, mainFunc);
            while (sr.Peek() != -1)
            {
                string line = sr.ReadLine().Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                else
                {
                    SLLineDescription lineDesc = WhatLine(line);
                    switch (lineDesc.LineType)
                    {
                        case SLLineType.Line:
                            if (SLContext.Current.ScriptRegister.Values.ContainsKey(definingFunc))
                            {
                                (SLContext.Current.ScriptRegister.Values[definingFunc] as SLFunction).AppendLine(LineToSLLine(line));
                            }
                            else
                            {
                                throw new NotSupportedException(string.Format("Cannot find the function named {0}", definingFunc));
                            }
                            break;
                        case SLLineType.DefiningFunc:
                            if (lineDesc.LeftScript.Equals("__main__", StringComparison.OrdinalIgnoreCase))
                            {
                                throw new NotSupportedException(string.Format("Cannot define a function named {0}", "__main__"));
                            }
                            if (definingFunc != "__main__")
                            {
                                throw new NotSupportedException(string.Format("Cannot define a nested function."));
                            }
                            SLContext.Current.ScriptRegister.Values.Add(lineDesc.LeftScript, new SLFunction() { Name = lineDesc.LeftScript });
                            definingFunc = lineDesc.LeftScript;
                            break;
                        case SLLineType.EndingFunc:
                            if (definingFunc == "__main__")
                            {
                                throw new NotSupportedException(string.Format("Cannot end function __main__"));
                            }
                            definingFunc = "__main__";
                            break;
                        case SLLineType.NotSupport:
                            throw new NotSupportedException();
                            break;

                    }
                }
            }
        }


        public static SLLineDescription WhatLine(string line)
        {
            SLLineDescription sLLineDescription = new SLLineDescription();
            if (line.StartsWith("func", StringComparison.OrdinalIgnoreCase))
            {
                string[] definingFuncSplit = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (definingFuncSplit.Length == 2)
                {
                    sLLineDescription.LineType = SLLineType.DefiningFunc;

                }
                else
                {
                    sLLineDescription.LineType = SLLineType.NotSupport;
                }
                sLLineDescription.LeftScript = definingFuncSplit[1].Trim();
            }
            else if (line.Equals("endfunc", StringComparison.OrdinalIgnoreCase))
            {
                sLLineDescription.LineType = SLLineType.EndingFunc;
            }
            else
            {
                sLLineDescription.LineType = SLLineType.Line;
                sLLineDescription.LeftScript = line;
            }
            return sLLineDescription;
        }

        public static SLLine LineToSLLine(string line)
        {
            SLLine slLine = new SLLine();
            string[] regOpSplit = line.Split("=>",StringSplitOptions.RemoveEmptyEntries);
            if (regOpSplit.Length > 2)
            {
                throw new NotSupportedException("Cannot use more than one =>.");
            }
            if (regOpSplit.Length == 2)
            {
                slLine.PipeToRegister = regOpSplit[1].Trim();
            }
            string[] actionSplit = regOpSplit[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);
            slLine.Action = actionSplit[0];
            slLine.Parameters = actionSplit.Skip(1).ToArray<object>();
            return slLine;
        }
    }
}
