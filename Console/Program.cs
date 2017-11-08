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
            GrayscaleBitmap bmp = new GrayscaleBitmap(1024,1024);

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

            bmp.SaveToFile("pokus.bmp");
        }
    }
}
