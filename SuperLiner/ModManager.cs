using SuperLiner.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SuperLiner
{
    public class ModManager
    {
        public Dictionary<string, SLModAction> ModActions { get; private set; }
        private bool IgnoreLoadError { get; set; }
        private ModManager(bool ignoreLoadError = false)
        {
            this.IgnoreLoadError = ignoreLoadError;
            this.ModActions = new Dictionary<string, SLModAction>();
        }

        public object FindAndInvoke(string name, params object[] ps)
        {
            SLModAction action = this.ModActions.ContainsKey(name) ? this.ModActions[name] : null;
            if (action != null)
            {
                return action.Execute(ps);
            }
            else
            {
                throw new NotSupportedException(string.Format("No such action: {0}", name));
            }
        }
        public void AddModAction(string name, SLModAction action)
        {
            this.ModActions.Add(name, action);
        }

        /// <summary>
        /// Check the if the mod was supported.
        /// </summary>
        /// <param name="path"></param>
        public void CheckAndAdd(string path)
        {
            Assembly abl = Assembly.LoadFile(path);
            Type[] ts = abl.GetTypes();
            foreach (Type t in ts)
            {
                if (t.GetCustomAttribute<SLModAttribute>() != null)
                {
                    object instance = Activator.CreateInstance(t);
                    MethodInfo[] methods = t.GetMethods(BindingFlags.Public);
                    foreach (MethodInfo method in methods)
                    {
                        SLModActionAttribute actionAttr =method.GetCustomAttribute<SLModActionAttribute>();
                        if ( actionAttr != null)
                        {
                            if (this.ModActions.ContainsKey(actionAttr.ActionKey))
                            {
                                Console.WriteLine("{0} is duplicated.", actionAttr.ActionKey, path);
                                if (this.IgnoreLoadError)
                                {
                                    continue;
                                }
                                else
                                {
                                    break;
                                }
                                
                            }
                            else
                            {
                                ParameterInfo[] pi = method.GetParameters();
                                this.AddModAction(actionAttr.ActionKey, new SLModAction(instance, method, pi));
                            }
                           
                        }
                    }
                }
            }
        }
        public static ModManager Init()
        {
            return new ModManager();
        }
    }
}
