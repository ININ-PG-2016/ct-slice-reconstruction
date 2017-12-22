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
        private int originIndex;
        private String name;

        public Filter1D(double[] filterValues, int originIndex, String name)
        {
            this.originIndex = originIndex;
            this.filterValues = filterValues;
            this.name = name;
        }

        public virtual double applyToIndex(double[] values, int index)
        {
            double sum = 0;
            for (int i = 0; i < filterValues.Length; i++)
            {
                int arrayIndex = index - originIndex + i;
                if (arrayIndex >= 0 && arrayIndex < values.Length)
                    sum += (filterValues[i] * values[arrayIndex]);
            }
            return sum;
        }

        virtual public void Apply(double[] values)
        {
            double[] newValues = new double[values.Length];
            for (int i = 0; i < values.Length; i++)
                newValues[i] = applyToIndex(values, i);
            for (int i = 0; i < values.Length; i++)
                values[i] = newValues[i];
        }

        virtual public void Apply(List<double[]> projections)
        {
            for (int i = 0; i < projections.Count; i++)
                Apply(projections[i]);
        }

        public override string ToString()
        {
            return name;
        }

        public static Filter1D GetLaplaceFilter()
        {
            return new Filter1D(new double[] { -1, 2, -1 }, 1, "Laplacian");
        }

        public static Filter1D GetLaplaceSharpeningFilter()
        {
            return new Filter1D(new double[] { -1, 3, -1 }, 1, "Laplacian Sharpening");
        }

        public static Filter1D GetHammingFilter1()
        {
            return new Filter1D(new double[] { -3.0 / 35.0, 12.0 / 35.0, 17.0 / 35.0, 12.0 / 35.0, -3.0 / 35.0 }, 2, "Hamming (Length: 5)");
        }

        public static Filter1D GetHammingFilter2()
        {
            return new Filter1D(new double[] { -2.0 / 21.0, 3.0 / 21.0, 6.0 / 21.0, 7.0 / 21.0, 6.0 / 21.0, 3.0 / 21.0, -2.0 / 21.0 }, 3, "Hamming (Length: 7)");
        }

        public static Filter1D GetHammingFilter3()
        {
            return new Filter1D(new double[] {
                -21.0/231.0, 14.0/231.0, 39.0/231.0, 54.0/231.0, 59.0/231.0, 54.0/231.0, 39.0/231.0, 14.0/231.0, -21.0/231.0
            }, 4, "Hamming (Length: 9)");
        }

        public static Filter1D GetGaussianFilter()
        {
            return new Filter1D(new double[] { 0.0654, 0.2423, 0.3832, 0.2423, 0.0654
            }, 2, "Gaussian (Length: 5)");
        }

        public static Filter1D GetUnsharpMaskingGaussianFilter()
        {
            return new UnsharpMaskingFilter(GetGaussianFilter(), "Sharpening by Unsharp Masking (Gaussian - Length: 5)");
        }

        public static Filter1D GetUnsharpMaskingHammingFilter1()
        {
            return new UnsharpMaskingFilter(GetHammingFilter1(), "Sharpening by Unsharp Masking (Hamming - Length: 5)");
        }

        public static Filter1D GetUnsharpMaskingHammingFilter2()
        {
            return new UnsharpMaskingFilter(GetHammingFilter2(), "Sharpening by Unsharp Masking (Hamming - Length: 7)");
        }

        public static Filter1D GetUnsharpMaskingHammingFilter3()
        {
            return new UnsharpMaskingFilter(GetHammingFilter3(), "Sharpening by Unsharp Masking (Hamming - Length: 9)");
        }

        public static Filter1D GetMultiplicativeNoiseFilter()
        {
            return new MultiplicativeNoiseFilter1D();
        }

        public static Filter1D GetAdditiveNoiseFilter()
        {
            return new AdditiveNoiseFilter1D();
        }
    }

    class UnsharpMaskingFilter : Filter1D
    {
        private Filter1D blurFilter;

        public UnsharpMaskingFilter(Filter1D blurFilter, String name) : base(null, 0, name)
        {
            this.blurFilter = blurFilter;
        }

        public override double applyToIndex(double[] values, int index)
        {
            return 2 * values[index] - blurFilter.applyToIndex(values, index);
        }
    }

    public class MultiplicativeNoiseFilter1D : Filter1D
    {
        public MultiplicativeNoiseFilter1D() : base(null, 0, "")
        {
        }

        public override void Apply(double[] values)
        {
            throw new NotImplementedException();
        }

        public override void Apply(List<double[]> projections)
        {
            NoiseMaker.AddMultiplicativeNoise(projections, 0.1);
        }

        public override string ToString()
        {
            return "Multiplicative noise (Magnitude: 0.1)";
        }
    }

    public class AdditiveNoiseFilter1D : Filter1D
    {
        public AdditiveNoiseFilter1D() : base(null, 0, "")
        {
        }

        public override void Apply(double[] values)
        {
            throw new NotImplementedException();
        }

        public override void Apply(List<double[]> projections)
        {
            NoiseMaker.AddAdditiveNoise(projections, 0.1);
        }

        public override string ToString()
        {
            return "Additive noise (Magnitude: 0.1)";
        }
    }
}
