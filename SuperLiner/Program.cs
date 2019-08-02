using SuperLiner.Core;
using System;

namespace SuperLiner
{
    class Program
    {
        static void Main(string[] args)
        {
            SLContext c = SLContext.Current;
            string x = System.IO.File.ReadAllText(@"C:\Users\Administrator\Desktop\scripthere.txt");
            SLLineLoader.ReadLinesFromSrcript(x);
            (c.ScriptRegister.Values["__main__"] as SLFunction).Execute();
            Console.Read();
        }
    }
}
