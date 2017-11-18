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
        public static void AddNoise(List<double[]> projections, double noiseMagnitude)
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
    }
}
