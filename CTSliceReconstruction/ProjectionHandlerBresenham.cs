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
            List<Point> intersections = getInteresctionsWithBitmapBoundary(ray, n);

            List<PixelInfo> line;

            if (intersections.Count == 1)
            {

                line = new List<PixelInfo>();
                Point point = intersections[0];

                line.Add(new PixelInfo(point.i, point.j, 1.0));
            }
            else
            {
                Point p0 = intersections[0];
                Point p1 = intersections[1];

                line = bresenhamLine(p0.i, p0.j, p1.i, p1.j);
            }

            //Calculate intersected pixels on boundary
            return line;
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
            double min = -n / 2.0;
            double max = n / 2.0;

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
                Point p = new Point((int)(Math.Round(intersection.x - min)), (int)(Math.Round(intersection.y - min)));

                if (p.i == n)
                {
                    p.i = n - 1;
                }

                if (p.j == n)
                {
                    p.j = n - 1;
                }

                intersectionPoints.Add(p);
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
            List<PixelInfo> line = new List<PixelInfo>();

            double deltaX = x1 - x0;
            double deltaY = y1 - y0;

            if (deltaX == 0)
            {
                return VerticalLine(x0, (int)Math.Abs(deltaY));
            }

            double deltaErr = Math.Abs(deltaY / deltaX);

            double error = 0.0;

            int y = y0;

            for (int x = x0; x <= x1; x++)
            {
                line.Add(new PixelInfo(x, y, 1.0));

                error += deltaErr;

                while (error >= 0.5)
                {
                    y += Math.Sign(deltaY);
                    error -= 1.0;
                }
            }

            return line;
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
