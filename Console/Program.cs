using CTSliceReconstruction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            /*GrayscaleBitmap bmp = new GrayscaleBitmap(1024,1024);

            Console.WriteLine(bmp.Width);
            Console.WriteLine(bmp.Height);

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    bmp[i, j] = Math.Sqrt(Math.Pow((double)i, 2.0) + Math.Pow((double)j, 2.0));
                }
            }

            bmp.Stretch();

            bmp.SaveToFile("pokus.bmp");*/

            /*GrayscaleBitmap bmp = new GrayscaleBitmap("star.png");
            double angle = 0;
            ProjectionHandler projectionHandler = new ProjectionHandlerRaycast();
            double[] projection = projectionHandler.CreateProjection(bmp, angle);
            GrayscaleBitmap projectedBmp = projectionHandler.ExtrudeProjection(projection, angle);
            projectedBmp.SaveToFile("projected.bmp");*/

            GrayscaleBitmap bmp = new GrayscaleBitmap("star2.png");
            ProjectionHandler projectionHandler = new ProjectionHandlerRaycast();
            Console.WriteLine("Generating projections");
            List<double[]> projections = projectionHandler.GenerateProjections(bmp, 180 / 5);
            IterativeSliceReconstructor reconstructor = new IterativeSliceReconstructor(projections, 5, projectionHandler);
            Console.WriteLine("Reconstructing");
            GrayscaleBitmap result = reconstructor.Reconstruct(100);
            for (int i = 0; i < result.Width; i++)
            {
                for (int j = 0; j < result.Height; j++)
                {
                    if (result[i, j] < 0)
                        result[i, j] = 0;
                    if (result[i, j] > 1)
                        result[i, j] = 1;
                }
            }
            result.SaveToFile("result2.bmp");
        }
    }
}
