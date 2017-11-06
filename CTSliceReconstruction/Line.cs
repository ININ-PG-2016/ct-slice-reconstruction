using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    class Line
    {
        public Vector2D origin;
        public Vector2D direction;

        public Line(Vector2D origin, Vector2D direction)
        {
            this.origin = origin;
            this.direction = direction;
        }

        public Vector2D GetPointForParam(double t)
        {
            double x = origin.x + t * direction.x;
            double y = origin.y + t * direction.y;
            return new Vector2D(x, y);
        }

        public double GetIntersectionWithVerticalLine(double distanceFromOrigin)
        {
            return (distanceFromOrigin - origin.x) / direction.x;
        }

        public double GetIntersectionWithHorizontalLine(double distanceFromOrigin)
        {
            return (distanceFromOrigin - origin.y) / direction.y;
        }
    }
}
