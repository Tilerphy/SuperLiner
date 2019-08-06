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

        public string Timeline { get; set; }
        public List<string> RunAt { get; set; }
        public string BelongToFunc { get; set; }
        public void Execute()
        {
            string currentTimeline = SLContext.Current.RuntimeRegister.Values["__current_timeline__"].ToString();
            string stopTimeline = SLContext.Current.RuntimeRegister.Values["__stop_timeline__"].ToString();
            List<string> tllist = (SLContext.Current.ScriptRegister.Values["__timeline__"] as List<string>);
            if (this.BelongToFunc!="__main__" || (tllist.IndexOf(this.Timeline) >= tllist.IndexOf(currentTimeline)
                            && (stopTimeline == "__default_stop_timeline__" || tllist.IndexOf(this.Timeline) < tllist.IndexOf(stopTimeline))))
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
            else
            {
                //Console.WriteLine("ignore {0}-{1}", this.Timeline, this.Action);
            }
        }

        /// <summary>
        /// Put all pairs together between "
        /// </summary>
        protected void AssembleParameters()
        {

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
