using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using CTSliceReconstruction;
using System.Windows.Threading;

namespace GuiApp
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            projectionAlgorithm.Items.Add(new ProjectionHandlerRaycast());
            projectionAlgorithm.Items.Add(new ProjectionHandlerBresenham());
            projectionAlgorithm.SelectedIndex = 0;
            reconstructionAlgorithm.Items.Add("Back projection");
            reconstructionAlgorithm.Items.Add("Iterative");
            reconstructionAlgorithm.SelectedIndex = 0;

            projectionFilter.Items.Add(Filter1D.GetGaussianFilter());
            projectionFilter.Items.Add(Filter1D.GetHammingFilter1());
            projectionFilter.Items.Add(Filter1D.GetHammingFilter2());
            projectionFilter.Items.Add(Filter1D.GetHammingFilter3());
            projectionFilter.Items.Add(Filter1D.GetLaplaceFilter());
            projectionFilter.Items.Add(Filter1D.GetLaplaceSharpeningFilter());
            projectionFilter.Items.Add(Filter1D.GetMultiplicativeNoiseFilter());
            projectionFilter.Items.Add(Filter1D.GetAdditiveNoiseFilter());
            projectionFilter.SelectedIndex = 0;

            sinogramFilter.Items.Add(ConvolutionFilter2D.GetGauss55());
            sinogramFilter.Items.Add(ConvolutionFilter2D.GetLaplace());
            sinogramFilter.Items.Add(EdgeDetectorRoberts.Instance);
            sinogramFilter.Items.Add(LaplacianSharpening.Instance);
            sinogramFilter.SelectedIndex = 0;
        }

        private void setState(String state)
        {
            stateLabel.Content = state;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }

        private void runBtn_Click(object sender, RoutedEventArgs e)
        {
            int steps = 0;
            if ((String)reconstructionAlgorithm.SelectedItem == "Back projection")
                steps = (int)numberOfProjections.Value;
            else
                steps = (int)numberOfIterations.Value;
            ProgressCounter progressCounter = new ProgessCounterGUI((int)numberOfProjections.Value + steps, progressBar);
            setState("Loading input image");
            GrayscaleBitmap bmp = new GrayscaleBitmap(inputPicture.Text);
            bmp = bmp.CreateSquareBitmap();
            setState("Generating projections");
            List<double[]> projections = ((ProjectionHandler)projectionAlgorithm.SelectedItem).GenerateProjections(bmp, (int)numberOfProjections.Value, progressCounter);

            

            setState("Filtering projections");
            foreach (Filter1D filter in projectionFilterList.Items)
            {
                filter.Apply(projections);
            }

            GrayscaleBitmap sinogram = SinogramHandler.ProjectionsToSinogram(projections);

            setState("Filtering sinogram");

            foreach (Filter2D filter in sinogramFilterList.Items)
            {
                filter.Apply(sinogram);
            }

            projections = SinogramHandler.SinogramToProjections(sinogram);

            GrayscaleBitmap result;

            setState("Reconstructing");
            if ((String)reconstructionAlgorithm.SelectedItem == "Back projection")
            {
                BackProjectionSliceReconstructor reconstructor = new BackProjectionSliceReconstructor(projections, 180.0 / (double)numberOfProjections.Value, (ProjectionHandler)projectionAlgorithm.SelectedItem);
                result = reconstructor.Reconstruct(progressCounter);
                
            }
            else
            {
                IterativeSliceReconstructor reconstructor = new IterativeSliceReconstructor(projections, 180.0 / (double)numberOfProjections.Value, (ProjectionHandler)projectionAlgorithm.SelectedItem, (bool)allowNegativeValuesCheckBox.IsChecked);
                result = reconstructor.Reconstruct((int)numberOfIterations.Value, progressCounter);
                
            }

            setState("Nothing to do");

            progressCounter.Reset();

            ResultWindow resultWnd = new ResultWindow(sinogram, result);
            resultWnd.Show();
        }

        private void loadPictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Image files|*.bmp;*.png;*.jpg;*.jpeg;*.tiff";
            if (openDialog.ShowDialog() == true)
            {
                inputPicture.Text = openDialog.FileName;
            }
        }

        private void reconstructionAlgorithm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((String)reconstructionAlgorithm.SelectedItem) == "Iterative")
            {
                numberOfIterationsLabel.Visibility = Visibility.Visible;
                numberOfIterations.Visibility = Visibility.Visible;
                allowNegativeValuesCheckBox.Visibility = Visibility.Visible;
            }
            else
            {
                numberOfIterationsLabel.Visibility = Visibility.Hidden;
                numberOfIterations.Visibility = Visibility.Hidden;
                allowNegativeValuesCheckBox.Visibility = Visibility.Hidden;
            }
        }

        private void inputPicture_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inputPicture.Text == null || inputPicture.Text == "")
                runBtn.IsEnabled = false;
            else
                runBtn.IsEnabled = true;
        }

        private void addProjecitonFilterBtn_Click(object sender, RoutedEventArgs e)
        {
            Filter1D selectedFilter = (Filter1D)projectionFilter.SelectedItem;
            projectionFilterList.Items.Add(selectedFilter);
        }

        private void clearProjecitonFilterBtn_Click(object sender, RoutedEventArgs e)
        {
            projectionFilterList.Items.Clear();
        }

        private void addSinogramFilterBtn_Click(object sender, RoutedEventArgs e)
        {
            Filter2D selectedFilter = (Filter2D)sinogramFilter.SelectedItem;
            sinogramFilterList.Items.Add(selectedFilter);
        }

        private void clearSinogramFilterBtn_Click(object sender, RoutedEventArgs e)
        {
            sinogramFilterList.Items.Clear();
        }
    }
}
