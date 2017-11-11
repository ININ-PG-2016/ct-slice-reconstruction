using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public class BackProjectionSliceReconstructor
    {
        private List<double[]> projections;
        private double angleBetweenProjections;
        private ProjectionHandler projectionHandler;

        public BackProjectionSliceReconstructor(List<double[]> projections, double angleBetweenProjections, ProjectionHandler projectionHandler)
        {
            this.angleBetweenProjections = angleBetweenProjections;
            this.projections = projections;
            this.projectionHandler = projectionHandler;
        }

        public GrayscaleBitmap Reconstruct()
        {
            int size = projections[0].Length;
            GrayscaleBitmap bmp = new GrayscaleBitmap(size, size);


            for (int i = 0; i < projections.Count; i++)
            {
                double angle = angleBetweenProjections * i;

                GrayscaleBitmap extrudedProjection = projectionHandler.ExtrudeProjection(projections[i], angle);

                bmp += extrudedProjection;
            }

            bmp.Stretch();
            return bmp;
        }
    }
}
