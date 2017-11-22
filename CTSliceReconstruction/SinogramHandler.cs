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
    }
}
