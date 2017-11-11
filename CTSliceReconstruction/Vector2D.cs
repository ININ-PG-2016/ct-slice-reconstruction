using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    class Vector2D
    {
        public double x;
        public double y;

        public Vector2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2D operator * (double value, Vector2D vector)
        {
            return new Vector2D(vector.x * value, vector.y * value);
        }

        public void Normalize()
        {
            double norm = Math.Sqrt(x * x + y * y);

            if (norm < 1E-12)
            {
                return;
            }

            x /= norm;
            y /= norm;
        }
    }
}
