using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    /// <summary>
    /// 2D filter to apply on GrayscaleBitmap
    /// </summary>
    public interface Filter2D
    {
        /// <summary>
        /// Applies filter on given bitmap
        /// </summary>
        /// <param name="bmp">Bitmap to be filtered</param>
        void Apply(GrayscaleBitmap bmp);
    }

    /// <summary>
    /// 2D convolution filter
    /// It is required to specify mask, origin of the filter and its name
    /// </summary>
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

        /// <summary>
        /// Applies filter on given bitmap on given position
        /// </summary>
        /// <param name="bmp">Filtered bitmap</param>
        /// <param name="i">I Position</param>
        /// <param name="j">J Position</param>
        /// <returns>Result of applied filter</returns>
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

        /// <summary>
        /// Applies filter on given bitmap
        /// </summary>
        /// <param name="bmp">Bitmap to be filtered</param>
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

        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// Gaussian blur of size 5x5
        /// </summary>
        /// <returns>Instance of Gaussian Blur</returns>
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

        /// <summary>
        /// Laplace filter
        /// </summary>
        /// <returns>Instance of Laplace filter</returns>
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

        /// <summary>
        /// Laplacian sharpening filter of size 3x3
        /// </summary>
        /// <returns>Instance of Laplacian sharpening filter</returns>
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

        /// <summary>
        /// First mask of Roberts edge operator
        /// </summary>
        /// <returns></returns>
        public static ConvolutionFilter2D GetRoberts1()
        {
            return new ConvolutionFilter2D(
                    new double[,] { { 1.0, 0.0 }, { 0.0, -1.0 } },
                    0,0,
                    "Roberts 1"
                );
        }

        /// <summary>
        /// Second mask of Roberts edge operator
        /// </summary>
        /// <returns></returns>
        public static ConvolutionFilter2D GetRoberts2()
        {
            return new ConvolutionFilter2D(
                    new double[,] { { 0.0, 1.0 }, { -1.0, -0.0 } },
                    0, 0,
                    "Roberts 2"
                );
        }

        /// <summary>
        /// First mask of Kirsch edge operator
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Second mask of Kirsch edge operator
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Mask of Laplacian of Gaussian of size 5x5
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Mask of Laplacian of Gaussian of size 7x7
        /// </summary>
        /// <returns></returns>
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

    /// <summary>
    /// Filter composed from multiple Convolution filters
    /// </summary>
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

        /// <summary>
        /// Apply filter on given bitmap
        /// </summary>
        /// <param name="bmp">Filtered bitmap</param>
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
                        //some filters require absolute value of partial results
                        bmp[i,j] += normalizationCoeff * (shouldAbs ? Math.Abs(partialResult[i,j]) : partialResult[i,j]);
                    }
                }
            }
        }

        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// Edge detection filter based on Roberts edge operator
        /// </summary>
        /// <returns></returns>
        public static CompositeConvolutionFilter2D getRoberts()
        {
            return new CompositeConvolutionFilter2D(
                    new ConvolutionFilter2D[] { ConvolutionFilter2D.GetRoberts1(), ConvolutionFilter2D.GetRoberts2() },
                    "Roberts",
                    true
                );
        }

        /// <summary>
        /// Edge detection filter based on Kirsch edge operator
        /// </summary>
        /// <returns></returns>
        public static CompositeConvolutionFilter2D getKirsch()
        {
            return new CompositeConvolutionFilter2D(
                    new ConvolutionFilter2D[] { ConvolutionFilter2D.GetKirsch1(), ConvolutionFilter2D.GetKirsch2() },
                    "Kirsch",
                    true
                );
        }
    }

    /// <summary>
    /// Filter for zeroing Negative values
    /// </summary>
    public class RemoveNegativeValuesFilter : Filter2D
    {
        public void Apply(GrayscaleBitmap bmp)
        {
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    bmp[i, j] = (bmp[i, j] > 0) ? bmp[i, j] : 0;
                }
            }
        }

        public override string ToString()
        {
            return "Negative value remover (Zeroing)";
        }
    }

    /// <summary>
    /// Filter for removing negative values by absolute value
    /// </summary>
    public class AbsoluteValueFilter : Filter2D
    {
        public void Apply(GrayscaleBitmap bmp) {
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    bmp[i, j] = Math.Abs(bmp[i,j]);
                }
            }
        }


        public override string ToString()
        {
            return "Negative value remover (Abs)";
        }
    }
}
