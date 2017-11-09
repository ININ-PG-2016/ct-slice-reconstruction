using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public abstract class ProjectionHandler
    {
        public List<double[]> GenerateProjections(GrayscaleBitmap bmp, int sliceCount)
        {
            if (sliceCount < 1)
            {
                throw new ArgumentOutOfRangeException("sliceCount", "sliceCount must be at least 1");
            }

            List<double[]> projections = new List<double[]>(sliceCount);

            double angleStep = 180.0 / sliceCount;

            for (int i = 0; i < sliceCount; i++)
            {
                double angle = i * angleStep;

                projections.Add(CreateProjection(bmp, angle));
            }

            return projections;
        }

        public double[] CreateProjection(GrayscaleBitmap bmp, double angle)
        {
            int n = bmp.Width;
            double[] projection = new double[n];

            for (int i = 0; i < n; i++)
            {
                projection[i] = calculateProjectionAtPosition(angle, bmp, i);
            }

            return projection;
        }

        private double calculateProjectionAtPosition(double angle, GrayscaleBitmap bmp, int position)
        {
            List<PixelInfo> line = generateLine(angle, bmp.Width, position);

            double result = 0.0;

            foreach(PixelInfo pixel in line)
            {
                result += pixel.weight * bmp[pixel.position];
            }

            return result;
        }

        public GrayscaleBitmap ExtrudeProjection(double[] projection, double angle)
        {
            GrayscaleBitmap bmp = new GrayscaleBitmap(projection.Length, projection.Length);
            bool[,] occupied = new bool[projection.Length, projection.Length];
            for (int i = 0; i < projection.Length; i++)
            {
                List<PixelInfo> line = generateLine(angle, projection.Length, i);
                double weightSum = 0;
                for (int j = 0; j < line.Count; j++)
                {
                    weightSum += line[j].weight;
                }
                for (int j = 0; j < line.Count; j++)
                {
                    Point position = line[j].position;
                    double relativeWeight = line[j].weight / weightSum;
                    double value = (relativeWeight * projection[i]) / line[j].weight;
                    if (occupied[position.i, position.j])
                    {
                        bmp[position] = (bmp[position] + value) / 2;
                    }
                    else
                    {
                        bmp[position] = value;
                    }
                    occupied[position.i, position.j] = true;
                }
            }
            return bmp;
        }

        public abstract List<PixelInfo> generateLine(double angle, int n, int position);
    }
}
