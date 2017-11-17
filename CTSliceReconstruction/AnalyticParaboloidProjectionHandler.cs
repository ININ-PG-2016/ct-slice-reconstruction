using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public class AnalyticParaboloidProjector
    {
        double ComputeIntegralValue(double y, double r)
        {
            return (1.0 / 6.0) * (y * Math.Sqrt(r * r - y * y) * (r * r + 2 * y * y) + 3 * r * r * r * r * Math.Atan(y / Math.Sqrt(r * r - y * y)));
        }

        double ComputeProjection(double r, double y1, double y2)
        {
            return ComputeIntegralValue(y2, r) - ComputeIntegralValue(y1, r);
        }

        public List<double[]> GenerateProjections(int bmpSize, int sliceCount)
        {
            if (sliceCount < 1)
            {
                throw new ArgumentOutOfRangeException("sliceCount", "sliceCount must be at least 1");
            }

            List<double[]> projections = new List<double[]>(sliceCount);

            double angleStep = 180.0 / sliceCount;
            double[] projection = CreateProjection(bmpSize);

            for (int i = 0; i < sliceCount; i++)
            {
                double angle = i * angleStep;
                projections.Add(projection);
            }

            return projections;
        }

        public double[] CreateProjection(int bmpSize)
        {
            double[] projection = new double[bmpSize];

            for (int i = 0; i < bmpSize; i++)
            {
                projection[i] = calculateProjectionAtPosition(bmpSize, i);
            }

            return projection;
        }

        private double calculateProjectionAtPosition(int bmpSize, int position)
        {
            double stripWidth = 1.0 / (bmpSize / 2.0);
            double begin = -1 + position * stripWidth;
            double end = -1 + (position + 1) * stripWidth;
            return ComputeProjection(1, begin, end) * (bmpSize / 2.0) * (bmpSize / 2.0);
        }
    }
}
