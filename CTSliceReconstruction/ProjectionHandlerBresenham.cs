using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    /// <summary>
    /// Projection handler based on Bresenhams line algorithm
    /// </summary>
    public class ProjectionHandlerBresenham : ProjectionHandler
    {
        /// <summary>
        /// Generates line representing x-ray of given angle and position.
        /// N determines the size of bitmap
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="n"></param>
        /// <param name="position"></param>
        /// <returns>Line of intersected pixels of given bitmap</returns>
        public override List<PixelInfo> generateLine(double angle, int n, int position)
        {
            //Generate line
            Line ray = calculateRay(angle, n, position);

            //Calculate intersections with bounding box
            List<Point> intersections = getInteresctionsWithBitmapBoundary(ray, n);

            List<PixelInfo> line;

            //single point intersection must be handled separately
            if (intersections.Count == 1)
            {

                line = new List<PixelInfo>();
                Point point = intersections[0];

                line.Add(new PixelInfo(point.i, point.j, 1.0));
            }
            //two point intersection
            else
            {
                Point p0 = intersections[0];
                Point p1 = intersections[1];

                //perform bresenham line algorithm between the two points
                line = bresenhamLine(p0.i, p0.j, p1.i, p1.j);
            }

            return line;
        }

        /// <summary>
        /// Returns ray specified by given angle, position and size of bitmap to be intersected
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="n"></param>
        /// <param name="position"></param>
        /// <returns></returns>
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


            //to simplify bresenhams algorithm, the points are sorted by x-coordinate
            intersections.Sort(delegate (Vector2D v1, Vector2D v2)
            {
                return v1.x.CompareTo(v2.x);
            });

            List<Point> intersectionPoints = new List<Point>();

            foreach(var intersection in intersections)
            {
                Point p = new Point((int)(Math.Round(intersection.x - min - 0.5)), (int)(Math.Round(intersection.y - min - 0.5)));

                //if intersection is falsely detected outside of bitmap, correct the coordinates
                if (p.i == n)
                {
                    p.i = n - 1;
                }

                if (p.j == n)
                {
                    p.j = n - 1;
                }

                if (p.i == -1)
                {
                    p.i = 0;
                }

                if (p.j == -1)
                {
                    p.j = 0;
                }

                intersectionPoints.Add(p);
            }

            return intersectionPoints;
        }

        /// <summary>
        /// Check whether the intersection with line going through the bitmap boundary is also intersection with the boundary
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="n"></param>
        /// <param name="param"></param>
        /// <param name="intersections"></param>
        /// <param name="isVertical"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Implementation of Bresenhams Line algorithm
        /// </summary>
        /// <param name="x0">X-coordinate of first pixel</param>
        /// <param name="y0">Y-coordinate of first pixel</param>
        /// <param name="x1">X-coordinate of last pixel</param>
        /// <param name="y1">Y-coordinate of last pixel</param>
        /// <returns>Raster line between first and last pixel</returns>
        public List<PixelInfo> bresenhamLine(int x0, int y0, int x1, int y1)
        {
            List<PixelInfo> line = new List<PixelInfo>();

            double deltaX = x1 - x0;
            double deltaY = y1 - y0;

            //if the line is horizontal or vertical, the generating process is simplified
            if (deltaX == 0)
            {
                return VerticalLine(x0, (int)Math.Abs(deltaY) + 1);
            }

            if (deltaY == 0)
            {
                return HorizontalLine(y0, (int)Math.Abs(deltaX) + 1);
            }

            //calculate the increment of error by performing one step in the x-coordinate
            double deltaErr = Math.Abs(deltaY / deltaX);

            double error = 0.0;

            int y = y0;

            for (int x = x0; x <= x1; x++)
            {
                line.Add(new PixelInfo(x, y, 1.0));

                //perform one step in x-coordinate and increase the error
                error += deltaErr;

                //if the error is bigger than threshold, adjust y-coordinate
                while (error >= 0.5)
                {
                    y += Math.Sign(deltaY);

                    if ((deltaY > 0 && y <= y1) || (deltaY < 0 && y >= y1))
                    {
                        line.Add(new PixelInfo(x, y, 1.0));
                    }
                    
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

        public override string ToString()
        {
            return "Bresenham";
        }
    }
}
