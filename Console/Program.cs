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

            String inputFilename = args.Length >= 1 ? args[0] : "Shapes.bmp";
            String outputFilename = args.Length >= 2 ? args[1] : "result.bmp";
            GrayscaleBitmap bmp = new GrayscaleBitmap(inputFilename);
            ProjectionHandler projectionHandler = new ProjectionHandlerRaycast();
            Console.WriteLine("Generating projections");
            double angle = 1;
            List<double[]> projections = projectionHandler.GenerateProjections(bmp, (int)(180.0 / angle));
            //IterativeSliceReconstructor reconstructor = new IterativeSliceReconstructor(projections, angle, projectionHandler, false);
            BackProjectionSliceReconstructor reconstructor = new BackProjectionSliceReconstructor(projections, angle, projectionHandler);
            Console.WriteLine("Reconstructing");
            GrayscaleBitmap result = reconstructor.Reconstruct();
            //GrayscaleBitmap result = reconstructor.Reconstruct(500);
            GrayscaleBitmap error = new GrayscaleBitmap(bmp.Width, bmp.Height);
            for (int i = 0; i < result.Width; i++)
            {
                for (int j = 0; j < result.Height; j++)
                {
                    if (result[i, j] < 0)
                        result[i, j] = 0;
                    if (result[i, j] > 1)
                        result[i, j] = 1;
                    error[i, j] = Math.Abs(result[i, j] - bmp[i, j]);
                }
            }
            result.SaveToFile(outputFilename);
            error.SaveToFile(outputFilename + ".error.bmp");

            /*ProjectionHandlerBresenham handler = new ProjectionHandlerBresenham();
            handler.generateLine(0.0, 25, 0);
            handler.generateLine(0.0, 25, 13);
            handler.generateLine(45, 25, 12);*/
        }
    }
}
