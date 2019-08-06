using SuperLiner.Actions;
using SuperLiner.Core;
using System;
using System.Collections.Generic;
using System.IO;
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
            name = name.ToLowerInvariant();
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
            name = name.ToLowerInvariant();
            this.ModActions.Add(name, action);
        }

        public void CheckAndAdd(params Type[] types)
        {
           
            foreach (Type t in types)
            {
                if (t.GetCustomAttribute<SLModAttribute>() != null)
                {
                    object instance = Activator.CreateInstance(t);
                    MethodInfo[] methods = t.GetMethods();
                    foreach (MethodInfo method in methods)
                    {
                        SLModActionAttribute actionAttr = method.GetCustomAttribute<SLModActionAttribute>();
                        if (actionAttr != null)
                        {
                            string name = actionAttr.ActionKey.ToLowerInvariant() ;
                            if (this.ModActions.ContainsKey(name))
                            {
                                Console.WriteLine("{0} is duplicated.", name);
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
                                this.AddModAction(name, new SLModAction(instance, method, pi));
                            }

                        }
                    }
                }
            }
        }
        /// <summary>
        /// Check the if the mod was supported.
        /// </summary>
        /// <param name="path"></param>
        public void CheckAndAdd(string path)
        { 
            Assembly abl = Assembly.LoadFrom(path);
            CheckAndAdd(abl.GetTypes());
        }
        public static ModManager Init()
        {
            ModManager modManager = new ModManager();
            DirectoryInfo runningFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            foreach (FileInfo file in runningFolder.EnumerateFiles())
            {
                if (file.Extension.Equals(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    modManager.CheckAndAdd(file.FullName);
                }
            }
            return modManager;
        }
    }
}
