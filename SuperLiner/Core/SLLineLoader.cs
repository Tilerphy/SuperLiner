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
        private static string definingFunc = Contants.Main_Func_Key;

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
                            if (lineDesc.LeftScript.Equals(Contants.Main_Func_Key, StringComparison.OrdinalIgnoreCase))
                            {
                                throw new NotSupportedException(string.Format("Cannot define a function named {0}", Contants.Main_Func_Key));
                            }
                            if (definingFunc != Contants.Main_Func_Key)
                            {
                                throw new NotSupportedException(string.Format("Cannot define a nested function. {0}", definingFunc));
                            }
                            SLContext.Current.ScriptRegister.Values.Add(lineDesc.LeftScript, new SLFunction() { Name = lineDesc.LeftScript });
                            definingFunc = lineDesc.LeftScript;
                            break;
                        case SLLineType.EndingFunc:
                            if (definingFunc == Contants.Main_Func_Key)
                            {
                                throw new NotSupportedException(string.Format("Cannot end function __main__"));
                            }
                            definingFunc = Contants.Main_Func_Key;
                            break;
                        case SLLineType.NotSupport:
                            throw new NotSupportedException();
                        case SLLineType.Timeline:

                            if (definingFunc == Contants.Main_Func_Key)
                            {
                                List<string> tl = (SLContext.Current.ScriptRegister.Values[Contants.Timeline_List_Key] as List<string>);
                                if (tl.Contains(lineDesc.LeftScript))
                                {
                                    throw new NotSupportedException(string.Format("Cannot create same timeline in __main__. {0}", lineDesc.LeftScript));
                                }
                                else
                                {
                                    (SLContext.Current.ScriptRegister.Values[Contants.Timeline_List_Key] as List<string>).Add(lineDesc.LeftScript);
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
            mainFunc.Name = Contants.Main_Func_Key;
            definingFunc = Contants.Main_Func_Key;
            SLContext.Current.ScriptRegister.Values.Add(mainFunc.Name, mainFunc);
            List<string> timeline = new List<string>();
            timeline.Add(Contants.Default_Timeline);
            SLContext.Current.ScriptRegister.Values.Add(Contants.Timeline_List_Key, timeline);
           
        }

        public static void AppendScript(string file)
        {
            string x = System.IO.File.ReadAllText(file);
            SLLineLoader.ReadLinesFromScript(x);
        }


        public static SLLineDescription WhatLine(string line)
        {
            SLLineDescription sLLineDescription = new SLLineDescription();
            if (line.StartsWith(Contants.Op.Op_Comment_It))
            {
                sLLineDescription.LineType = SLLineType.Comment;
                sLLineDescription.LeftScript = line;
                return sLLineDescription;
            }
            if (line.StartsWith(Contants.Op.Op_Append_Script, StringComparison.OrdinalIgnoreCase))
            {
                sLLineDescription.LineType = SLLineType.AppendScript;
                // the space out of ` will be ignored.
                // but the space in ` will not be ignored.
                sLLineDescription.LeftScript = line.Replace(Contants.Op.Op_Append_Script, "", StringComparison.OrdinalIgnoreCase).Trim().Trim('`');
                return sLLineDescription;
            }
            if (line.StartsWith(Contants.Op.Op_Quote_Timeline) && line.EndsWith(Contants.Op.Op_Quote_Timeline))
            {
                sLLineDescription.LineType = SLLineType.Timeline;
                sLLineDescription.LeftScript = line.Replace("--","", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                if (line.StartsWith(Contants.Op.Op_Start_Define_Func, StringComparison.OrdinalIgnoreCase))
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
                else if (line.Equals(Contants.Op.Op_End_Define_Func, StringComparison.OrdinalIgnoreCase))
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
            line = line.Trim();
            SLLine slLine = new SLLine();
            slLine.BelongToFunc = func;
            slLine.Origin = line;
            if (slLine.BelongToFunc == Contants.Main_Func_Key)
            {
                List<string> tl = (SLContext.Current.ScriptRegister.Values[Contants.Timeline_List_Key] as List<string>);
                slLine.Timeline = tl.Last();
            }
            List<object> tmpParameters = new List<object>();
            bool isActionSet = false;
            bool isCoupleClosed = true;
            bool isRegsterNeed = false;
            bool ignoreNextSpace = false;
            bool isMarkingRemoteLine = false;
            bool isRegisterDefined = true;
            StringBuilder buffer = new StringBuilder();
            foreach (char c in line)
            {
                switch (c)
                {
                    case '@':
                        if (!isCoupleClosed)
                        {
                            buffer.Append(c);
                            ignoreNextSpace = true;
                            continue;
                        }    
                        if (isActionSet && isCoupleClosed && isRegisterDefined)
                        {
                            if (isMarkingRemoteLine)
                            {
                                throw new NotSupportedException("Cannot use @ after @");
                            }
                            else
                            {
                                isMarkingRemoteLine = true;
                                slLine.RunAt = new List<string>();
                                ignoreNextSpace = true;
                            }
                        }
                        else
                        {
                            throw new NotSupportedException("The remote mark must be defined after Action command");
                        }
                       
                        break;

                    //line is trimmed.
                    case ' ':
                        if (!isCoupleClosed)
                        {
                            buffer.Append(c);
                            ignoreNextSpace = true;
                            continue;
                        }
                        if (ignoreNextSpace)
                        {
                            continue;
                        }

                        
                        if (isRegsterNeed && !isRegisterDefined)
                        {
                            isRegisterDefined = true;
                            slLine.PipeToRegister = buffer.ToString();
                            buffer.Clear();
                        }
                        else
                        {
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
                        }
                       
                        ignoreNextSpace = true;
                        break;
                    case '`':
                        //if (isMarkingRemoteLine)
                        //{
                        //    throw new NotSupportedException("Cannot use ` after @");
                        //}
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
                        if (!isCoupleClosed)
                        {
                            buffer.Append(c);
                            ignoreNextSpace = true;
                            continue;
                        }
                        if (isMarkingRemoteLine)
                        {
                            throw new NotSupportedException("Cannot use > after @");
                        }
                        if (isActionSet)
                        {
                            isRegsterNeed = true;
                            isRegisterDefined = false;
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
                //register tail
                if (isRegsterNeed)
                {
                    isRegisterDefined = true;
                    slLine.PipeToRegister = buffer.ToString();
                }
                //remote mark tail
                else if (isMarkingRemoteLine)
                {
                    slLine.RunAt.AddRange(buffer.ToString().Trim().Split(',', StringSplitOptions.RemoveEmptyEntries));
                }
                //normal tail
                else
                {
                    tmpParameters.Add(buffer.ToString());
                }
            }
            else
            {
                //only action tail
                slLine.Action = buffer.ToString();
            }


            slLine.Parameters = tmpParameters.ToArray<object>();
            return slLine;
        }
    }
}
