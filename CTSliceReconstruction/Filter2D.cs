using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public class Filter2D
    {
        private double[,] filterValues;
        int originI;
        int originJ;

        public Filter2D(double[,] filterValues, int originI, int originJ)
        {
            this.filterValues = filterValues;
            this.originI = originI;
            this.originJ = originJ;
        }
        private double applyToPosition(GrayscaleBitmap bmp, int i, int j)
        {
            double sum = 0.0;

            for (int k = 0; k < filterValues.GetLength(0); k++)
            {
                int indexI = i - originI + k;

                for (int l = 0; l < filterValues.GetLength(1); l++)
                {
                    int indexJ = j - originJ + l;

                    sum += (filterValues[k,l] * bmp[indexI, indexJ]);
                }
            }

            return sum;
        }

        public void Apply(GrayscaleBitmap bmp)
        {
            double[,] newValues = new double[bmp.Height, bmp.Width];
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    newValues[i,j] = applyToPosition(bmp, i, j);
                }
            }

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    bmp[i, j] = newValues[i, j];
                }
            }
        }

        public static Filter2D GetGauss55()
        {
            return new Filter2D
                (
                    new double[,] 
                    { 
                        { 0.0037, 0.0148, 0.0259, 0.0148, 0.0037 },
                        { 0.0148, 0.0592, 0.0962, 0.0592, 0.0148 },
                        { 0.0259, 0.0962, 0.1517, 0.0962, 0.0259 },
                        { 0.0148, 0.0592, 0.0962, 0.0592, 0.0148 },
                        { 0.0037, 0.0148, 0.0259, 0.0148, 0.0037 }
                    },
                    2,2
                );
        }

        public static Filter2D GetLaplace()
        {
            return new Filter2D
                (
                    new double[,]
                    {
                        { -1, -1, -1 },
                        { -1,  8, -1 },
                        { -1, -1, -1 }
                    },
                    1, 1
                );
        }
    }
}
