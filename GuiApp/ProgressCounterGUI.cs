using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTSliceReconstruction;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;

namespace GuiApp
{
    class ProgressCounterGUI : ProgressCounter
    {
        private ProgressBar progressBar;

        public ProgressCounterGUI(int maxSteps, ProgressBar progressBar) : base(maxSteps)
        {
            progressBar.Value = 0;
            progressBar.Maximum = maxSteps;
            this.progressBar = progressBar;
        }

        public override void AddStep()
        {
            base.AddStep();
            progressBar.Value++;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }

        public override void AddSteps(int steps)
        {
            base.AddSteps(steps);
            progressBar.Value += steps;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }

        public override void Reset()
        {
            base.Reset();
            progressBar.Value = 0;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }

        public override void SetMaxSteps(int maxSteps)
        {
            base.SetMaxSteps(maxSteps);
            progressBar.Maximum = maxSteps;
        }
    }
}
