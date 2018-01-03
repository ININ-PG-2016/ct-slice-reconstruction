using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    /// <summary>
    /// CT slice reconstructor algorithm
    /// </summary>
    public abstract class SliceReconstructor
    {
        /// <summary>
        /// List of input projections
        /// </summary>
        protected List<double[]> projections;

        /// <summary>
        /// Calculated angle between projections
        /// </summary>
        protected double angleBetweenProjections;

        /// <summary>
        /// Handler used for projection generating process and extrusion
        /// </summary>
        protected ProjectionHandler projectionHandler;
    }
}
