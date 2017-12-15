using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public class IterativeSliceReconstructor : SliceReconstructor
    {
        private bool allowNegativeValues;

        public IterativeSliceReconstructor(List<double[]> projections, double angleBetweenProjections, ProjectionHandler projectionHandler, bool allowNegativeValues = true)
        {
            this.angleBetweenProjections = angleBetweenProjections;
            this.projections = projections;
            this.projectionHandler = projectionHandler;
            this.allowNegativeValues = allowNegativeValues;
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
                    if (!allowNegativeValues && bmp[i, j] < 0)
                        bmp[i, j] = 0;
                }
            }
        }

        public GrayscaleBitmap Reconstruct(int iterationCount)
        {
            int size = projections[0].Length;
            double avrg = 0;
            for (int i = 0; i < projections[0].Length; i++)
                avrg += projections[0][i];
            avrg /= (size * size);
            GrayscaleBitmap bmp = new GrayscaleBitmap(size, size);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    bmp[i, j] = avrg;
                }
            }
            for (int i = 0; i < iterationCount; i++)
            {
                Console.WriteLine("Iteration: " + i);
                performOneIteration(i % projections.Count, bmp);
            }
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    if (bmp[i, j] < 0)
                        bmp[i, j] = 0;
                    if (bmp[i, j] > 1)
                        bmp[i, j] = 1;
                }
            }
            return bmp;
        }
    }
}
