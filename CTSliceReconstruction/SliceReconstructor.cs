using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public  abstract class SliceReconstructor
    {
        protected List<double[]> projections;
        protected double angleBetweenProjections;
        protected ProjectionHandler projectionHandler;
    }
}
