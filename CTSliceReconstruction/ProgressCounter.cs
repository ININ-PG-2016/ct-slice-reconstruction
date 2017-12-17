using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public class ProgressCounter
    {
        private int maxSteps;
        private int steps = 0;

        public ProgressCounter(int maxSteps)
        {
            this.maxSteps = maxSteps;
            steps = 0;
        }

        virtual public void AddStep()
        {
            steps++;
            if (steps > maxSteps)
                steps = maxSteps;
        }

        virtual public void AddSteps(int steps)
        {
            steps += steps;
            if (steps > maxSteps)
                steps = maxSteps;
        }

        public int GetSteps()
        {
            return steps;
        }

        public int GetMaxSteps()
        {
            return maxSteps;
        }

        virtual public void SetMaxSteps(int maxSteps)
        {
            this.maxSteps = maxSteps;
        }

        virtual public void Reset()
        {
            steps = 0;
        }
    }
}
