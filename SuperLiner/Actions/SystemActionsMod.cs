using SuperLiner.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml;

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

        [SLModAction("{funcName} {iplist}", "RemoteCall")]
        public void RemoteCall(string funcName, string ipList)
        {
            string[] ips = ipList.Split(",", StringSplitOptions.RemoveEmptyEntries);
            SLFunction slFunc = (SLContext.Current.ScriptRegister.Values[funcName] as SLFunction);
            foreach (SLLine  slLine in slFunc.Lines)
            {
                foreach (string ip in ips)
                {
                    slLine.RemoteExecute(ip.Trim());
                }
            }
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

        [SLModAction("", "pause")]
        public void Pause()
        {
            Console.ReadLine();
        }

        [SLModAction("{ip} {port}, {secure}", "setslaver")]
        public void setslaverinfo(string ip, string port, string secure)
        {
            Dictionary<string,object> rv = SLContext.Current.RuntimeRegister.Values;
            string portKey = string.Format(Constants.Slaver_Port_Key_Template, ip);
            string secureKey = string.Format(Constants.Slaver_Secure_Key_Template, ip);
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

        [SLModAction("{command} {args}", "cmd")]
        public void Cmd(string command, string args)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.FileName = command;
            Process ps = Process.Start(info);
            ps.StandardInput.WriteLine(args);

        }

        [SLModAction("{xml} {xpath} {newval}", "changexml")]
        public void ChangeXml(string xmlFile, string xpath, string newVal)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            XmlNodeList nodes = doc.SelectNodes(xpath);
            foreach (XmlNode node in nodes)
            {
                node.Value = newVal;
            }
            doc.Save(xmlFile);
        }

        [SLModAction("{key} {val}", "sethttpheader")]
        public void SetHttpHeader(string key, string val)
        {
            Dictionary<string, string> headers = null;
            if (!SLContext.Current.RuntimeRegister.Values.ContainsKey(Constants.Http_Header_Key))
            {
                SLContext.Current.RuntimeRegister.Values.Add(Constants.Http_Header_Key, new Dictionary<string,string>());
                
            }
            headers = SLContext.Current.RuntimeRegister.Values[Constants.Http_Header_Key] as Dictionary<string, string>;
            headers.Add(key, val);

        }

        [SLModAction("", "cleanhttpheader")]
        public void CleanHttpHeader()
        {
            if (SLContext.Current.RuntimeRegister.Values.ContainsKey(Constants.Http_Header_Key))
            {
                SLContext.Current.RuntimeRegister.Values.Remove(Constants.Http_Header_Key);
            }
          
        }

        [SLModAction("{url} {output}", "download")]
        public void Download(string url, string output)
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            if (SLContext.Current.RuntimeRegister.Values.ContainsKey(Constants.Http_Header_Key))
            {
                headers = SLContext.Current.RuntimeRegister.Values[Constants.Http_Header_Key] as Dictionary<string, string>;
            }
            foreach (string key in headers.Keys)
            {
                webrequest.Headers.Add(key, headers[key]);
            }
            WebResponse resp = webrequest.GetResponse();
            using (FileStream fs = new FileStream(output, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[20480];
                using (Stream stream = resp.GetResponseStream())
                {
                    int realRead = 0;
                    while ((realRead = stream.Read(buffer))> 0)
                    {
                        fs.Write(buffer, 0 ,realRead);
                    }
                }
            }

            resp.Dispose();
            
        }

    }
}
