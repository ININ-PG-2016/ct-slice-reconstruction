using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTSliceReconstruction
{
    public class Filter1D
    {
        private double[] filterValues;
        int originIndex;
        bool addToOriginalValue;

        public Filter1D(double[] filterValues, int originIndex, bool addToOriginalValue = false)
        {
            this.originIndex = originIndex;
            this.filterValues = filterValues;
            this.addToOriginalValue = addToOriginalValue;
        }

        private double applyToIndex(double[] values, int index)
        {
            double sum = 0;
            for (int i = 0; i < filterValues.Length; i++)
            {
                int arrayIndex = index - originIndex + i;
                if (arrayIndex >= 0 && arrayIndex < values.Length)
                    sum += (filterValues[i] * values[arrayIndex]);
            }
            if (addToOriginalValue)
                return values[index] + sum;
            return sum;
        }

        public void Apply(double[] values)
        {
            double[] newValues = new double[values.Length];
            for (int i = 0; i < values.Length; i++)
                newValues[i] = applyToIndex(values, i);
            for (int i = 0; i < values.Length; i++)
                values[i] = newValues[i];
        }

        public void Apply(List<double[]> projections)
        {
            for (int i = 0; i < projections.Count; i++)
                Apply(projections[i]);
        }

        public static Filter1D GetLaplaceFilter()
        {
            return new Filter1D(new double[] { -1, 2, -1 }, 1, true);
        }

        public static Filter1D GetHammingFilter1()
        {
            return new Filter1D(new double[] { -3.0 / 35.0, 12.0 / 35.0, 17.0 / 35.0, 12.0 / 35.0, -3.0 / 35.0 }, 2);
        }

        public static Filter1D GetHammingFilter2()
        {
            return new Filter1D(new double[] { -2.0 / 21.0, 3.0 / 21.0, 6.0 / 21.0, 7.0 / 21.0, 6.0 / 21.0, 3.0 / 21.0, -2.0 / 21.0 }, 3);
        }

        public static Filter1D GetHammingFilter3()
        {
            return new Filter1D(new double[] {
                -21.0/231.0, 14.0/231.0, 39.0/231.0, 54.0/231.0, 59.0/231.0, 54.0/231.0, 39.0/231.0, 14.0/231.0, -21.0/231.0
            }, 4);
        }
    }
}
