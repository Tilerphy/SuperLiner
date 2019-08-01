using SuperLiner.Core;
using System;

namespace SuperLiner
{
    class Program
    {
        static void Main(string[] args)
        {
            SLContext c = SLContext.Current;
            SLLineLoader.ReadLinesFromSrcript("Func a\n Print c\nendfunc\nPrint b\n");
            Console.Read();
        }
    }
}
