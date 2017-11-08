using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    class ProjectionHandlerRaycast : ProjectionHandler
    {
        protected override List<PixelInfo> generateLine(double angle, int n, int position)
        {
            double radAngle = angle * (Math.PI / 180);
            Vector2D direction = new Vector2D(Math.Cos(radAngle), Math.Sin(radAngle));
            Vector2D perpendicular = new Vector2D(-direction.y, direction.x);
            double distBegin = -(n / 2.0);
            double distEnd = (n / 2.0);
            double lineShift = distBegin + 0.5 + position;
            Line line = new Line(new Vector2D(perpendicular.x * lineShift, perpendicular.y * lineShift), direction);
            double dirLen = Math.Sqrt(line.direction.x * line.direction.x + line.direction.y * line.direction.y);
            line.direction = new Vector2D(line.direction.x / dirLen, line.direction.y / dirLen);
            List<double> intersectionParameters = new List<double>();
            for (double k = distBegin; k < distEnd + 0.5; k++)
            {
                double intersection = line.GetIntersectionWithHorizontalLine(k);
                if (double.IsNaN(intersection))
                    intersection = double.PositiveInfinity;
                intersectionParameters.Add(intersection);
                intersection = line.GetIntersectionWithVerticalLine(k);
                if (double.IsNaN(intersection))
                    intersection = double.PositiveInfinity;
                intersectionParameters.Add(intersection);
            }
            intersectionParameters.Sort();
            List<PixelInfo> pixels = new List<PixelInfo>();
            for (int i = 0; i < intersectionParameters.Count - 1; i++)
            {
                double avrgIntersectionParam = (intersectionParameters[i] + intersectionParameters[i + 1]) / 2;
                Vector2D avrgIntersection = line.GetPointForParam(avrgIntersectionParam);
                avrgIntersection.x -= distBegin;
                avrgIntersection.y -= distBegin;
                if (avrgIntersection.x >= 0 && avrgIntersection.x < n &&
                    avrgIntersection.y >= 0 && avrgIntersection.y < n)
                {
                    double weight = (intersectionParameters[i + 1] - intersectionParameters[i]) / Math.Sqrt(2);
                    PixelInfo pixel = new PixelInfo((int)avrgIntersection.x, (int)avrgIntersection.y, weight);
                    if (pixels.Count > 0)
                    {
                        PixelInfo last = pixels[pixels.Count - 1];
                        if (last.position.i == pixel.position.i && last.position.j == pixel.position.j)
                        {
                            if (last.weight < pixel.weight)
                                pixels[pixels.Count - 1] = pixel;
                        }
                        else
                            pixels.Add(pixel);
                    }
                    else
                        pixels.Add(pixel);
                }
            }
            return pixels;
        }
    }
}
