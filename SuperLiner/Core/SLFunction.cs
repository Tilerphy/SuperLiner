using System;
using System.Collections.Generic;
using System.Text;

namespace SuperLiner.Core
{
    public class SLFunction
    {
        public string Name { get; set; }
        private List<SLLine> Lines { get; }
        public SLFunction()
        {
            this.Lines = new List<SLLine>();
        }

        public void AppendLine(SLLine slLine)
        {
            this.Lines.Add(slLine);
        }

        public void Execute()
        {
            for (int i=0;i<Lines.Count;i++)
            {
                Lines[i].Execute();
            }
        }
    }
}
