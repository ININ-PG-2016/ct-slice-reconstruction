using CTSliceReconstruction;
using System;
using System.Collections.Generic;

namespace ConsoleApp
{
    class Program
    {

        static void Main(string[] args)
        {
            //if (args.Length >= 1 && args[0].Equals("-s"))
            //{
            //    GenerateSinogram(args);
            //    return;
            //}

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
            /*GrayscaleBitmap bmp = new GrayscaleBitmap(500, 500);
            double begin = -bmp.Width / 2.0;
            double end = bmp.Width / 2.0;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    double x = (i + begin) / (bmp.Width / 2.0);
                    double y = (j + begin) / (bmp.Width / 2.0);
                    double value = x * x + y * y;
                    if (value > 1)
                        bmp[i, j] = 0;
                    else
                        bmp[i, j] = value;
                }
            }
            bmp.SaveToFile("paraboloid.bmp");*/
            GrayscaleBitmap bmp = new GrayscaleBitmap(inputFilename);
            Console.WriteLine("Generating projections");
            double angle = 1;
            List<double[]> projections = new ProjectionHandlerRaycast().GenerateProjections(bmp, (int)(180.0 / angle));

            //NoiseMaker.AddNoise(projections, 0.5);

            GrayscaleBitmap sinogram = SinogramHandler.ProjectionsToSinogram(projections);

            EdgeDetectorRoberts.Instance.Apply(sinogram);

            //GrayscaleBitmap laplaceSinogram = sinogram.Copy();

            //for (int i = 0; i < 1; i++)
            //{
            //    Filter2D.GetLaplace().Apply(laplaceSinogram);

            //    sinogram += laplaceSinogram;
            //}


            sinogram.SaveToFile("sinogram.bmp");

            ////Filter1D.GetHammingFilter3().Apply(projections);

            ////sinogram = SinogramHandler.ProjectionsToSinogram(projections);

            //Filter2D.GetGauss55().Apply(sinogram);
            //////Filter2D.GetGauss55().Apply(sinogram);
            //////Filter2D.GetGauss55().Apply(sinogram);

            ////sinogram.SaveToFile("sinogram_after.bmp");

            projections = SinogramHandler.SinogramToProjections(sinogram);

            //Filter1D.GetLaplaceFilter().Apply(projections);
            //Filter1D.GetLaplaceFilter().Apply(projections);
            //Filter1D.GetLaplaceFilter().Apply(projections);

            //GrayscaleBitmap sinogram = GrayscaleBitmap.Sinogram(projections);
            //sinogram.Stretch();
            //sinogram.SaveToFile("sinogram.bmp");

            //List<double[]> projections = SinogramHandler.SinogramToProjections(new GrayscaleBitmap(inputFilename));
            //double angle = 180.0 / projections.Count;

            //List<double[]> projections = new AnalyticParaboloidProjector().GenerateProjections(500, (int)(180.0 / angle));
            //IterativeSliceReconstructor reconstructor = new IterativeSliceReconstructor(projections, angle, new ProjectionHandlerRaycast(), false);
            BackProjectionSliceReconstructor reconstructor = new BackProjectionSliceReconstructor(projections, angle, new ProjectionHandlerRaycast());
            Console.WriteLine("Reconstructing");
            //GrayscaleBitmap result = reconstructor.Reconstruct(540);
            GrayscaleBitmap result = reconstructor.Reconstruct();

            //GrayscaleBitmap filteredResult = result.Copy();
            //ConvolutionFilter2D.GetLaplace().Apply(filteredResult);
            //result += filteredResult;
            //result += filteredResult;
            //result += filteredResult;

            //result.Stretch();

            

            

            //result.Stretch();

            GrayscaleBitmap error = new GrayscaleBitmap(result.Width, result.Height);
            for (int i = 0; i < result.Width; i++)
            {
                for (int j = 0; j < result.Height; j++)
                {
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

        //public static void GenerateSinogram(string[] args)
        //{
        //    String inputFilename = args.Length >= 2 ? args[1] : "Shapes.bmp";
        //    String outputFilename = args.Length >= 3 ? args[2] : "sinogram.bmp";
        //    GrayscaleBitmap bmp = new GrayscaleBitmap(inputFilename);

        //    Console.WriteLine("Generating projections");
        //    double angle = 0.5;
        //    List<double[]> projections = new ProjectionHandlerRaycast().GenerateProjections(bmp, (int)(180.0 / angle));

        //    //Filter1D.GetLaplaceFilter().Apply(projections);
        //    Console.WriteLine("Saving sinogram");
        //    GrayscaleBitmap sinogram = SinogramHandler.ProjectionsToSinogram(projections);
        //    //sinogram.Stretch();
        //    sinogram.SaveToFile(outputFilename);
        //}
    }
}
