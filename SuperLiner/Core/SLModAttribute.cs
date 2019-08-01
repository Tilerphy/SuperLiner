using System;
using System.Collections.Generic;
using System.Text;

namespace SuperLiner.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SLModAttribute: Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class SLModActionAttribute : Attribute
    {
        public string Template { get; set; }
        public string ActionKey { get; set; }
        public SLModActionAttribute(string template, string key)
        {
            this.Template = template;
            this.ActionKey = key;
        }
    }
}
