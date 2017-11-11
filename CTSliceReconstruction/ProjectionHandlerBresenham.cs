using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public class ProjectionHandlerBresenham : ProjectionHandler
    {
        public override List<PixelInfo> generateLine(double angle, int n, int position)
        {
            //Generate line
            Line ray = calculateRay(angle, n, position);

            //Calculate intersections with bounding box
            getInteresctionsWithBitmapBoundary(ray, n);

            //Calculate intersected pixels on boundary
            throw new NotImplementedException();
            return null;
        }

        private Line calculateRay(double angle, int n, int position)
        {
            double radAngle = angle * (Math.PI / 180);

            Vector2D direction = new Vector2D(Math.Cos(radAngle), Math.Sin(radAngle));
            Vector2D perpendicular = new Vector2D(-direction.y, direction.x);

            double shift = -(n / 2.0) + position + 0.5;

            return new Line(shift * perpendicular, direction);
        }

        private List<Point> getInteresctionsWithBitmapBoundary(Line ray, int n)
        {
            double min = -n / 2.0 + 0.5;
            double max = n / 2.0 - 0.5;

            List<Vector2D> intersections = new List<Vector2D>();

            processIntersection(ray, n, ray.GetIntersectionWithVerticalLine(min), intersections, true);

            processIntersection(ray, n, ray.GetIntersectionWithVerticalLine(max), intersections, true);

            processIntersection(ray, n, ray.GetIntersectionWithHorizontalLine(min), intersections, false);

            processIntersection(ray, n, ray.GetIntersectionWithHorizontalLine(max), intersections, false);

            intersections.Sort(delegate (Vector2D v1, Vector2D v2)
            {
                return v1.x.CompareTo(v2.x);
            });

            List<Point> intersectionPoints = new List<Point>();

            foreach(var intersection in intersections)
            {
                intersectionPoints.Add(new Point((int) (Math.Round(intersection.x - min)), (int) (Math.Round(intersection.y - min))));
            }

            return intersectionPoints;
        }

        private bool processIntersection(Line ray, int n, double param, List<Vector2D> intersections, bool isVertical)
        {
            if (double.IsNaN(param))
            {
                return false;
            }

            Vector2D intersection = ray.GetPointForParam(param);

            if (isVertical && intersection.y >= -n / 2.0 && intersection.y <= n / 2.0)
            {
                intersections.Add(intersection);
                return true;
            }
            else if (!isVertical && intersection.x >= -n / 2.0 && intersection.x <= n / 2.0)
            {
                intersections.Add(intersection);
                return true;
            }

            return false;
        }

        public List<PixelInfo> bresenhamLine(int x0, int y0, int x1, int y1)
        {
            return null;
        }

        private List<PixelInfo> VerticalLine(int x, int n)
        {
            List<PixelInfo> result = new List<PixelInfo>();

            for (int i = 0; i < n; i++)
            {
                result.Add(new PixelInfo(x, i, 1.0));
            }

            return result;
        }

        private List<PixelInfo> HorizontalLine(int y, int n)
        {
            List<PixelInfo> result = new List<PixelInfo>();

            for (int i = 0; i < n; i++)
            {
                result.Add(new PixelInfo(i, y, 1.0));
            }

            return result;
        }
    }
}
