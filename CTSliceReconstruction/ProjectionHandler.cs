using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public abstract class ProjectionHandler
    {
        public List<double> GenerateProjections(Bitmap bmp, int sliceCount)
        {
            return null;
        }

        public double[] CreateProjection(Bitmap bmp, double angle)
        {
            return null;
        }

        public Bitmap ExtrudeProjection(double[] projection, double angle)
        {
            return null;
        }

        protected 
    }
}
