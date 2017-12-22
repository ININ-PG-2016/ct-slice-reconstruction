using CTSliceReconstruction;
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
using System.Windows.Shapes;

namespace GuiApp
{
    /// <summary>
    /// Interakční logika pro ResultFilterWindow.xaml
    /// </summary>
    public partial class ResultFilterWindow : Window
    {
        ResultWindow resultWindow;

        public ResultFilterWindow(ResultWindow resultWindow)
        {
            InitializeComponent();

            this.resultWindow = resultWindow;

            resultFilter.Items.Add(ConvolutionFilter2D.GetGauss55());
            resultFilter.Items.Add(ConvolutionFilter2D.GetLaplace());
            resultFilter.Items.Add(ConvolutionFilter2D.GetSharpen());
            resultFilter.Items.Add(ConvolutionFilter2D.GetLaplacianOfGaussian55());
            resultFilter.Items.Add(ConvolutionFilter2D.GetLaplacianOfGaussian77());
            //resultFilter.Items.Add(EdgeDetectorRoberts.Instance);
            resultFilter.Items.Add(CompositeConvolutionFilter2D.getRoberts());
            resultFilter.Items.Add(CompositeConvolutionFilter2D.getKirsch());
            resultFilter.Items.Add(new RemoveNegativeValuesFilter());
            //resultFilter.Items.Add(new AbsoluteValueFilter());
            resultFilter.SelectedIndex = 0;
        }

        private void apply_Click(object sender, RoutedEventArgs e)
        {
            resultWindow.ApplyFilters(resultFilterList.Items);
            Close();
        }

        private void addResultFilterBtn_Click(object sender, RoutedEventArgs e)
        {
            Filter2D selectedFilter = (Filter2D)resultFilter.SelectedItem;
            resultFilterList.Items.Add(selectedFilter);
        }

        private void clearResultFilterBtn_Click(object sender, RoutedEventArgs e)
        {
            resultFilterList.Items.Clear();
        }
    }
}
