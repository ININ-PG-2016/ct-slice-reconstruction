using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public class SinogramHandler
    {
        public static List<double[]> SinogramToProjections(GrayscaleBitmap bmp)
        {
            int projectionCount = bmp.Height / 3;
            int projectionSize = bmp.Width;

            List<double[]> projections = new List<double[]>(projectionCount);

            for (int i = 0; i < projectionCount; i++)
            {
                double[] projection = new double[projectionSize];

                for (int j = 0; j < projectionSize; j++)
                {
                    projection[j] = bmp[projectionCount + i, j];
                }

                projections.Add(projection);
            }

            return projections;
        }

        public static GrayscaleBitmap ProjectionsToSinogram(List<double[]> projections)
        {
            int projectionCount = projections.Count;

            int projectionSize = projections[0].Length;

            GrayscaleBitmap sinogram = new GrayscaleBitmap(projectionSize, 3 * projectionCount);

            bool reversed = true;

            for (int k = 0; k < 3; k++)
            {
                for (int i = 0; i < projectionCount; i++)
                {
                    for (int j = 0; j < projectionSize; j++)
                    {
                        int xIndex = j;

                        if (reversed)
                        {
                            xIndex = projectionSize - 1 - j;
                        }

                        sinogram[k * projectionCount + i, xIndex] = projections[i][j];
                    }
                }

                reversed ^= true;
            }

            return sinogram;
        }
    }
}
