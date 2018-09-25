using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delta
{
    public class w2PeakDetector
    {
        private const double DefaultCoef = 0.5;

        private const int DefaultExclusion = 8;
        private List<double> mValue;
        private List<double> mFV;
        private List<double> mDV;
        private double mCoef;
        private int mExclusion;
        private bool mHavePeak;
        private bool mHaveValley;
        private double mMin;

        private double mMax;
        public w2PeakDetector()
        {
            mValue = new List<double>();
            mFV = new List<double>();
            mDV = new List<double>();
            mCoef = DefaultCoef;
            mExclusion = DefaultExclusion;
        }

        #region "properties"
        public double[] Data
        {
            get { return mValue.ToArray(); }
        }

        public double LowPassCoefficient
        {
            get { return mCoef; }
            set { mCoef = value; }
        }

        public int Exclusion
        {
            get { return mExclusion; }
            set { mExclusion = value; }
        }

        public bool HavePeak
        {
            get { return mHavePeak; }
        }

        public bool HaveValley
        {
            get { return mHaveValley; }
        }

        public int FirstPeakIndex
        {
            get
            {
                if (!mHavePeak)
                    return -1;
                return GetIndex(true);
            }
        }

        public int FirstValleyIndex
        {
            get
            {
                if (!mHaveValley)
                    return -1;
                return GetIndex(false);
            }
        }

        private int GetIndex(bool NeedPeak)
        {
            int i = 0;
            int ii = 0;
            double[] v = null;
            double[] dV = null;

            ii = mFV.Count - 1;
            v = new double[ii + 1];
            dV = new double[ii + 1];

            //do a backward filtering
            v[ii] = mFV[ii];
            //mFV[0] is singularity, ignore
            for (i = ii - 1; i >= 1; i += -1)
            {
                v[i] = mCoef * mFV[i] + (1.0 - mCoef) * v[i+1];
            }

            //calculate the difference, note the valid value is from index 1 to ii-1
            dV[1] = v[2] - v[1];
            for (i = 2; i <= ii - 2; i++)
            {
                dV[i] = v[i+1] - v[i];
                if (dV[i] * dV[i-1] <= 0)
                {
                    //dV[i] = V(i+1) - v[i], so i is the center, return i will reflect the peak/valley
                    if (dV[i] < 0 || dV[i-1] > 0)
                    {
                        if (NeedPeak)
                            return i;
                    }
                    else
                    {
                        if ((!NeedPeak))
                            return i;
                    }
                }
            }

            return -1;
        }

        public double Min
        {
            get { return mMin; }
        }

        public double Max
        {
            get { return mMax; }
        }

        #endregion

        public void Add(double value)
        {
            int i = 0;

            //add the original value
            mValue.Add(value);

            //get min/max
            if (mValue.Count == 1)
            {
                mMin = value;
                mMax = value;
            }
            else if (mMin > value)
            {
                mMin = value;
            }
            else if (mMax < value)
            {
                mMax = value;
            }
           
            //calculate the filtered value, and its delta
            i = mValue.Count - 1;
            if (i == 0)
            {
                mFV.Add(value);
            }
            else
            {
                value = mCoef * value + (1.0 - mCoef) * mFV[i-1];
                mFV.Add(value);
                //calculate delat
                if (i > 1)
                {
                    //first i-1 equals 1, mFV[0] is a singularity without filtering, ignore 
                    value = mFV[i] - mFV[i-1];
                    mDV.Add(value);
                }
                //check peak valley
                if (i > mExclusion)
                {
                    i = mDV.Count - 1;
                    if (i > 0)
                    {
                        if ((mDV[i] * mDV[i-1]) <= 0)
                        {
                            if (mDV[i] < 0 || mDV[i-1] > 0)
                            {
                                mHavePeak = true;
                                //peak makes last point negative
                            }
                            else
                            {
                                mHaveValley = true;
                                //valley makes last point positive
                            }
                        }
                    }
                }

            }
        }

        public void Add(double[] values)
        {
            foreach (double value in values)
            {
                this.Add(values);
            }
        }

        public void ClearPeak()
        {
            mHavePeak = false;
        }

        public void ClearValley()
        {
            mHaveValley = false;
        }

        public void Clear()
        {
            mValue.Clear();
            mFV.Clear();
            mDV.Clear();
            mHavePeak = false;
            mHaveValley = false;
        }

        #region "peak valley functions"
        public double[] GetPeakIndexes()
        {
            double[] p = new double[mValue.Count];
            double[] v = new double[mValue.Count];

            GetIndexes(mValue.ToArray(), mCoef, ref p, ref v);
            return p;
        }

        public double[] GetValleyIndexes()
        {
            double[] p = new double[mValue.Count];
            double[] v = new double[mValue.Count];

            GetIndexes(mValue.ToArray(), mCoef, ref p, ref v);
            return v;
        }

        public void GetIndexes(ref double[] PeakIndex, ref double[] ValleyIndex)
        {
            GetIndexes(mValue.ToArray(), mCoef, ref PeakIndex, ref ValleyIndex);
        }
        #endregion

        #region "shared functions"
        public static double[] GetPeakIndexes(double[] value)
        {
            return GetPeakIndexes(value, DefaultCoef);
        }

        public static double[] GetPeakIndexes(double[] value, double FilterCoef)
        {
            double[] p = new double[value.Length];
            double[] v = new double[value.Length];

            GetIndexes(value, FilterCoef, ref p, ref v);
            return p;
        }

        public static double[] GetValleyIndexes(double[] value)
        {
            return GetValleyIndexes(value, DefaultCoef);
        }

        public static double[] GetValleyIndexes(double[] value, double FilterCoef)
        {
            double[] p = new double[value.Length];
            double[] v = new double[value.Length];

            GetIndexes(value, FilterCoef, ref p, ref v);
            return v;
        }

        public static void GetIndexes(double[] value, ref double[] PeakIndex, ref double[] ValleyIndex)
        {
            GetIndexes(value, DefaultCoef, ref PeakIndex, ref ValleyIndex);
        }

        public static void GetIndexes(double[] value, double FilterCoef, ref double[] PeakIndex, ref double[] ValleyIndex)
        {
            int i = 0;
            int ii = 0;
            double[] x = null;
            double[] dx = null;
            double iZero = 0;
            dynamic p = default(List<double>);
            List<double> v = new List<double>();

            ii = value.Length - 1;
            if (ii < 2)
                return;

            x = new double[ii + 1];
            dx = new double[ii + 1];

            //forward filtering
            x[0] = value[0];
            for (i = 1; i <= ii; i++)
            {
                x[i] = FilterCoef * value[i] + (1.0 - FilterCoef) * x[i-1];
            }

            //backward filtering
            for (i = ii - 1; i >= 1; i += -1)
            {
                x[i] = FilterCoef * x[i] + (1.0 - FilterCoef) * x[i+1];
            }

            //get difference
            for (i = 1; i <= ii - 2; i++)
            {
                dx[i] = x[i+1] - x[i];
            }

            //check zero crossing
            for (i = 2; i <= ii - 2; i++)
            {
                if (dx[i] * dx[i-1] < 0)
                {
                    //get the partial index where d will be zero
                    iZero = dx[i] - dx[i-1];
                    iZero = dx[i-1] / iZero;
                    iZero = (i-1) - iZero;
                    iZero += 0.5;
                    //add the zero because if the two are equal, the peak is in the middle of the two
                    if (dx[i] < 0 || dx[i-1] > 0)
                    {
                        p.Add(iZero);
                    }
                    else
                    {
                        v.Add(iZero);
                    }
                }
            }

            //return
            PeakIndex = p.ToArray();
            ValleyIndex = v.ToArray();
        }

        #region "Dominant"
        public static double GetDominantPeakIndex(double[] Value)
        {
            double p = 0;
            double v = 0;
            GetDominantPeakValleyIndex(Value, DefaultCoef, ref p, ref v);
            return p;
        }

        public static double GetDominantValleyIndex(double[] Value)
        {
            double p = 0;
            double v = 0;
            GetDominantPeakValleyIndex(Value, DefaultCoef, ref p, ref v);
            return v;
        }

        public static double GetDominantPeakIndex(double[] Value, double FilterCoef)
        {
            double p = 0;
            double v = 0;
            GetDominantPeakValleyIndex(Value, FilterCoef, ref p, ref v);
            return p;
        }

        public static double GetDominantValleyIndex(double[] Value, double FilterCoef)
        {
            double p = 0;
            double v = 0;
            GetDominantPeakValleyIndex(Value, FilterCoef, ref p, ref v);
            return v;
        }

        public static void GetDominantPeakValleyIndex(double[] value, ref double PeakIndex, ref double ValleyIndex)
        {
            GetDominantPeakValleyIndex(value, DefaultCoef, ref PeakIndex, ref ValleyIndex);
        }

        public static void GetDominantPeakValleyIndex(double[] value, double FilterCoef, ref double PeakIndex, ref double ValleyIndex)
        {
            double Min = 0;
            double Max = 0;
            int iMin = 0;
            int iMax = 0;
            int i = 0;
            int idx = 0;
            double Diff = 0;
            double MinDiff = 0;
            double[] p = new double[value.Length];
            double[] v = new double[value.Length];

            //get simple peak and valley
            w2Array.MinMax(value, ref Min, ref iMin,ref Max, ref iMax);

            //get all peak and valley
            GetIndexes(value, FilterCoef, ref p, ref v);

            //find the peak index closest to iMax
            switch (p.Length)
            {
                case 0:
                    PeakIndex = double.NaN;
                    break;
                case 1:
                    PeakIndex = p[0];
                    break;
                default:
                    for (i = 0; i <= p.Length - 1; i++)
                    {
                        Diff = Math.Abs(p[i] - iMax);
                        if (i == 0)
                        {
                            MinDiff = Diff;
                            idx = i;
                        }
                        else if (MinDiff > Diff)
                        {
                            MinDiff = Diff;
                            idx = i;
                        }
                    }

                    PeakIndex = p[idx];
                    break;
            }

            //find the valley index closest to iMin
            switch (v.Length)
            {
                case 0:
                    ValleyIndex = double.NaN;
                    break;
                case 1:
                    ValleyIndex = v[0];
                    break;
                default:
                    for (i = 0; i <= v.Length - 1; i++)
                    {
                        Diff = Math.Abs(v[i] - iMin);
                        if (i == 0)
                        {
                            MinDiff = Diff;
                            idx = i;
                        }
                        else if (MinDiff > Diff)
                        {
                            MinDiff = Diff;
                            idx = i;
                        }
                    }

                    ValleyIndex = v[idx];
                    break;
            }

        }
        #endregion
        #endregion
    }
}
