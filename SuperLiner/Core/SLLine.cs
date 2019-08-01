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
            object result = SLContext.Current.Mods.FindAndInvoke(Action, Parameters);
            var register = SLContext.Current.RuntimeRegister.Values;
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
}
