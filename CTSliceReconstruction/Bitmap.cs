using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public class Bitmap
    {
        public int Width
        {
            get;
        }

        public int Height
        {
            get;
        }

        public double this[int i, int j]
        {
            get { return 0.0; }
            set { }
        }

        public double this[Point point]
        {
            get { return 0.0; }
            set { }
        }
    }
}
