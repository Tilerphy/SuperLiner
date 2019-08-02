using SuperLiner.Core;
using System;

namespace SuperLiner
{
    class Program
    {
        static void Main(string[] args)
        {
            SLContext c = SLContext.Current;
            SLLineLoader.ReadLinesFromSrcript("Func a\n Print c\n Set hwllo => bingo\nendfunc\nCall a\nPrint b\nPrint &bingo");
            (c.ScriptRegister.Values["__main__"] as SLFunction).Execute();
            Console.Read();
        }
    }
}
