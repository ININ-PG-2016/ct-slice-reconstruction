using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    /// <summary>
    /// Utility for handling sinogram data
    /// </summary>
    public class SinogramHandler
    {
        /// <summary>
        /// Converts given sinogram bitmap to projections
        /// </summary>
        /// <param name="bmp">Input sinogram</param>
        /// <returns>List of projections</returns>
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
                    //valuable data is placed in the second third of the image
                    projection[j] = bmp[projectionCount + i, j];
                }

                projections.Add(projection);
            }

            return projections;
        }

        /// <summary>
        /// Converts given list of projections to sinogram.
        /// Projections must be in correct order for sinogram to show correct values
        /// </summary>
        /// <param name="projections">Input list of projections</param>
        /// <returns>Bitmap containing sinogram of given projections</returns>
        public static GrayscaleBitmap ProjectionsToSinogram(List<double[]> projections)
        {
            int projectionCount = projections.Count;

            int projectionSize = projections[0].Length;

            //Sinogram contains given projections 3 times, the valuable data is in the second third
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

                //For sinogram continuality, the first and last third must be reversed
                reversed ^= true;
            }

            return sinogram;
        }
    }
}
