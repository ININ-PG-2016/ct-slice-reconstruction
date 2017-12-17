using CTSliceReconstruction;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GuiApp
{
    /// <summary>
    /// Interakční logika pro ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        public ResultWindow(GrayscaleBitmap sinogram, GrayscaleBitmap result)
        {
            InitializeComponent();
            sinogramImg.Source = PrepareBitmap(sinogram.Bmp);
            resultImg.Source = PrepareBitmap(result.Bmp);
        }

        private BitmapSource PrepareBitmap(System.Drawing.Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);

            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

            IntPtr pointer = bmpData.Scan0;

            BitmapSource source = BitmapSource.Create(width, height, 300, 300, PixelFormats.Gray8, null, pointer, height * bmpData.Stride, bmpData.Stride);

            bmp.UnlockBits(bmpData);
            return source;
        }
    }
}
