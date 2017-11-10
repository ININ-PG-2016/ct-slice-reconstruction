using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    class ProjectionHandlerBresenham : ProjectionHandler
    {
        protected override List<PixelInfo> generateLine(double angle, int n, int position)
        {
            //Generate line

            //Calculate intersections with bounding box

            //Calculate intersected pixels on boundary

            return null;
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
