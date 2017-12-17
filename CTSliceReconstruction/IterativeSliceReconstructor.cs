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
        private GrayscaleBitmap currentReconstruction = null;
        private int lastIterationNumber = 0;

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

        public GrayscaleBitmap Reconstruct(int iterationCount, ProgressCounter progressCounter = null)
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
                lastIterationNumber = i;
                progressCounter?.AddStep();
            }
            GrayscaleBitmap ret = new GrayscaleBitmap(bmp.Width, bmp.Height);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    if (bmp[i, j] < 0)
                        ret[i, j] = 0;
                    else if (bmp[i, j] > 1)
                        ret[i, j] = 1;
                    else
                        ret[i, j] = bmp[i, j];
                }
            }
            currentReconstruction = bmp;
            return ret;
        }

        public GrayscaleBitmap PerformAdditionalIterations(int iterationCount)
        {
            if (currentReconstruction == null)
                throw new NullReferenceException("The Reconstruct method must be called first");
            for (int i = 0; i < iterationCount; i++)
            {
                lastIterationNumber++;
                Console.WriteLine("Iteration: " + lastIterationNumber);
                performOneIteration(lastIterationNumber % projections.Count, currentReconstruction);
            }
            GrayscaleBitmap ret = new GrayscaleBitmap(currentReconstruction.Width, currentReconstruction.Height);
            for (int i = 0; i < currentReconstruction.Width; i++)
            {
                for (int j = 0; j < currentReconstruction.Height; j++)
                {
                    if (currentReconstruction[i, j] < 0)
                        ret[i, j] = 0;
                    else if (currentReconstruction[i, j] > 1)
                        ret[i, j] = 1;
                    else
                        ret[i, j] = currentReconstruction[i, j];
                }
            }
            return ret;
        }
    }
}
