using System;
using System.Collections.Generic;
using System.Text;

namespace SuperLiner.Core
{
    public class SLRegister
    {
        public string Name { get; set; }
        public Dictionary<string, object> Values { get; private set; }

        public SLRegister()
        {
            this.Values = new Dictionary<string, object>();
        }

        public T Get<T>(string rKey) where T: class
        {
            if (this.Values.ContainsKey(rKey))
            {
                return (T)this.Values[rKey];
            }
            else
            {
                return default(T);
            }
        }
    }
}
