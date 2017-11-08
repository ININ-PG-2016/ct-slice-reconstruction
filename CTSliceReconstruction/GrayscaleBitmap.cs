using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public class GrayscaleBitmap
    {
        private double[] data;
        private System.Drawing.Bitmap bmp;
        private int actualWidth;

        public GrayscaleBitmap(string path)
        {
            this.bmp = new System.Drawing.Bitmap(path);

            //Only supported format is 8bpp Indexed
            if (bmp.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new Exception("Unsupported pixel format of the image. Only 8bbp gray level format is supported.");
            }

            Width = bmp.Width;
            Height = bmp.Height;

            Rectangle rect = new Rectangle(0, 0, Width, Height);

            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

            IntPtr pointer = bmpData.Scan0;
            this.actualWidth = Math.Abs(bmpData.Stride);
            int byteCount = actualWidth * bmp.Height;

            byte[] byteData = new byte[byteCount];

            //Copy data from bitmap to pixel data
            Marshal.Copy(pointer, byteData, 0, byteCount);

            bmp.UnlockBits(bmpData);

            data = new double[Width * Height];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    data[i * Width + j] = ((double)byteData[i * actualWidth + j]) / 255.0;
                }
            }
        }

        /// <summary>
        /// Create empty bitmap with given size
        /// </summary>
        /// <param name="width">Bitmap width</param>
        /// <param name="height">Bitmap height</param>
        public GrayscaleBitmap(int width, int height)
        {
            this.data = new double[width * height];

            Width = width;
            Height = height;
            this.actualWidth = width;
        }

        public int Width
        {
            get;
        }

        public int Height
        {
            get;
        }

        public double this[int i, int j]
        {
            get
            {
                //if acessing out of bounds, return zero
                if (i < 0 || i >= Height)
                {
                    return 0.0;
                }

                if (j < 0 || j >= Width)
                {
                    return 0.0;
                }

                return data[i * (Width) + j];
            }

            set
            {
                //do not set value if out of bounds
                if (i < 0 || i >= Height)
                {
                    return;
                }

                if (j < 0 || j >= Width)
                {
                    return;
                }

                data[i * (Width) + j] = value;
            }
        }

        public double this[Point point]
        {
            get { return this[point.i, point.j]; }
            set { this[point.i, point.j] = value; }
        }

        /// <summary>
        /// System representation of Bitmap
        /// Used for File IO and GUI
        /// </summary>
        public System.Drawing.Bitmap Bmp
        {
            get
            {
                if (bmp == null)
                {
                    createSystemBitmap();
                }

                return bmp;
            }
        }

        /// <summary>
        /// Create the system representation of bitmap
        /// </summary>
        private void createSystemBitmap()
        {
            this.bmp = new System.Drawing.Bitmap(Width, Height, PixelFormat.Format8bppIndexed);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            // Copy the bytes from the image into a byte array
            byte[] bytes = new byte[data.Height * data.Stride];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    bytes[i * data.Stride + j] = (byte)(this[i, j] * 255.0);
                }
            }

            ColorPalette pallete = bmp.Palette;
            Color[] colors = pallete.Entries;

            for (int i = 0; i < 256; i++)
            {
                colors[i] = Color.FromArgb((byte)i, (byte)i, (byte)i);
            }

            bmp.Palette = pallete;

            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            bmp.UnlockBits(data);
        }

        /// <summary>
        /// Save bitmap to file
        /// </summary>
        /// <param name="filename">File to save</param>
        public void SaveToFile(string filename)
        {
            if (bmp == null)
            {
                createSystemBitmap();
            }

            bmp.Save(filename, ImageFormat.Bmp);
        }

        public void Stretch()
        {
            double max = Double.MinValue;
            double min = Double.MaxValue;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (this[i,j] < min)
                    {
                        min = this[i, j];
                    }

                    if (this[i,j] > max)
                    {
                        max = this[i, j];
                    }
                }
            }

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    this[i, j] = (this[i, j] - min) / (max - min);
                }
            }
        }
    }
}
