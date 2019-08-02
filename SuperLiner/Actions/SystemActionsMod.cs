using SuperLiner.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperLiner.Actions
{
    [SLMod]
    public class  SystemActionsMod
    {
        [SLModAction("{p1}", "print")]
        public void Print(string p1)
        {
            Console.WriteLine(p1);
        }

        [SLModAction("{val}", "set")]
        public object Set(object val)
        {
            return val;
        }

        [SLModAction("{funcName}", "call")]
        public void Call(string funcName)
        {
            (SLContext.Current.ScriptRegister.Values[funcName] as SLFunction).Execute();
        }
    }
}
