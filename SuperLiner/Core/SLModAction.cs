using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SuperLiner.Core
{
    public class SLModAction
    {
        private object Instance { get; set; }
        private ParameterInfo[] Parameters { get; set; }
        private MethodInfo Method { get; set; }

        public SLModAction(object instance, MethodInfo method, ParameterInfo[] parameters)
        {
            this.Instance = instance;
            this.Method = method;
            this.Parameters = parameters;
        }

        public object Execute(params object[] ps)
        {
            return this.Method.Invoke(this.Instance, ps);
        }
    }
}
