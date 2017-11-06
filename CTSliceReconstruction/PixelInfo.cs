using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public class PixelInfo
    {
        public Point position;
        public double weight;

        public PixelInfo(int i, int j, double weight)
        {
            this.position = new Point(i, j);
            this.weight = weight;
        }
    }
}
