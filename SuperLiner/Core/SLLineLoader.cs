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
        private static string definingFunc = "__main__";
        public static void ReadLinesFromScript(string script)
        {
            StringReader sr = new StringReader(script);
            
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
                                (SLContext.Current.ScriptRegister.Values[definingFunc] as SLFunction).AppendLine(LineToSLLine(line, definingFunc));
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
                        case SLLineType.Timeline:

                            if (definingFunc == "__main__")
                            {
                                List<string> tl = (SLContext.Current.ScriptRegister.Values["__timeline__"] as List<string>);
                                if (tl.Contains(lineDesc.LeftScript))
                                {
                                    throw new NotSupportedException(string.Format("Cannot create same timeline in __main__. {0}", lineDesc.LeftScript));
                                }
                                else
                                {
                                    (SLContext.Current.ScriptRegister.Values["__timeline__"] as List<string>).Add(lineDesc.LeftScript);
                                }
                            }
                            else
                            {
                                throw new NotSupportedException(string.Format("Only __main__ supports timeline. {0}", lineDesc.LeftScript));
                            }
                            break;
                        case SLLineType.Comment:
                            break;
                        case SLLineType.AppendScript:
                            AppendScript(lineDesc.LeftScript);
                            break;

                    }
                }
            }
        }
        public static void InitScriptLoader()
        {
            SLFunction mainFunc = new SLFunction();
            //__main__ cannot be endfunc
            mainFunc.Name = "__main__";
            definingFunc = "__main__";
            SLContext.Current.ScriptRegister.Values.Add(mainFunc.Name, mainFunc);
            List<string> timeline = new List<string>();
            timeline.Add("__default_timeline__");
            SLContext.Current.ScriptRegister.Values.Add("__timeline__", timeline);
           
        }

        public static void AppendScript(string file)
        {
            string x = System.IO.File.ReadAllText(file);
            SLLineLoader.ReadLinesFromScript(x);
        }


        public static SLLineDescription WhatLine(string line)
        {
            SLLineDescription sLLineDescription = new SLLineDescription();
            if (line.StartsWith("#"))
            {
                sLLineDescription.LineType = SLLineType.Comment;
                sLLineDescription.LeftScript = line;
                return sLLineDescription;
            }
            if (line.StartsWith("appendscript", StringComparison.OrdinalIgnoreCase))
            {
                sLLineDescription.LineType = SLLineType.AppendScript;
                // the space out of ` will be ignored.
                // but the space in ` will not be ignored.
                sLLineDescription.LeftScript = line.Replace("appendscript","", StringComparison.OrdinalIgnoreCase).Trim().Trim('`');
                return sLLineDescription;
            }
            if (line.StartsWith("--") && line.EndsWith("--"))
            {
                sLLineDescription.LineType = SLLineType.Timeline;
                sLLineDescription.LeftScript = line.Trim('-');
            }
            else
            {
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
                    sLLineDescription.LeftScript = line.Trim();

                }
            }
            return sLLineDescription;
        }

        public static SLLine LineToSLLine(string line, string func)
        {
            SLLine slLine = new SLLine();
            slLine.BelongToFunc = func;
            if (slLine.BelongToFunc == "__main__")
            {
                List<string> tl = (SLContext.Current.ScriptRegister.Values["__timeline__"] as List<string>);
                slLine.Timeline = tl.Last();
            }
            List<object> tmpParameters = new List<object>();
            bool isActionSet = false;
            bool isCoupleClosed = true;
            bool isRegsterNeed = false;
            bool ignoreNextSpace = false;
            StringBuilder buffer = new StringBuilder();
            foreach (char c in line)
            {
                switch (c)
                {
                    //line is trimmed.
                    case ' ':
                        if (ignoreNextSpace)
                        {
                            continue;
                        }
                        if (isRegsterNeed)
                        {
                            throw new NotSupportedException("only one register can be set one time.");
                        }
                        if (isActionSet)
                        {
                            if (isCoupleClosed)
                            {
                                tmpParameters.Add(buffer.ToString());
                                buffer.Clear();
                            }
                            else
                            {
                                buffer.Append(c);
                            }

                        }
                        else
                        {
                            slLine.Action = buffer.ToString();
                            isActionSet = true;
                            buffer.Clear();
                        }
                        ignoreNextSpace = true;
                        break;
                    case '`':
                        if (isActionSet && !isRegsterNeed)
                        {
                            
                            if (isCoupleClosed)
                            {
                                //start new couple
                                isCoupleClosed = false;
                            }
                            else
                            {
                                //end couple
                                isCoupleClosed = true;
                            }
                        }
                        else
                        {
                            throw new NotSupportedException("` cannot be front of action or register.");
                        }
                        ignoreNextSpace = false;
                        break;
                    case '>':
                        if (isActionSet)
                        {
                            isRegsterNeed = true;
                        }
                        else
                        {
                            throw new NotSupportedException("action is needed.");
                        }
                        ignoreNextSpace = true;
                        break;
                    default:
                        buffer.Append(c);
                        ignoreNextSpace = false;
                        break;

                }
            }
            if (isActionSet)
            {
                if (isRegsterNeed)
                {
                    slLine.PipeToRegister = buffer.ToString();
                }
                else
                {
                    tmpParameters.Add(buffer.ToString());
                }
            }
            else
            {
                slLine.Action = buffer.ToString();
            }
            

            slLine.Parameters = tmpParameters.ToArray<object>();
            return slLine;
        }
    }
}
