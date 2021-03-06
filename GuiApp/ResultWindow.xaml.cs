﻿using CTSliceReconstruction;
using Microsoft.Win32;
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
        private GrayscaleBitmap sinogram;
        private GrayscaleBitmap result;
        IterativeSliceReconstructor reconstructor;
        public ResultWindow(GrayscaleBitmap sinogram, GrayscaleBitmap result, IterativeSliceReconstructor reconstructor)
        {
            this.sinogram = sinogram;
            this.result = result;
            this.reconstructor = reconstructor;
            InitializeComponent();
            sinogramImg.Source = PrepareBitmap(sinogram.Bmp);
            resultImg.Source = PrepareBitmap(result.Bmp);

            if (reconstructor != null)
            {
                moreIterationsBtn.Visibility = Visibility.Visible;
                moreIterationsCount.Visibility = Visibility.Visible;
                progressBar.Visibility = Visibility.Visible;
                moreIterationsColumn.Width = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                moreIterationsBtn.Visibility = Visibility.Hidden;
                moreIterationsCount.Visibility = Visibility.Hidden;
                progressBar.Visibility = Visibility.Hidden;
                moreIterationsColumn.Width = new GridLength(0);
            }
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

        private void moreIterationsBtn_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            ProgressCounterGUI progressCounter = new ProgressCounterGUI(moreIterationsCount.Value.GetValueOrDefault(), progressBar);
            result = reconstructor.PerformAdditionalIterations(moreIterationsCount.Value.GetValueOrDefault(), progressCounter);
            resultImg.Source = PrepareBitmap(result.Bmp);
            progressCounter.Reset();
            this.IsEnabled = true;
        }

        public void ApplyFilters(ItemCollection selectedFilters)
        {
            foreach (Filter2D filter in selectedFilters)
            {
                filter.Apply(result);
            }

            result.InvalidateSystemBitmap();

            resultImg.Source = PrepareBitmap(result.Bmp);
        }

        private void filterResultBtn_Click(object sender, RoutedEventArgs e)
        {
            ResultFilterWindow filtering = new ResultFilterWindow(this);
            filtering.Show();
        }

        private void saveResultBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Image files|*.bmp";
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                result.SaveToFile(path);
            }
        }
    }
}
