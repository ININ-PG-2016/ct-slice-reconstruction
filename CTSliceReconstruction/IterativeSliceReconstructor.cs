using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public class IterativeSliceReconstructor
    {
        private List<double[]> projections;
        private double angleBetweenProjections;
        private ProjectionHandler projectionHandler;

        public IterativeSliceReconstructor(List<double[]> projections, double angleBetweenProjections, ProjectionHandler projectionHandler)
        {
            this.angleBetweenProjections = angleBetweenProjections;
            this.projections = projections;
            this.projectionHandler = projectionHandler;
        }

        private void performOneIteration(int angleIndex, GrayscaleBitmap bmp)
        {
            double[] projection = projectionHandler.CreateProjection(bmp, angleIndex * angleBetweenProjections);
            double[] error = new double[projection.Length];
            for (int i = 0; i < projection.Length; i++)
            {
                error[i] = projections[angleIndex][i] - projection[i];
            }
            GrayscaleBitmap errorBmp = projectionHandler.ExtrudeProjection(error, angleIndex * angleBetweenProjections);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    bmp[i, j] += errorBmp[i, j];
                }
            }
        }

        public GrayscaleBitmap Reconstruct(int iterationCount)
        {
            int size = projections[0].Length;
            GrayscaleBitmap bmp = new GrayscaleBitmap(size, size);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    bmp[i, j] = 0.5;
                }
            }
            for (int i = 0; i < iterationCount; i++)
            {
                Console.WriteLine("Iteration: " + i);
                performOneIteration(i % projections.Count, bmp);
            }
            return bmp;
        }
    }
}
