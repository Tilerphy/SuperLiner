using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
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
            TcpClient master = state as TcpClient;
            Stream stream = master.GetStream();
            StreamReader reader = new StreamReader(stream);
            string line = reader.ReadLine();
            if (line.StartsWith(this.Secure))
            {
                line = line.Substring(this.Secure.Length);
                SLLineDescription t = SLLineLoader.WhatLine(line);
                if (t.LineType == SLLineType.Line)
                {
                    SLLine slLine = SLLineLoader.LineToSLLine(line, "__slave_main__");
                    slLine.Execute();
                }
                else
                {
                    //TO DO:
                    //audit
                    Console.WriteLine("Not support this command in remote.");
                }
            }
            else
            {
                //TO DO:
                //audit
                Console.WriteLine("Secure error from {0}", master.Client.RemoteEndPoint.ToString());
                master.Dispose();
            }

        }
    }
}
