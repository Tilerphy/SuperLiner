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

        [SLModAction("{times} {funcName}", "times")]
        public void Times(string times, string funcName)
        {
            int intTimes = int.Parse(times);
            for (int i = 0; i < intTimes; i++)
            {
                (SLContext.Current.ScriptRegister.Values[funcName] as SLFunction).Execute();
            }
        }

        [SLModAction("{ip} {port}, {secure}", "setslaver")]
        public void setslaverinfo(string ip, string port, string secure)
        {
            Dictionary<string,object> rv = SLContext.Current.RuntimeRegister.Values;
            string portKey = string.Format(Contants.Slaver_Port_Key_Template, ip);
            string secureKey = string.Format(Contants.Slaver_Secure_Key_Template, ip);
            if (rv.ContainsKey(portKey))
            {
                rv[portKey] = port;
            }
            else
            {
                rv.Add(portKey, port);
            }

            if (rv.ContainsKey(secureKey))
            {
                rv[secureKey] = secure;
            }
            else
            {
                rv.Add(secureKey, secure);
            }
        }


    }
}
