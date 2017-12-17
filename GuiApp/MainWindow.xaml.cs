﻿using System;
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
            setState("Reconstructing");
            if ((String)reconstructionAlgorithm.SelectedItem == "Back projection")
            {
                BackProjectionSliceReconstructor reconstructor = new BackProjectionSliceReconstructor(projections, 180.0 / (double)numberOfProjections.Value, (ProjectionHandler)projectionAlgorithm.SelectedItem);
                GrayscaleBitmap result = reconstructor.Reconstruct(progressCounter);
                result.SaveToFile("result.bmp");
                setState("Nothing to do");
            }
            else
            {
                IterativeSliceReconstructor reconstructor = new IterativeSliceReconstructor(projections, 180.0 / (double)numberOfProjections.Value, (ProjectionHandler)projectionAlgorithm.SelectedItem, (bool)allowNegativeValuesCheckBox.IsChecked);
                GrayscaleBitmap result = reconstructor.Reconstruct((int)numberOfIterations.Value, progressCounter);
                result.SaveToFile("result.bmp");
                setState("Nothing to do");
            }
            progressCounter.Reset();
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
    }
}
