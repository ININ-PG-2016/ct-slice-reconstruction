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
        string name;

        public ConvolutionFilter2D(double[,] filterValues, int originI, int originJ, string name)
        {
            this.filterValues = filterValues;
            this.originI = originI;
            this.originJ = originJ;
            this.name = name;
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
                    bmp[i, j] = Math.Abs(newValues[i, j]);
                }
            }
        }

        public override string ToString()
        {
            return name;
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
                    2,2,
                    "Gaussian (5x5)"
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
                    1, 1,
                    "Laplacian (3x3)"
                );
        }

        public static ConvolutionFilter2D GetSharpen()
        {
            return new ConvolutionFilter2D(
                new double[,] { 
                    {  0, -1,  0 }, 
                    { -1,  5, -1 },
                    {  0, -1,  0 }
                },
                1,1,
                "Laplacian Sharpening (3x3)"
            );
        }

        public static ConvolutionFilter2D GetRoberts1()
        {
            return new ConvolutionFilter2D(
                    new double[,] { { 1.0, 0.0 }, { 0.0, -1.0 } },
                    0,0,
                    "Roberts 1"
                );
        }

        public static ConvolutionFilter2D GetRoberts2()
        {
            return new ConvolutionFilter2D(
                    new double[,] { { 0.0, 1.0 }, { -1.0, -0.0 } },
                    0, 0,
                    "Roberts 2"
                );
        }

        public static ConvolutionFilter2D GetKirsch1()
        {
            return new ConvolutionFilter2D(
                new double[,] {
                    {  5,  5,  5 },
                    { -3,  0, -3 },
                    { -3, -3, -3 }
                },
                1, 1,
                "Kirsch 1"
            );
        }

        public static ConvolutionFilter2D GetKirsch2()
        {
            return new ConvolutionFilter2D(
                new double[,] {
                    { -3, -3,  5 },
                    { -3,  0,  5 },
                    { -3, -3,  5 }
                },
                1, 1,
                "Kirsch 2"
            );
        }

        public static ConvolutionFilter2D GetLaplacianOfGaussian55()
        {
            return new ConvolutionFilter2D
                (
                    new double[,]
                    {
                        {   0,   0,   1,   0,   0 },
                        {   0,   1,   2,   1,   0 },
                        {   1,   2, -16,   2,   1 },
                        {   0,   1,   2,   1,   0 },
                        {   0,   0,   1,   0,   0 }
                    },
                    2, 2,
                    "Laplacian of Gaussian (5x5)"
                );
        }

        public static ConvolutionFilter2D GetLaplacianOfGaussian77()
        {
            return new ConvolutionFilter2D
                (
                    new double[,]
                    {
                        {   0,   0,   1,   1,   1,   0,   0 },
                        {   0,   1,   3,   3,   3,   1,   0 },
                        {   1,   3,   0,  -7,   0,   3,   1 },
                        {   1,   3,  -7, -24,  -7,   3,   1 },
                        {   1,   3,   0,  -7,   0,   3,   1 },
                        {   0,   1,   3,   3,   3,   1,   0 },
                        {   0,   0,   1,   1,   1,   0,   0 }
                    },
                    3, 3,
                    "Laplacian of Gaussian (7x7)"
                );
        }
    }

    public class CompositeConvolutionFilter2D : Filter2D
    {
        ConvolutionFilter2D[] filters;
        bool shouldAbs;
        string name;

        private CompositeConvolutionFilter2D(ConvolutionFilter2D[] filters, string name, bool shouldAbs = false)
        {
            this.filters = filters;
            this.name = name;
            this.shouldAbs = shouldAbs;
        }

        public void Apply(GrayscaleBitmap bmp)
        {
            GrayscaleBitmap[] partialResults = new GrayscaleBitmap[filters.Length];

            for(int i = 0; i < filters.Length; i++)
            {
                partialResults[i] = bmp.Copy();

                filters[i].Apply(partialResults[i]);
            }

            double normalizationCoeff = 1.0 / filters.Length;

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    bmp[i, j] = 0;
                    foreach (var partialResult in partialResults)
                    {
                        bmp[i,j] += normalizationCoeff * (shouldAbs ? Math.Abs(partialResult[i,j]) : partialResult[i,j]);
                    }
                }
            }
        }

        public override string ToString()
        {
            return name;
        }

        public static CompositeConvolutionFilter2D getRoberts()
        {
            return new CompositeConvolutionFilter2D(
                    new ConvolutionFilter2D[] { ConvolutionFilter2D.GetRoberts1(), ConvolutionFilter2D.GetRoberts2() },
                    "Roberts",
                    true
                );
        }

        public static CompositeConvolutionFilter2D getKirsch()
        {
            return new CompositeConvolutionFilter2D(
                    new ConvolutionFilter2D[] { ConvolutionFilter2D.GetKirsch1(), ConvolutionFilter2D.GetKirsch2() },
                    "Kirsch",
                    true
                );
        }
    }

    //public class EdgeDetectorRoberts : Filter2D
    //{
    //    private static EdgeDetectorRoberts INSTANCE = new EdgeDetectorRoberts();

    //    public static EdgeDetectorRoberts Instance {
    //        get { return INSTANCE; }
    //    }

    //    private EdgeDetectorRoberts() { }

    //    private Filter2D roberts1 = ConvolutionFilter2D.GetRoberts1();
    //    private Filter2D roberts2 = ConvolutionFilter2D.GetRoberts2();

    //    public void Apply(GrayscaleBitmap bmp)
    //    {
    //        GrayscaleBitmap robertsBmp1 = bmp.Copy();
    //        GrayscaleBitmap robertsBmp2 = bmp.Copy();

    //        roberts1.Apply(robertsBmp1);
    //        roberts2.Apply(robertsBmp2);

    //        for (int i = 0; i < bmp.Height; i++)
    //        {
    //            for (int j = 0; j < bmp.Width; j++)
    //            {
    //                bmp[i, j] = 0.5 * (Math.Abs(robertsBmp1[i, j]) + Math.Abs(robertsBmp2[i, j]));
    //            }
    //        }
    //    }

    //    public override string ToString()
    //    {
    //        return "Edge detection (Roberts)";
    //    }
    //}
}
