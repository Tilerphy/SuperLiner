using System;
using System.Collections.Generic;
using System.Text;

namespace SuperLiner.Core
{
    public class SLLine
    {
        public string Action { get; set; }
        public string PipeToRegister { get; set; }
        public object[] Parameters { get; set; }

        public void Execute()
        {
            LoadRegisterValue();
            object result = SLContext.Current.Mods.FindAndInvoke(Action, Parameters);
            var register = SLContext.Current.RuntimeRegister.Values;
            if (!string.IsNullOrEmpty(PipeToRegister))
            {
                if (register.ContainsKey(PipeToRegister))
                {
                    register[PipeToRegister] = result;
                }
                else
                {
                    register.Add(PipeToRegister, result);
                }
            }
        }

        protected void LoadRegisterValue()
        {
            int index = 0;
            for (index = 0; index < Parameters.Length; index++)
            {
                string paramString = Parameters[index].ToString();
                if (paramString.StartsWith("&"))
                {
                    string regName = paramString.Substring(1);
                    if (SLContext.Current.RuntimeRegister.Values.ContainsKey(regName))
                    {
                        Parameters[index] = SLContext.Current.RuntimeRegister.Values[regName];
                    }
                    else
                    {
                        Parameters[index] = null;
                    }
                   
                }
            }

        }
    }
}
