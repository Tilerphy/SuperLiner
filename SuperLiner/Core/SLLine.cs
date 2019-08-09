using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace SuperLiner.Core
{
    public class SLLine
    {
        public string Action { get; set; }
        public string PipeToRegister { get; set; }
        public object[] Parameters { get; set; }
        public string Origin { get; set; }
        public string Timeline { get; set; }
        public List<string> RunAt { get; set; }
        public string BelongToFunc { get; set; }
        public void Execute()
        {
            string currentTimeline = SLContext.Current.RuntimeRegister.Values[Contants.Current_Timeline_Key].ToString();
            string stopTimeline = SLContext.Current.RuntimeRegister.Values[Contants.Stop_Timeline_Key].ToString();
            List<string> tllist = (SLContext.Current.ScriptRegister.Values[Contants.Timeline_List_Key] as List<string>);
            if (this.BelongToFunc!=Contants.Main_Func_Key || (tllist.IndexOf(this.Timeline) >= tllist.IndexOf(currentTimeline)
                            && (stopTimeline == Contants.Default_Stop_Timeline || tllist.IndexOf(this.Timeline) < tllist.IndexOf(stopTimeline))))
            {
                if (this.RunAt != null && this.RunAt.Count > 0)
                {
                    this.RemoteExecute();
                }
                else
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
            }
            else
            {
                //Console.WriteLine("ignore {0}-{1}", this.Timeline, this.Action);
            }
        }

        protected void RemoteExecute()
        {
            foreach (string str in RunAt)
            {
                string ip = str.Trim();
                this.RemoteExecute(ip);
            }
        }

        public void RemoteExecute(string ip)
        {
            try
            {
                int port = int.Parse(SLContext.Current.RuntimeRegister.Values[string.Format(Contants.Slaver_Port_Key_Template, ip)].ToString());
                string secure = SLContext.Current.RuntimeRegister.Values[string.Format(Contants.Slaver_Secure_Key_Template, ip)].ToString();
                TcpClient client = new TcpClient(ip, port);
                Stream writer = client.GetStream();
                byte[] buffer = Encoding.UTF8.GetBytes(this.Origin.Split('@')[0]);
                RijndaelManaged rm = new RijndaelManaged
                {
                    Key = Encoding.UTF8.GetBytes(secure.PadRight(24,'#')),
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };
                byte[] outBuffer = rm.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length);
                writer.Write(outBuffer);
                writer.Flush();
                client.Dispose();
            }
            catch (Exception e)
            {
                //TODO: LOG
                //Console.WriteLine("");
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
                if (paramString.StartsWith(Contants.Op.Op_Get_From_Register))
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
