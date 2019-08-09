using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace SuperLiner.Core
{
    public class SLSlaveListener
    {
        public TcpListener Listener { get; set; }
        public string Secure { get; set; }
        public SLSlaveListener(string ip, int port, string secure)
        {
            this.Secure = secure;
            this.Listener = new TcpListener(IPAddress.Parse(ip), port);
            this.Listener.Start();
            while (true)
            {
                TcpClient master = this.Listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(new WaitCallback(HandlingMaster), master);
            }
        }

        private void HandlingMaster(object state)
        {
            try
            {
                TcpClient master = state as TcpClient;
                Stream stream = master.GetStream();
                using (MemoryStream mStream = new MemoryStream())
                {
                    byte[] buffer = new byte[10240];
                    int realRead = 0;
                    while ((realRead = stream.Read(buffer)) > 0)
                    {
                        mStream.Write(buffer, 0, realRead);
                    }

                    RijndaelManaged rm = new RijndaelManaged
                    {
                        Key = Encoding.UTF8.GetBytes(Secure.PadRight(24, '#')),
                        Mode = CipherMode.ECB,
                        Padding = PaddingMode.PKCS7
                    };

                    byte[] plainTextBuffer = rm.CreateDecryptor().TransformFinalBlock(mStream.ToArray(), 0, (int)mStream.Length);
                    string line = Encoding.UTF8.GetString(plainTextBuffer);
                    SLLineDescription t = SLLineLoader.WhatLine(line);
                    if (t.LineType == SLLineType.Line)
                    {
                        SLLine slLine = SLLineLoader.LineToSLLine(line, "__slaver_main__");
                        slLine.Execute();
                    }
                    else
                    {
                        //TO DO:
                        //audit
                        Console.WriteLine("Not support this command in remote.");
                    }
                }
            }
            catch (Exception e)
            {
                //TODO: Log
                //do nothing now.
            }
            

        }
    }
}
