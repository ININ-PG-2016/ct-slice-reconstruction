using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public struct Point
    {
        public int i;
        public int j;

        public Point(int i, int j)
        {
            this.i = i;
            this.j = j;
        }

        public override string ToString()
        {
            return "[" + i + " ; " + j + "]";
        }
    }
}
