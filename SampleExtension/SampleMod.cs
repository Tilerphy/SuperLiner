using SuperLiner.Core;
using System;

namespace SampleExtension
{
    [SLMod]
    public class SampleMod
    {
        [SLModAction("", "woo")]
        public void Woo()
        {
            Console.WriteLine("Woo!");
        }
    }
}
