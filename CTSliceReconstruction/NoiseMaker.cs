using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public class NoiseMaker
    {
        static Random rnd = new Random();
        public static void AddMultiplicativeNoise(List<double[]> projections, double noiseMagnitude)
        {
            for (int i = 0; i < projections.Count; i++)
            {
                for (int j = 0; j < projections[i].Length; j++)
                {
                    double noise = (rnd.NextDouble() - 0.5) * 2 * noiseMagnitude;
                    projections[i][j] *= (1 + noise);
                }
            }
        }

        public static void AddAdditiveNoise(List<double[]> projections, double noiseMagnitude)
        {
            double totalAvrg = 0;
            for (int i = 0; i < projections.Count; i++)
            {
                double avrg = 0;
                for (int j = 0; j < projections[i].Length; j++)
                {
                    avrg += projections[i][j];
                }
                avrg /= (double)projections[i].Length;
                totalAvrg += avrg;
            }
            totalAvrg /= (double)projections.Count;
            for (int i = 0; i < projections.Count; i++)
            {
                for (int j = 0; j < projections[i].Length; j++)
                {
                    double noise = (rnd.NextDouble() - 0.5) * 2 * noiseMagnitude * totalAvrg;
                    projections[i][j] += noise;
                }
            }
        }

        public static void AddMultiplicativeNoise(GrayscaleBitmap bmp, double noiseMagnitude)
        {
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    double noise = (rnd.NextDouble() - 0.5) * 2 * noiseMagnitude;
                    bmp[i, j] *= (1 + noise);
                }
            }
        }
    }
}
