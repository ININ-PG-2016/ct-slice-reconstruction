using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public interface Filter2D
    {
        void Apply(GrayscaleBitmap bmp);
    }

    public class ConvolutionFilter2D : Filter2D
    {
        private double[,] filterValues;
        int originI;
        int originJ;

        public ConvolutionFilter2D(double[,] filterValues, int originI, int originJ)
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

        public static ConvolutionFilter2D GetGauss55()
        {
            return new ConvolutionFilter2D
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

        public static ConvolutionFilter2D GetLaplace()
        {
            return new ConvolutionFilter2D
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

        public static ConvolutionFilter2D GetRoberts1()
        {
            return new ConvolutionFilter2D(
                    new double[,] { { 1.0, 0.0 }, { 0.0, -1.0 } },
                    0,0
                );
        }

        public static ConvolutionFilter2D GetRoberts2()
        {
            return new ConvolutionFilter2D(
                    new double[,] { { 0.0, 1.0 }, { -1.0, -0.0 } },
                    0, 0
                );
        }
    }

    public class EdgeDetectorRoberts : Filter2D
    {
        private static EdgeDetectorRoberts INSTANCE = new EdgeDetectorRoberts();

        public static EdgeDetectorRoberts Instance {
            get { return INSTANCE; }
        }

        private EdgeDetectorRoberts() { }

        private Filter2D roberts1 = ConvolutionFilter2D.GetRoberts1();
        private Filter2D roberts2 = ConvolutionFilter2D.GetRoberts2();

        public void Apply(GrayscaleBitmap bmp)
        {
            GrayscaleBitmap robertsBmp1 = bmp.Copy();
            GrayscaleBitmap robertsBmp2 = bmp.Copy();

            roberts1.Apply(robertsBmp1);
            roberts2.Apply(robertsBmp2);

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    bmp[i, j] = Math.Abs(robertsBmp1[i, j]) + Math.Abs(robertsBmp2[i, j]);
                }
            }
        }
    }
}
