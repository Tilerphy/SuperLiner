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

        [SLModAction("{j}", "loadjson")]
        public void LoadJson(string json)
        {
            object o = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            Console.WriteLine(o.ToString());
        }
    }
}
