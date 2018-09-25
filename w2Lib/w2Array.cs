using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Windows.Forms;

namespace Delta
{
    public class w2Array
    {
        #region "Simple algebra"
        public static double[] Abs(double[] Input)
        {
            double[] Output = null;
            int i = 0;
            int ii = 0;
            ii = Input.Length - 1;
            Output = new double[ii + 1];
            for (i = 0; i <= ii; i++)
            {
                Output[i] = Math.Abs(Input[i]);
            }
            return Output;
        }

        public static double[] Abs(List<double> Input)
        {
            return Abs(Input.ToArray());
        }

        public static double[] ArrayDelta(double[] Input, int Offset = 1)
        {
            double[] Output = null;
            int i = 0;
            int ii = 0;

            double sign = 1;
            if (Offset < 0)
            {
                sign = -1;
                Offset = -Offset;
            }

            ii = Input.Length - 1 - Offset;
            Output = new double[ii + 1];
            for (i = 0; i <= ii; i++)
            {
                Output[i] = (Input[i + Offset] - Input[i]) * sign;
            }

            return Output;
        }

        public static double[] ArrayDelta(List<double> Input, int Offset = 1)
        {
            return ArrayDelta(Input.ToArray());
        }

        public static double[] ArrayDeltaAbs(double[] Input, int Offset = 1)
        {
            double[] Output = null;
            int i = 0;
            int ii = 0;

            if (Offset < 0)
                Offset = -Offset;

            ii = Input.Length - 1 - Offset;
            Output = new double[ii + 1];
            for (i = 0; i <= ii; i++)
            {
                Output[i] = Math.Abs(Input[i + Offset] - Input[i]);
            }

            return Output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Offset"></param>
        /// <returns></returns>
        /// <remarks>Percentage is normalized to the larger of the two</remarks>
        public static double[] ArrayDeltaPercentageAbs(double[] Input, int Offset = 1)
        {
            double[] Output = null;
            int i = 0;
            int ii = 0;

            if (Offset < 0)
                Offset = -Offset;

            ii = Input.Length - 1 - Offset;
            Output = new double[ii + 1];
            for (i = 0; i <= ii; i++)
            {
                Output[i] = Input[i + Offset] - Input[i];
                Output[i] /= Math.Max(Math.Abs(Input[i]), Math.Abs(Input[i + Offset]));
                Output[i] *= 100.0;
                Output[i] = Math.Abs(Output[i]);
            }

            return Output;
        }

        public static double[] ArrayDeltaPercentage(double[] Input, int Offset = 1)
        {
            double[] Output = null;
            int i = 0;
            int ii = 0;

            double sign = 1;
            if (Offset < 0)
            {
                sign = -1;
                Offset = -Offset;
            }

            ii = Input.Length - 1 - Offset;
            Output = new double[ii + 1];
            for (i = 0; i <= ii; i++)
            {
                Output[i] = sign * (Input[i + Offset] - Input[i]);
                Output[i] /= Input[i];
                Output[i] *= 100.0;
            }

            return Output;
        }

        public static double[] Add(double[] Input, double Value)
        {
            int i = 0;
            int ii = 0;
            ii = Input.Length - 1;

            double[] Output = new double[ii + 1];
            for (i = 0; i <= ii; i++)
            {
                Output[i] = Input[i] + Value;
            }

            return Output;
        }

        public static double[] Add(double[] A, double[] B)
        {
            int i = 0;
            int ii = 0;
            ii = Math.Min(A.Length - 1, B.Length - 1);

            double[] Output = new double[ii + 1];
            for (i = 0; i <= ii; i++)
            {
                Output[i] = A[i] + B[i];
            }

            return Output;
        }

        public static double[] Add(List<double> Input, double Value)
        {
            return Add(Input.ToArray(), Value);
        }

        public static double[] Subtract(double[] input, double Value)
        {
            return Add(input, -Value);
        }

        public static double[] Subtract(double[] A, double[] B)
        {
            int i = 0;
            int ii = 0;
            ii = Math.Min(A.Length - 1, B.Length - 1);

            double[] Output = new double[ii + 1];
            for (i = 0; i <= ii; i++)
            {
                Output[i] = A[i] - B[i];
            }

            return Output;
        }

        public static double[] Subtract(List<double> input, double Value)
        {
            return Add(input, -Value);
        }

        /// <summary>
        /// Replace data in the input array with the NullValue if the data is 
        /// inside range of [Min, Max] inclusive
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="NullValue">the value used to replac the nulled data</param>
        /// <param name="Min"></param>
        /// <param name="Max"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double[] NullDataInArray(double[] Input, double NullValue, double Min, double Max)
        {
            int i = 0;
            int ii = 0;
            ii = Input.Length - 1;

            double[] Output = new double[ii + 1];
            for (i = 0; i <= ii; i++)
            {
                if ( Input[i] >= Min &&  Input[i] <= Max)
                {
                    Output[i] = NullValue;
                }
                else
                {
                    Output[i] =  Input[i];
                }
            }

            return Output;
        }
        #endregion

        #region "Mean, Min, Max"
        public static double AbsoluteMaximum(double[] Input)
        {
            double Min = 0;
            double Max = 0;
            MinMax(Input, ref Min, ref Max);
            return Math.Max(Math.Abs(Min), Max);
        }

        public static double AbsoluteMaximum(List<double> Input)
        {
            return AbsoluteMaximum(Input.ToArray());
        }

        public static double AbsoluteMinimum(double[] Input)
        {
            return Minimum(Abs(Input));
        }

        public static double AbsoluteMinimum(List<double> Input)
        {
            return AbsoluteMaximum(Input.ToArray());
        }

        public static double AbsoluteMaximumSigned(double[] Input)
        {
            double Min = 0;
            double Max = 0;
            MinMax(Input, ref Min, ref Max);
            if (Math.Abs(Min) < Max)
            {
                return Max;
            }
            else
            {
                return Min;
            }
        }

        public static double AbsoluteMaximumSigned(List<double> Input)
        {
            return AbsoluteMaximumSigned(Input.ToArray());
        }

        public static double Maximum(double[] Input)
        {
            double Min = 0;
            double Max = 0;
            MinMax(Input, ref Min, ref Max);
            return Max;
        }

        public static double Maximum(List<double> Input)
        {
            double Min = 0;
            double Max = 0;
            MinMax(Input, ref Min, ref Max);
            return Max;
        }

        public static double Mean(double[] Input)
        {
            if (Input.Length == 0)
                return 0;
            double Sum = 0;
            int Num = 0;
            foreach (double v in Input)
            {
                if (!double.IsNaN(v))
                {
                    Sum += v;
                    Num += 1;
                }
            }
            return Sum / Num;
        }

        public static double Mean(List<double> Input)
        {
            return Mean(Input.ToArray());
        }

        public static double Minimum(double[] Input)
        {
            double Min = 0;
            double Max = 0;
            MinMax(Input, ref Min, ref Max);
            return Min;
        }

        public static double Minimum(List<double> Input)
        {
            double Min = 0;
            double Max = 0;
            MinMax(Input, ref Min, ref Max);
            return Min;
        }

        public static void MinMax(double[] Input, ref double Min, ref int iMin, ref double Max, ref int iMax)
        {
            int i = 0;
            foreach (double v in Input)
            {
                if (!double.IsNaN(v))
                {
                    if (i == 0)
                    {
                        //initialize value
                        //i = 1
                        Min = v;
                        iMin = i;
                        Max = v;
                        iMax = i;
                    }
                    else
                    {
                        //check min, max now
                        if (v < Min)
                        {
                            Min = v;
                            iMin = i;
                        }
                        else if (v > Max)
                        {
                            Max = v;
                            iMax = i;
                        }
                    }
                    i += 1;
                }
            }
        }

        public static void MinMax(double[] Input, ref double Min, ref double Max)
        {
            int i = 0;
            int j = 0;
            MinMax(Input, ref Min, ref i, ref Max, ref j);
        }

        public static void MinMax(List<double> Input, ref double Min, ref double Max)
        {
            MinMax(Input.ToArray(), ref Min, ref Max);
        }

        public static void MinMaxAverage(double[] Input, ref double Min, ref double Max, ref double Average)
        {
            int i = 0;

            Average = 0.0;
            foreach (double v in Input)
            {
                if (!double.IsNaN(v))
                {
                    if (i == 0)
                    {
                        //initialize value
                        Min = v;
                        Max = v;
                    }
                    else
                    {
                        //check min, max now
                        if (v < Min)
                        {
                            Min = v;
                        }
                        else if (v > Max)
                        {
                            Max = v;
                        }
                    }
                    //sum
                    Average += v;
                    i += 1;
                }
            }
            Average = Average / i;
        }

        public static void MinMaxAverage(Array Input, ref double Min, ref double Max, ref double Average)
        {
            int i = 0;

            Average = 0.0;
            foreach (double v in Input)
            {
                if (!double.IsNaN(v))
                {
                    if (i == 0)
                    {
                        //initialize value
                        Min = v;
                        Max = v;
                    }
                    else
                    {
                        //check min, max now
                        if (v < Min)
                        {
                            Min = v;
                        }
                        else if (v > Max)
                        {
                            Max = v;
                        }
                    }
                    //sum
                    Average += v;
                    i += 1;
                }
            }
            Average = Average / i;
        }

        public static void MinMaxAverage(List<double> Input, ref double Min, ref double Max, ref double Average)
        {
            MinMaxAverage(Input.ToArray(), ref Min, ref Max, ref Average);
        }

        public static double Range(double[] Input)
        {
            double Min = 0;
            double Max = 0;
            MinMax(Input, ref Min, ref Max);
            return (Max - Min);
        }

        public static double Range(List<double> Input)
        {
            double Min = 0;
            double Max = 0;
            MinMax(Input, ref Min, ref Max);
            return (Max - Min);
        }
        #endregion

        #region "Mean, StdEv, R"
        public static double StandardDeviation(double[] Input)
        {
            double S0 = 0.0;
            double S1 = 0.0;
            double S2 = 0.0;
            foreach (double v in Input)
            {
                if (!double.IsNaN(v))
                {
                    S0 += 1;
                    S1 += v;
                    S2 += Math.Pow(v, 2);
                }
            }
            return Math.Sqrt(S0 * S2 - Math.Pow(S1, 2)) / S0;
        }

        public static void MeanAndStandardDeviation(double[] Input, ref double Mean, ref double Stdev)
        {
            double S0 = 0.0;
            double S1 = 0.0;
            double S2 = 0.0;
            foreach (double v in Input)
            {
                S0 += 1;
                S1 += v;
                S2 += Math.Pow(v, 2);
            }

            Mean = S1 / S0;
            Stdev = Math.Sqrt(S0 * S2 - Math.Pow(S1, 2)) / S0;
        }

        //calculate the "R"
        public static double CalculateR(double[] vData, double MeanSquareError)
        {
            double v = 0;
            double va = 0;

            //mean
            va = w2Array.Mean(vData);
            //variance
            v = 0;
            for (int i = 0; i <= vData.Length - 1; i++)
            {
                v += Math.Pow((vData[i] - va), 2);
            }
            //R
            return 1.0 - MeanSquareError * vData.Length / v;
        }
        #endregion

        #region "Counting"
        public static int SingChanges(double[] Input)
        {
            int i = 0;
            int ii = 0;
            int c = 0;

            ii = Input.Length - 2;
            c = 0;
            for (i = 0; i <= ii; i++)
            {
                if (Math.Sign( Input[i]) * Math.Sign( Input[i+1]) < 0)
                    c += 1;
            }

            return c;
        }

        public static int SingChanges(List<double> Input)
        {
            return SingChanges(Input.ToArray());
        }
        #endregion

        #region "Digital filter"
        public static double[] LowPassFilter(double Coef, double[] Input)
        {
            int i = 0;
            int ii = 0;
            //copy the data
            double[] Output = Input;
            ii = Output.Length - 1;
            //forward
            for (i = 1; i <= ii; i++)
            {
                Output[i] = Coef * Output[i] + (1.0 - Coef) * Output[i - 1];
            }
            //backward
            for (i = ii - 1; i >= 0; i += -1)
            {
                Output[i] = Coef * Output[i] + (1.0 - Coef) * Output[i+1];
            }
            //return
            return Output;
        }

        #endregion

        #region "Sort/random"
        public static void SortUnique(List<double> Data, ref double[] QuiqueData, ref double[] Count)
        {
            //copy to a new list and then sort
            List<double> L = new List<double>();
            L.AddRange(Data);
            L.Sort();
            //check unique
            List<double> x = new List<double>();
            List<double> y = new List<double>();
            double v = L[0];
            int c = 1;
            for (int i = 1; i <= L.Count - 1; i++)
            {
                if (L[i] == v)
                {
                    c += 1;
                }
                else
                {
                    x.Add(v);
                    y.Add(c);

                    v = L[i];
                    c = 1;
                }
            }
            //last
            x.Add(v);
            y.Add(c);
            //return
            QuiqueData = x.ToArray();
            Count = y.ToArray();
        }

        public static void SortByKeyUniqueMemberCount(List<double> Key, List<double> Data, ref double[] UniqueKey, ref double[] UniqueMemberCount)
        {
            //copy array
            double[] x = Key.ToArray();
            double[] y = Data.ToArray();
            //sort array
            Array.Sort(x, y);
            //average those are the same
            List<double> Lx = new List<double>();
            List<double> Ly = new List<double>();
            List<double> L = new List<double>();
            double xKey = 0;
            //Dim Num As Integer
            int i = 0;
            int ii = 0;

            ii = x.Length - 1;
            xKey = x[0];
            L.Add(y[0]);
            for (i = 1; i <= ii; i++)
            {
                if (x[i] == xKey)
                {
                    if (!L.Contains(y[i]))
                        L.Add(y[i]);
                }
                else
                {
                    //save last
                    Lx.Add(xKey);
                    Ly.Add(L.Count);
                    //prepare for next
                    xKey = x[i];
                    L.Clear();
                    L.Add(y[i]);
                }
            }
            //add the last pair
            Lx.Add(xKey);
            Ly.Add(L.Count);

            //return
            UniqueKey = Lx.ToArray();
            UniqueMemberCount = Ly.ToArray();
        }

        public static void SortByKeyAverage(List<double> Key, List<double> Data, ref double[] UniqueKey, ref double[] DataAverage)
        {
            //copy array
            double[] x = Key.ToArray();
            double[] y = Data.ToArray();
            //sort array
            Array.Sort(x, y);
            //average those are the same
            List<double> Lx = new List<double>();
            List<double> Ly = new List<double>();
            double aKey = 0;
            double Sum = 0;
            int Num = 0;
            int i = 0;
            int ii = 0;

            ii = x.Length - 1;
            aKey = x[0];
            Sum = y[0];
            Num = 1;
            for (i = 1; i <= ii; i++)
            {
                if (x[i] == aKey)
                {
                    //sum
                    Sum += y[i];
                    Num += 1;
                }
                else
                {
                    //save last
                    Lx.Add(aKey);
                    Ly.Add(Sum / Num);
                    //prepare for next
                    aKey = x[i];
                    Sum = y[i];
                    Num = 1;
                }
            }
            //add the last pair
            Lx.Add(aKey);
            Ly.Add(Sum / Num);

            //return
            UniqueKey = Lx.ToArray();
            DataAverage = Ly.ToArray();
        }

        public static void SortByKeyMinMax(List<double> Key, List<double> Data, ref double[] UniqueKey, ref double[] DataMin, ref double[] DataMax)
        {
            //copy array
            double[] x = Key.ToArray();
            double[] y = Data.ToArray();
            //sort array
            Array.Sort(x, y);
            //average those are the same
            List<double> Lx = new List<double>();
            List<double> Lmin = new List<double>();
            List<double> Lmax = new List<double>();
            double aKey = 0;
            double Min = 0;
            double Max = 0;
            int i = 0;
            int ii = 0;

            ii = x.Length - 1;
            aKey = x[0];
            Min = y[0];
            Max = y[0];
            for (i = 1; i <= ii; i++)
            {
                if (x[i] == aKey)
                {
                    //check
                    if (y[i] > Max)
                    {
                        Max = y[i];
                    }
                    else if (y[i] < Min)
                    {
                        Min = y[i];
                    }
                }
                else
                {
                    //save last
                    Lx.Add(aKey);
                    Lmin.Add(Min);
                    Lmax.Add(Max);
                    //prepare for next
                    aKey = x[i];
                    Min = y[i];
                    Max = y[i];
                }
            }
            //add the last pair
            Lx.Add(aKey);
            Lmin.Add(Min);
            Lmax.Add(Max);

            UniqueKey = Lx.ToArray();
            DataMin = Lmin.ToArray();
            DataMax = Lmax.ToArray();
        }

        public static void SortByKeyRange(List<double> Key, List<double> Data, ref double[] UniqueKey, ref double[] DataRange)
        {
            double[] Min = new double[Data.Count];
            double[] Max = new double[Data.Count];
            SortByKeyMinMax(Key, Data, ref UniqueKey, ref Min, ref Max);
            DataRange = Subtract(Max, Min);
        }

        public static List<T> Shuffle<T>(List<T> OldList)
        {
            int k = 0;
            int i = 0;
            int ii = 0;
            T tmp = default(T);
            Random Ran = new Random();

            //copy the old list to new list to preserve the oldlist
            List<T> NewList = new List<T>(OldList);

            //shuffle the new list, 
            ii = NewList.Count - 1;
            for (i = ii; i >= 0; i += -1)
            {
                k = Ran.Next(i);
                tmp = NewList[i];
                NewList[i] = NewList[k];
                NewList[k] = tmp;
            }

            return NewList;
        }

        public static T[] Shuffle<T>(T[] OldArray)
        {
            return Shuffle(new List<T>(OldArray)).ToArray();
        }

        #endregion

        #region "get value from non-integer index"
        public static double GetValueAtIndex(double[] Data, double Index)
        {
            int i = 0;
            i = Convert.ToInt32(Math.Floor(Index));
            return Data[i] + (Index - i) * (Data[i+1] - Data[i]);
        }
        #endregion

        #region "get a value from a curve"
        /// <summary>
        /// Given a curve defined by X() and Y(), get all the Y values corresponding to X0
        /// using linear interpolation bteween two nearest neighbours
        /// </summary>
        /// <param name="X0"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns>zero length array if nothing is found</returns>
        /// <remarks></remarks>
        public static double[] GetYAll(double X0, double[] X, double[] Y)
        {
            int i = 0;
            int ii = 0;
            double v1 = 0;
            double v2 = 0;
            List<double> Y0 = new List<double>();

            ii = X.Length - 1;
            for (i = 0; i <= ii - 1; i++)
            {
                v1 = X0 - X[i];
                if (v1 == 0)
                {
                    Y0.Add(Y[i]);
                    continue;
                }

                v2 = X0 - X[i+1];
                if (v2 == 0)
                    continue;
                //let the next iteration handle this, otherwise it will be double counted

                if (v1 * v2 < 0)
                {
                    v1 *= Y[i] - Y[i+1];
                    v1 /= X[i] - X[i+1];
                    //X[i] - X[i+1] <> 0 because v1 * v2 < 0
                    v1 += Y[i];
                    Y0.Add(v1);
                }
            }

            return Y0.ToArray();
        }

        /// <summary>
        /// Given a curve defined by X() and Y(), get the first Y values corresponding to X0
        /// </summary>
        /// <param name="X0"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns>return NaN if nothing is found</returns>
        /// <remarks></remarks>
        public static double GetYFirst(double X0, double[] X, double[] Y)
        {
            return GetY(0, X0, X, Y, true);
        }

        /// <summary>
        /// Given a curve defined by X() and Y(), get the first Y values corresponding to X0
        /// in the range from StartIndex to the end of the array
        /// </summary>
        /// <param name="StartIndex"></param>
        /// <param name="X0"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double GetYFirst(int StartIndex, double X0, double[] X, double[] Y)
        {
            return GetY(StartIndex, X0, X, Y, true);
        }

        /// <summary>
        /// Given a curve defined by X() and Y(), get the last Y values corresponding to X0
        /// </summary>
        /// <param name="X0"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns>return NaN if nothing is found</returns>
        /// <remarks></remarks>
        public static double GetYLast(double X0, double[] X, double[] Y)
        {
            return GetY(X.Length, X0, X, Y, false);
        }

        /// <summary>
        /// Given a curve defined by X() and Y(), get the last Y values corresponding to X0
        /// in the array range from 0 to EndIndex
        /// </summary>
        /// <param name="EndIndex"></param>
        /// <param name="X0"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double GetYLast(int EndIndex, double X0, double[] X, double[] Y)
        {
            return GetY(EndIndex, X0, X, Y, false);
        }

        private static double GetY(int StartIndex, double X0, double[] X, double[] Y, bool SearchForward)
        {
            int i = 0;
            int j = 0;
            int iMax = 0;
            int iStart = 0;
            int iStop = 0;
            int iStep = 0;
            double v1 = 0;
            double v2 = 0;

            iMax = X.Length - 2;
            //-1 for upper bound, -1 again for (i+1) 


            if (StartIndex < 0)
            {
                StartIndex = 0;
            }
            else if (StartIndex > iMax)
            {
                StartIndex = iMax;
            }
            iStart = StartIndex;

            if (SearchForward)
            {
                iStop = iMax;
                iStep = 1;
            }
            else
            {
                iStop = 0;
                iStep = -1;
            }

            for (i = iStart; i <= iStop; i += iStep)
            {
                v1 = X0 - X[i];
                if (v1 == 0)
                    return Y[i];

                v2 = X0 - X[i+1];
                if (v2 == 0)
                    return Y[i+1];

                if (v1 * v2 < 0)
                {
                    v1 *= Y[i] - Y[i+1];
                    v1 /= X[i] - X[i+1];
                    //X[i] - X[i+1] <> 0 because v1 * v2 < 0
                    v1 += Y[i];
                    return v1;
                }
            }

            //X0 outside iStart, iStop
            //use nearest end points to estimate Y0
            //note that iMax is not the end points
            if (iStop == iMax)
                iStop = iMax + 1;
            if (iStart == iMax)
                iStart = iMax + 1;

            v1 = X0 - X[iStart];
            v2 = X0 - X[iStop];
            if (Math.Abs(v1) <= Math.Abs(v2))
            {
                i = iStart;
                j = iStart + iStep;
            }
            else
            {
                i = iStop;
                j = iStop - iStep;
            }

            v1 = X0 - X[i];
            v1 *= Y[i] - Y[j];
            v1 /= X[i] - X[j];
            //X[i] - X[i+1] <> 0 because = 0 case is already treated
            v1 += Y[i];
            return v1;

        }

        #endregion

        #region "Scan parameter list"
        public static double[] BuildScanList(double vStart, double vStop, double vStep)
        {
            List<double> vList = new List<double>();
            double v1 = 0;
            double v2 = 0;
            double v = 0;

            if (vStep == 0)
            {
                return new double[] { vStart, vStop };
            }
            else if (vStep > 0)
            {
                v1 = Math.Min(vStart, vStop);
                v2 = Math.Max(vStart, vStop);
                v = v1;
                while (v <= v2)
                {
                    vList.Add(v);
                    v += vStep;
                }
            }
            else if (vStep < 0)
            {
                v1 = Math.Max(vStart, vStop);
                v2 = Math.Min(vStart, vStop);
                v = v1;
                while (v >= v2)
                {
                    vList.Add(v);
                    v += vStep;
                }
            }

         
            //make sure end is included
            if (vList.Count > 0)
            {
                if (vList[vList.Count - 1] != v2)
                    vList.Add(v2);
            }

            //return
            return vList.ToArray();
        }

        public static bool ValidateScanParameter(double vStart, double vStop, double vStep, bool ShowError)
        {
            string s = "";

            if (vStep == 0)
            {
                s = "Step cannot be zero!";
            }
            else if (vStep > 0)
            {
                if (vStart > vStop)
                {
                    s = "Inconsistent setting: Step > 0 but Start > Stop";
                }
            }
            else if (vStep < 0)
            {
                if (vStart < vStop)
                {
                    s = "Inconsistent setting: Step < 0 but Start < Stop";
                }

            }

            if (string.IsNullOrEmpty(s))
            {
                return true;
            }
            else
            {
                if (ShowError)
                {
                    s += ControlChars.CrLf + "Start: " + vStart;
                    s += ControlChars.CrLf + "Stop:  " + vStop;
                    s += ControlChars.CrLf + "Step:  " + vStep;
                    MessageBox.Show(s, "Scan parameter", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return false;
            }
        }
        #endregion

        #region "trajectory"
        public static void GetSpiral(int Length, double StepSize, double X0, double Y0, ref double[] X, ref double[] Y)
        {
            int i = 0;
            int ii = 0;
            int k = 0;
            int kk = 0;

            ii = Length - 1;
            // ERROR: Not supported in C#: ReDimStatement


            X[0] = X0;
            Y[0] = Y0;

            i = 1;
            kk = 1;
            while (i <= ii)
            {
                //move x first
                for (k = 1; k <= kk; k++)
                {
                    X[i] = X[i - 1] + StepSize;
                    Y[i] = Y[i - 1];
                    i += 1;
                    if (i > ii)
                        return;
                }
                //move y now
                for (k = 1; k <= kk; k++)
                {
                    X[i] = X[i - 1];
                    Y[i] = Y[i - 1] + StepSize;
                    i += 1;
                    if (i > ii)
                        return;
                }
                //next 
                StepSize = -1.0 * StepSize;
                kk += 1;
            }
        }

        public static void GetSpiralMatrixIndex(int Length, double StepSizeSign, ref int[] X, ref int[] Y)
        {
            int i = 0;
            int ii = 0;
            int k = 0;
            int kk = 0;
            int MinX = 0;
            int MinY = 0;
            int StepSize = 0;

            ii = Length - 1;
            // ERROR: Not supported in C#: ReDimStatement


            X[0] = 0;
            Y[0] = 0;
            StepSize = Math.Sign(StepSizeSign);
            if (StepSize == 0)
                StepSize = 1;

            MinX = X[0];
            MinY = Y[0];

            i = 1;
            kk = 1;
            while (i <= ii)
            {
                //move x first
                for (k = 1; k <= kk; k++)
                {
                    X[i] = X[i - 1] + StepSize;
                    Y[i] = Y[i - 1];
                    if (X[i] < MinX)
                        MinX = X[i];
                    if (Y[i] < MinY)
                        MinY = Y[i];
                    i += 1;
                    if (i > ii)
                        break; // TODO: might not be correct. Was : Exit While
                }
                //move y now
                for (k = 1; k <= kk; k++)
                {
                    X[i] = X[i - 1];
                    Y[i] = Y[i - 1] + StepSize;
                    if (X[i] < MinX)
                        MinX = X[i];
                    if (Y[i] < MinY)
                        MinY = Y[i];
                    i += 1;
                    if (i > ii)
                        break; // TODO: might not be correct. Was : Exit While
                }
                //next 
                StepSize = -StepSize;
                kk += 1;
            }

            //now we need to offset all the array so that they starts with (0, 0)
            for (i = 0; i <= ii; i++)
            {
                X[i] -= MinX;
                Y[i] -= MinY;
            }
        }
        #endregion

        #region "string related"
        public static string Concatenate(string Delimiter, params object[] Parts)
        {
            object x = null;
            object y = null;
            Array aArray = default(Array);
            string s = "";
            foreach (object x_loopVariable in Parts)
            {
                x = x_loopVariable;
                if (x is Array)
                {
                    aArray = (Array)x;
                    foreach (object y_loopVariable in aArray)
                    {
                        y = y_loopVariable;
                        if (y is double)
                            y = Convert.ToSingle(y);
                        s += (Delimiter + y.ToString());
                    }
                }
                else
                {
                    if (x is double)
                        x = Convert.ToSingle(x);
                    s += (Delimiter + x.ToString());
                }
            }
            if (s.StartsWith(Delimiter))
                s = s.Substring(Delimiter.Length);
            return s;
        }

        public static string Concatenate(string Delimiter, string fmt, double[] Data)
        {
            double v = 0;
            string s = "";

            foreach (double v_loopVariable in Data)
            {
                v = v_loopVariable;
                if (fmt.StartsWith("{"))
                {
                    s += Delimiter + string.Format(fmt, v);
                }
                else
                {
                    s += Delimiter + v.ToString(fmt);
                }
            }
            if (s.StartsWith(Delimiter))
                s = s.Substring(Delimiter.Length);
            return s;
        }

        //Public Shared Function SplitToInteger(ByVal s As String, ByVal Delimiter As Char) As Integer()
        //    Dim sData As String()
        //    Dim vData As Integer() = New Integer() {}
        //    Dim i, ii As Integer
        //    Dim k As Integer

        //    If s = "" Then Return vData


        //    sData = 
        //    ii = sData.Length - 1
        //    ReDim vData(ii)
        //    k = 0
        //    For i = 0 To ii
        //        Try
        //            vData[k] = Integer.Parse(sData[i])
        //            k += 1
        //        Catch ex As Exception
        //            'ignore
        //        End Try
        //    Next

        //    If k <> 0 Then k = k - 1
        //    If k < ii Then ReDim vData[k]

        //    Return vData
        //End Function
        #endregion
    }

}
