using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delta
{
    public class BeamWaistCalculator
    {
        #region "utility"
        public struct BeamWaistInfo
        {
            public BeamWaistInfo(double NewW0, double NewZ1, double NewZ2, double NewW1, double NewW2)
            {
                W0 = NewW0;

                Z1 = NewZ1;
                Z2 = NewZ2;

                W1 = NewW1;
                W2 = NewW2;
            }
            public double W0;
            public double Z1;

            public double Z2;
            public double W1;
            public double W2;
        }

        public struct BeamWaistInfo2D
        {
            public enum DataSelectionOption
            {
                UseX,
                UseY
                //UseAverage
            }
            public BeamWaistInfo X;

            public BeamWaistInfo Y;

            public BeamWaistInfo Average
            {
                get
                {
                    BeamWaistInfo v = default(BeamWaistInfo);
                    v.W0 = 0.5 * (X.W0 + Y.W0);
                    v.W1 = 0.5 * (X.W1 + Y.W1);
                    v.W2 = 0.5 * (X.W2 + Y.W2);
                    v.Z1 = 0.5 * (X.Z1 + Y.Z1);
                    v.Z2 = 0.5 * (X.Z2 + Y.Z2);
                    return v;
                }
            }

            public BeamWaistInfo GetActiveBeamWaistInfo(DataSelectionOption Selection)
            {
                switch (Selection)
                {
                    case DataSelectionOption.UseX:
                        return X;
                    case DataSelectionOption.UseY:
                        return Y;
                    //Case DataSelectionOption.UseAverage
                    //    Return Me.Average
                    default:
                        return default(BeamWaistInfo);
                }
            }
        }
        #endregion

        public double Lamda { get; set; }

        public BeamWaistCalculator()
        {
        }

        public BeamWaistCalculator(double Wavelength)
        {
            Lamda = Wavelength;
        }

        public BeamWaistInfo CalculateWaist(double DeltaZ, double W1, double W2)
        {
            double A = 0;
            double B = 0;
            double C = 0;
            double W1S = 0;
            double W2S = 0;
            double LamdaZpi = 0;
            double v = 0;
            double v1 = 0;
            double v2 = 0;
            BeamWaistInfo result = default(BeamWaistInfo);

            //passdown
            result.W1 = W1;
            result.W2 = W2;

            //calculate
            LamdaZpi = Lamda * DeltaZ / Math.PI;
            W1S = Math.Pow(W1, 2);
            W2S = Math.Pow(W2, 2);

            v = (W1S - W2S) / LamdaZpi / 2;
            A = 1 + Math.Pow(v, 2);
            B = -0.5 * (W1S + W2S);
            C = Math.Pow((LamdaZpi / 2), 2);

            v = Math.Sqrt(Math.Pow(B, 2) - 4 * A * C);
            v -= B;
            v /= (2 * A);
            v = Math.Sqrt(v);

            result.W0 = v;

            A = Math.PI * v / Lamda;
            v = Math.Pow(v, 2);
            v1 = A * Math.Sqrt(W1S - v);
            v2 = A * Math.Sqrt(W2S - v);

            result.Z1 = Math.Min(v1, v2);
            result.Z2 = Math.Max(v1, v2);


            return result;
        }

        public double GetWidthAtZ(double W0, double Z)
        {
            double v = 0;

            v = Lamda * Z / Math.PI / Math.Pow(W0, 2);
            v = 1 + Math.Pow(v, 2);
            v = W0 * Math.Sqrt(v);

            return v;
        }

        public double GetZfromWidth(double W0, double W)
        {
            double v = 0;

            v = Math.Sqrt(Math.Pow(W, 2) - Math.Pow(W0, 2));
            v = Math.PI * W0 / Lamda / v;

            return v;
        }

    }

    public class LIV
    {
        public static double FitThresholdCurrent(double[] Current, double[] Power)
        {
            int i = 0;
            int ii = 0;
            double idx = 0;


            ii = Current.Length - 1;
            double[] D1 = new double[ii];
            double[] D2 = new double[ii];



            //1st derivative
            for (i = 0; i <= ii - 1; i++)
            {
                D1[i] = Power[i + 1] - Power[i];
                D1[i] /= (Current[i + 1] - Current[i]);
            }

            //2nd derivative
            for (i = 1; i <= ii - 1; i++)
            {
                D2[i] = D1[i] - D1[i - 1];
            }

            //get peak from 2nd derivativee,
            idx = w2PeakDetector.GetDominantPeakIndex(D2);
            return w2Array.GetValueAtIndex(Current, idx);
        }

        public static double FitThresholdCurrent(double[] Current, double[] Power, double StartCurrent, double EndCurrent)
        {
            int i = 0;
            int ii = 0;
            double idx = 0;
            double diff = 0;
            double max = 0;
            double[] peakIndex = null;
            List<double> D1 = default(List<double>);
            List<double> D2 = default(List<double>);

            ii = Current.Length - 1;
            D1 = new List<double>();
            D2 = new List<double>();

            //1st derivative
            for (i = 0; i <= ii - 1; i++)
            {
                if (Current[i] >= StartCurrent && Current[i + 1] <= EndCurrent)
                {
                    diff = Power[i + 1] - Power[i];
                    diff /= (Current[i + 1] - Current[i]);
                    D1.Add(diff);
                }
            }

            //2nd derivative
            for (i = 1; i <= D1.Count - 1; i++)
            {
                D2.Add(D1[i] - D1[i-1]);
            }

            //get peak from 2nd derivativee,
            //idx = w2PeakDetector.GetDominantPeakIndex(D2.ToArray())
            peakIndex = w2PeakDetector.GetPeakIndexes(D2.ToArray());
            max = double.MinValue;
            foreach (int i_loopVariable in peakIndex)
            {
                i = i_loopVariable;
                if (D2[i] > max)
                    max = D2[i];
            }

            foreach (int i_loopVariable in peakIndex)
            {
                i = i_loopVariable;
                if (D2[i] > 0.3 * max)
                {
                    idx = i;
                    break; // TODO: might not be correct. Was : Exit For
                }
            }

            return w2Array.GetValueAtIndex(Current, idx);
        }
    }
}
