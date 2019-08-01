using System;
using System.Collections.Generic;
using System.Text;

namespace SuperLiner.Core
{
    public class SLContext
    {
        public SLRegister ScriptRegister { get; set; }
        public SLRegister RuntimeRegister { get; set; }

        public ModManager Mods { get; private set; }

        public SLContext()
        {
            this.RuntimeRegister = new SLRegister() { Name = "__system_reg__"};
            this.ScriptRegister = new SLRegister() { Name = "__script_reg__"};
            this.Mods = ModManager.Init();
        }

        private static SLContext _current;
        public static SLContext Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new SLContext();
                }
                return _current;
                
            }
        }
    }
}
