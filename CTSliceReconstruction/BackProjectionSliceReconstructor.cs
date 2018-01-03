using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    /// <summary>
    /// Slice reconstructor based on Backprojection method (also called Summation method)
    /// </summary>
    public class BackProjectionSliceReconstructor : SliceReconstructor
    {
        

        public BackProjectionSliceReconstructor(List<double[]> projections, double angleBetweenProjections, ProjectionHandler projectionHandler)
        {
            this.angleBetweenProjections = angleBetweenProjections;
            this.projections = projections;
            this.projectionHandler = projectionHandler;
        }

        /// <summary>
        /// Reconstructs the image from projections specified in constructor
        /// </summary>
        /// <param name="progressCounter"></param>
        /// <returns>Reconstructed image</returns>
        public GrayscaleBitmap Reconstruct(ProgressCounter progressCounter = null)
        {
            int size = projections[0].Length;
            GrayscaleBitmap bmp = new GrayscaleBitmap(size, size);


            for (int i = 0; i < projections.Count; i++)
            {
                double angle = angleBetweenProjections * i;

                //Extrude projection in given angle
                GrayscaleBitmap extrudedProjection = projectionHandler.ExtrudeProjection(projections[i], angle);

                //add given extruded projection to the result
                bmp += extrudedProjection;
                progressCounter?.AddStep();
            }

            bmp.Stretch();
            return bmp;
        }
    }
}
