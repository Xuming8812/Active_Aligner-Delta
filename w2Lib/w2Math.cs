using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Delta
{
    public class Matrix
    {

        public static double DotProduct(double[] X, double[] Y)
        {
            int i = 0;
            int ii = 0;
            ii = Math.Min(X.Length, Y.Length) - i;

            double v = 0.0;
            for (i = 0; i <= ii; i++)
            {
                v += (X[i] * Y[i]);
            }

            return v;
        }

        public static double[] DotProduct(double[,] X, double[] Y)
        {
            int i = 0;
            int j = 0;
            int ii = 0;
            ii = Y.Length - 1;

            double[] Z = new double[ii + 1];
            for (i = 0; i <= ii; i++)
            {
                Z[i] = 0;
                for (j = 0; j <= ii; j++)
                {
                    Z[i] += (X[i, j] * Y[j]);
                }
            }

            return Z;
        }

        /// <summary>
        /// This is Gauss-Jardan matrix inversion for solving the linear equation.
        /// It is a modified version of that from Numeric Recipes 
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <returns>Inversed matrix </returns>
        /// <remarks></remarks>
        public static double[,] Inverse(double[,] A)
        {
            if (A.GetLength(0) != A.GetLength(1))
                return null;
            int ii = A.GetLength(0) - 1;

            //copy matrix 
            double[,] X = new double[ii + 1, ii + 1];
            Array.Copy(A, X, A.Length);

            //local variables
            double big = 0;
            double tmp = 0;
            double pivinv = 0;
            int[] indcol = new int[ii+1];
            int[] indrow = new int[ii+1];
            int[] piv = new int[ii + 1];
            int i = 0;
            int j = 0;
            int k = 0;
            int col = 0;
            int row = 0;

            //fill the pivotal as zero
            for (j = 0; j <= ii; j++)
            {
                piv[j] = 0;
            }

            //loop
            for (i = 0; i <= ii; i++)
            {
                //search for povotal 
                big = 0.0;
                for (j = 0; j <= ii; j++)
                {
                    if (piv[j] != 1)
                    {
                        for (k = 0; k <= ii; k++)
                        {
                            if (piv[k] == 0)
                            {
                                if (Math.Abs(X[j, k]) >= big)
                                {
                                    big = Math.Abs(X[j, k]);
                                    row = j;
                                    col = k;
                                }
                            }
                            else if (piv[k] > 1)
                            {
                                // check for singular
                                return null;
                            }
                        }
                    }
                }

                piv[col] += 1;

                if (row != col)
                {
                    for (j = 0; j <= ii; j++)
                    {
                        tmp = X[row, j];
                        X[row, j] = X[col, j];
                        X[col, j] = tmp;
                    }
                }
                indrow[i] = row;
                indcol[i] = col;

                //divide pivot row by the pivot element
                if (X[col, col] == 0)
                    return null;
                // check for singular
                pivinv = 1.0 / X[col, col];
                X[col, col] = 1.0;

                for (j = 0; j <= ii; j++)
                {
                    X[col, j] *= pivinv;
                }

                //reduce the row except for the pivot
                for (j = 0; j <= ii; j++)
                {
                    if (j != col)
                    {
                        tmp = X[j, col];
                        X[j, col] = 0.0;
                        for (k = 0; k <= ii; k++)
                        {
                            X[j, k] -= (X[col, k] * tmp);
                        }
                    }
                }
            }

            //swap value
            for (j = ii; j >= 0; j += -1)
            {
                if (indrow[j] != indcol[j])
                {
                    for (k = 0; k <= ii; k++)
                    {
                        tmp = X[k, indrow[j]];
                        X[k, indrow[j]] = X[k, indcol[j]];
                        X[k, indcol[j]] = tmp;
                    }
                }
            }

            return X;
        }
    }


    public class CurveFit
    {

        #region "arbitary function"
        /// <summary>
        /// Delegate function for the fit
        /// Calculate Y, and dY/dCeof from X, and Coef
        /// </summary>
        /// <param name="X">x value of the function</param>
        /// <param name="Coef">function coefficient array</param>
        /// <param name="Y">resulting y</param>
        /// <param name="dYdCeof">resulting dY/dCeof array</param>
        /// <remarks></remarks>
        public delegate void FitFunction(double X, double[] Coef, ref double Y, ref double[] dYdCeof);

        /// <summary>
        /// This is the so called Levenberg-Marquardt Method for nonlinear curve fitting.
        /// It is a modified version of that from Numeric Recipes. 
        /// The the equation being fitted is passed over using Delegate!        
        /// W2 08/12/2003  Original
        /// W2 11/30/2007  Modified for .NET2005 taking the advantage of delegate
        /// </summary>
        /// <param name="X">Input: data array for x</param>
        /// <param name="Y">Input: data array for y</param>
        /// <param name="Coef">Input/Output: in as initial coefficient gass, out as the final best result</param>
        /// <param name="ChiSq">Output: the final Chi square of the fit</param>
        /// <param name="Tolerance">Input: fit stop paramter, fit stops when the relative difference the Chi square between iteration is smaller than this number </param>
        /// <returns>Total number of iteration performed</returns>
        /// <remarks></remarks>
        /// 
        public static int NonlinearFit(double[] X, double[] Y, ref double[] Coef, ref double ChiSq, double Tolerance, FitFunction TheFunction)
        {

            const int MaxTrial = 2000;

            int M = 0;
            int j = 0;
            double lamda = 0;
            double[,] alpha1 = null;
            double[] beta1 = null;
            double[,] alpha2 = null;
            double[] beta2 = null;
            double ChiSq1 = 0;
            double ChiSq2 = 0;
            double[] dA = null;
            double[] ATry = null;
            int Trial = 0;
            bool bLoop = false;

            //get ready
            M = Coef.Length - 1;
            ATry = new double[M + 1];
            alpha1 = new double[+1, +1];
            //beta1 = new double[];
            alpha2 = new double[+1, +1];
            //beta2 = new double[];

            //do 1st try chi square
            MRQCof(X, Y, ref Coef, ref alpha1, ref beta1, ref ChiSq1, TheFunction);

            bLoop = true;
            Trial = 0;
            lamda = 0.001;
            while (bLoop & Trial < MaxTrial)
            {
                //update counter
                Trial += 1;

                //Marquardt insight
                for (j = 0; j <= M; j++)
                {
                    alpha1[j, j] *= (1 + lamda);
                }

                //get next set of A
                dA = Matrix.DotProduct(Matrix.Inverse(alpha1), beta1);
                for (j = 0; j <= M; j++)
                {
                    ATry[j] = Coef[j] + dA[j];
                }

                //get next chi square
                MRQCof(X, Y, ref ATry, ref alpha2, ref beta2, ref ChiSq2, TheFunction);

                //compare result
                if (ChiSq2 <= ChiSq1)
                {
                    //exit loop if no significant change is seen
                    bLoop = (ChiSq1 - ChiSq2) > Tolerance * ChiSq1;
                    //reduce the parameter
                    lamda = lamda * 0.01;
                    //replace results for next try
                    ChiSq1 = ChiSq2;
                    Coef = ATry;
                    beta1 = beta2;
                    alpha1 = alpha2;
                }
                else
                {
                    //exit loop if lamda is already very small
                    bLoop = (lamda > 1E-50);
                    //increase the parameter
                    lamda = lamda * 10.0;
                }

            }

            ChiSq = ChiSq1;
            return Trial;
        }

        private static void MRQCof(double[] X, double[] Y, ref double[] A, ref double[,] alpha, ref double[] beta, ref double ChiSq, FitFunction TheFunction)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int N = 0;
            int M = 0;

            double[] dYdA = null;
            double YFit = 0;
            double dY = 0;

            N = Information.UBound(X);
            M = Information.UBound(A);

            alpha = new double[M + 1, M + 1];
            beta = new double[M + 1];

            ChiSq = 0.0;
            for (i = 0; i <= N; i++)
            {
                //get Yfit and dY/dA
                TheFunction(X[i], A, ref YFit, ref dYdA);

                //calculate alpha, beta, and Chi Square
                dY = Y[i] - YFit;
                ChiSq += (Math.Pow(dY, 2.0));

                for (j = 0; j <= M; j++)
                {
                    for (k = 0; k <= j; k++)
                    {
                        alpha[j, k] += (dYdA[j] * dYdA[k]);
                    }
                    beta[j] += (dYdA[j] * dY);
                }
            }

            //alpha is a symmetric matrix, fill the other half
            for (j = 1; j <= M; j++)
            {
                for (k = 1; k <= j - 1; k++)
                {
                    alpha[k, j] = alpha[j, k];
                }
            }

        }

        #endregion

        #region "Linear fit"

        
        public static void LinearFit(double[] X, double[] Y, ref double[] Coef, ref double RSquared)
        {
            double[] tempArray = new double[X.Length];
            double tempVal = 0;
            LinearFit(X, Y, null, ref Coef, ref tempArray, ref tempArray, ref tempVal, ref tempVal, ref RSquared);
        }

        public static void LinearFit(double[] X, double[] Y, ref double[] Coef, double[] YFit, ref double RSquared)
        {
            double[] tempArray = new double[X.Length];
            double tempVal = 0;
            LinearFit(X, Y, null, ref Coef, ref tempArray, ref YFit, ref tempVal, ref tempVal, ref RSquared);
        }

        public static void LinearFit(double[] X, double[] Y, ref double[] Coef, ref double MSE, ref double RSquared)
        {
            double[] tempArray = new double[X.Length];
            double tempVal = 0;
            LinearFit(X, Y, null, ref Coef, ref tempArray, ref tempArray, ref MSE, ref tempVal, ref RSquared);
        }

        public static void LinearFit(double[] X, double[] Y, ref double[] Coef, double[] YFit, ref double MSE, ref double RSquared)
        {
            double[] tempArray = new double[X.Length];
            double tempVal = 0;
            LinearFit(X, Y, null, ref Coef, ref tempArray, ref YFit, ref MSE, ref tempVal, ref RSquared);
        }

        public static void LinearFit(double[] X, double[] Y, double[] Ystdev, ref double[] Coef, ref double[] CoefStdev, ref double[] Yfit, ref double mse, ref double ChiSquare, ref double RSquared)
        {
            double ss = 0;
            double sx = 0;
            double sy = 0;
            double w = 0;
            double t = 0;
            double sxoss = 0;
            double st2 = 0;
            double a = 0;
            double b = 0;
            int i = 0;
            int ii = 0;
            int n = 0;
            bool NoStdev = false;

            n = X.Length;
            ii = n - 1;

            NoStdev = (Ystdev == null);
            if (NoStdev)
            {
                Ystdev = new double[ii + 1];
                for (i = 0; i <= ii; i++)
                {
                    Ystdev[i] = 1.0;
                }
            }

            ss = 0.0;
            sx = 0.0;
            sy = 0.0;
            st2 = 0.0;
            b = 0.0;
            for (i = 0; i <= ii; i++)
            {
                w = Ystdev[i] * Ystdev[i];
                w = 1.0 / w;
                ss += w;
                sx += X[i] * w;
                sy += Y[i] * w;
            }

            sxoss = sx / ss;
            for (i = 0; i <= ii; i++)
            {
                t = (X[i] - sxoss) / Ystdev[i];
                st2 += t * t;
                b += t * Y[i] / Ystdev[i];
            }

            b /= st2;
            a = (sy - sx * b) / ss;

            //get fitted y
            Yfit = new double[ii + 1];
            for (i = 0; i <= ii; i++)
            {
                Yfit[i] = a + b * X[i];
            }

            //get chi square
            ChiSquare = 0.0;
            mse = 0.0;
            RSquared = 0.0;
            for (i = 0; i <= ii; i++)
            {
                w = Y[i] - Yfit[i];
                mse += w * w;

                w /= Ystdev[i];
                ChiSquare += w * w;

                w = Ystdev[i] * Ystdev[i];
                w = Y[i] / w - sy / n;
                RSquared += w * w;
            }
            RSquared = 1.0 - ChiSquare * n / RSquared;
            mse = mse / n;

            //retrun coef
            Coef = new double[] {
            a,
            b
        };

            //return error for ceof
            a = sx * sx / ss / st2;
            a = Math.Sqrt((1 + a) / ss);
            b = Math.Sqrt(1.0 / st2);

            if (NoStdev)
            {
                w = Math.Sqrt(ChiSquare / (n - 1));
                a = a * w;
                b = b * w;
            }

            CoefStdev = new double[] {
            a,
            b
        };
        }
        #endregion

        #region "Polynomial fit"
        public static void PolynomialFit(double[] X, double[] Y, int Order, ref double[] Coef)
        {
            int i = 0;
            int ii = 0;
            int j = 0;
            int k = 0;
            double C = 0;
            double P = 0;
            double G = 0;
            double Q = 0;
            double D1 = 0;
            double D2 = 0;
            double[] B = null;
            double[] T = null;
            double[] S = null;

            Coef = new double[Order + 1];
            B = new double[Order + 1];
            T = new double[Order + 1];
            S = new double[Order + 1];

            ii = X.Length - 1;

            //------------------------Q1(x) = 1 -----------
            B[0] = 1.0;
            D1 = X.Length;

            //average Y and X
            C = 0.0;
            P = 0.0;
            for (i = 0; i <= ii; i++)
            {
                C += Y[i];
                P += X[i];
            }
            C /= D1;
            P /= D1;

            Coef[0] = C * B[0];

            //------------------------Q2(x) = (x - P)
            T[0] = -P;
            T[1] = 1.0;

            D2 = 0.0;
            C = 0.0;
            G = 0.0;
            for (i = 0; i <= ii; i++)
            {
                Q = X[i] - P;
                D2 += Q * Q;
                C += Y[i] * Q;
                G += X[i] * Q * Q;
            }
            C /= D2;
            P = G / D2;
            Q = D2 / D1;

            D1 = D2;
            Coef[0] += C * T[0];
            Coef[1] = C * T[1];

            //------------------------the rest of Q(x)
            for (j = 2; j <= Order; j++)
            {
                S[0] = -P * T[0] - Q * B[0];

                S[j - 1] = -P * T[j - 1] + T[j - 2];
                if (j > 2)
                {
                    for (k = j - 1; k >= 1; k += -1)
                    {
                        S[k] = -P * T[k] + T[k - 1] - Q * B[k];
                    }
                }

                S[j] = T[j - 1];

                D2 = 0.0;
                C = 0.0;
                P = 0.0;
                for (i = 0; i <= ii; i++)
                {
                    Q = S[j];
                    for (k = j - 1; k >= 0; k += -1)
                    {
                        Q = Q * X[i] + S[k];
                    }

                    D2 += Q * Q;
                    C += Y[i] * Q;
                    P += X[i] * Q * Q;
                }

                C /= D2;
                P /= D2;
                Q = D2 / D1;
                D1 = D2;

                Coef[j] = C * S[j];
                T[j] = S[j];
                for (k = j - 1; k >= 0; k += -1)
                {
                    Coef[k] = C * S[k] + Coef[k];
                    B[k] = T[k];
                    T[k] = S[k];
                }
            }
        }

        public static void PolynomialFit(double[] X, double[] Y, int Order, ref double[] Coef, ref double[] Yfit, ref double mse, ref double RSquared)
        {
            int i = 0;
            int ii = 0;
            int k = 0;
            double yAve = 0;

            //do fit
            PolynomialFit(X, Y, Order, ref Coef);

            //dimension
            ii = Y.Length - 1;
            Yfit = new double[ii + 1];

            //average
            yAve = 0.0;
            for (i = 0; i <= ii; i++)
            {
                yAve += Y[i];
            }
            yAve /= Y.Length;

            //get mes, and R squared
            mse = 0.0;
            RSquared = 0.0;
            for (i = 0; i <= ii; i++)
            {
                //calculate YFit backwards, this is fater
                Yfit[i] = Coef[Order];
                for (k = Order - 1; k >= 0; k += -1)
                {
                    Yfit[i] = Yfit[i] * X[i] + Coef[k];
                }
                //sum square error
                mse += Math.Pow((Y[i] - Yfit[i]), 2);
                //sum variance
                RSquared = Math.Pow((Y[i] - yAve), 2);
            }

            RSquared = 1.0 - mse / RSquared;
            mse /= (Y.Length);
        }
        #endregion
    }

    public class CheckSum
    {
        private byte[] CRC8_Code_Table = new byte[] {
        0x0,         0x7,         0xe,         0x9,         0x1c,        0x1b,        0x12,        0x15,        0x38,        0x3f,
        0x36,        0x31,        0x24,        0x23,        0x2a,        0x2d,        0x70,        0x77,        0x7e,        0x79,
        0x6c,        0x6b,        0x62,        0x65,        0x48,        0x4f,        0x46,        0x41,        0x54,        0x53,
        0x5a,        0x5d,        0xe0,        0xe7,        0xee,        0xe9,        0xfc,        0xfb,        0xf2,        0xf5,
        0xd8,        0xdf,        0xd6,        0xd1,        0xc4,        0xc3,        0xca,        0xcd,        0x90,        0x97,
        0x9e,        0x99,        0x8c,        0x8b,        0x82,        0x85,        0xa8,        0xaf,        0xa6,        0xa1,
        0xb4,        0xb3,        0xba,        0xbd,        0xc7,        0xc0,        0xc9,        0xce,        0xdb,        0xdc,
        0xd5,        0xd2,        0xff,        0xf8,        0xf1,        0xf6,        0xe3,        0xe4,        0xed,        0xea,
        0xb7,        0xb0,        0xb9,        0xbe,        0xab,        0xac,        0xa5,        0xa2,        0x8f,        0x88,
        0x81,        0x86,        0x93,        0x94,        0x9d,        0x9a,        0x27,        0x20,        0x29,        0x2e,
        0x3b,        0x3c,        0x35,        0x32,        0x1f,        0x18,        0x11,        0x16,        0x3,         0x4,
        0xd,         0xa,         0x57,        0x50,        0x59,        0x5e,        0x4b,        0x4c,        0x45,        0x42,
        0x6f,        0x68,        0x61,        0x66,        0x73,        0x74,        0x7d,        0x7a,        0x89,        0x8e,
        0x87,        0x80,        0x95,        0x92,        0x9b,        0x9c,        0xb1,        0xb6,        0xbf,        0xb8,
        0xad,        0xaa,        0xa3,        0xa4,        0xf9,        0xfe,        0xf7,        0xf0,        0xe5,        0xe2,
        0xeb,        0xec,        0xc1,        0xc6,        0xcf,        0xc8,        0xdd,        0xda,        0xd3,        0xd4,
        0x69,        0x6e,        0x67,        0x60,        0x75,        0x72,        0x7b,        0x7c,        0x51,        0x56,
        0x5f,        0x58,        0x4d,        0x4a,        0x43,        0x44,        0x19,        0x1e,        0x17,        0x10,
        0x5,         0x2,         0xb,         0xc,         0x21,        0x26,        0x2f,        0x28,        0x3d,        0x3a,
        0x33,        0x34,        0x4e,        0x49,        0x40,        0x47,        0x52,        0x55,        0x5c,        0x5b,
        0x76,        0x71,        0x78,        0x7f,        0x6a,        0x6d,        0x64,        0x63,        0x3e,        0x39,
        0x30,        0x37,        0x22,        0x25,        0x2c,        0x2b,        0x6,         0x1,         0x8,         0xf,
        0x1a,        0x1d,        0x14,        0x13,        0xae,        0xa9,        0xa0,        0xa7,        0xb2,        0xb5,
        0xbc,        0xbb,        0x96,        0x91,        0x98,        0x9f,        0x8a,        0x8d,        0x84,        0x83,
        0xde,        0xd9,        0xd0,        0xd7,        0xc2,        0xc5,        0xcc,        0xcb,        0xe6,        0xe1,
        0xe8,        0xef,        0xfa,        0xfd,        0xf4,        0xf3    };

        public static string CRC8(string Data)
        {
            System.Text.ASCIIEncoding e = new System.Text.ASCIIEncoding();
            byte[] b = null;

            //convert string to byte
            b = e.GetBytes(Data);

            //get CRC byte
            b[0] = CRC8(b);

            //get char
            
            return Convert.ToString(BitConverter.ToChar(b, 0));
        }

        public static byte CRC8(byte[] Data)
        {
            int i = 0;
            int ii = 0;
            int k = 0;
            int crc = 0;
            int temp = 0;

            ii = Data.Length - 1;

            crc = 0x0;
            for (i = 0; i <= ii; i++)
            {
                temp = Data[i];
                for (k = 0; k <= 7; k++)
                {
                    if (((crc ^ temp) & 0x1) == 0x1)
                    {
                        crc = crc ^ 0x18;
                        crc >>= 1;
                        crc = crc | 0x80;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                    temp >>= 1;
                }
            }

            crc = crc & 0xff;
            //take the LSB
            return Convert.ToByte(crc);
        }

        public static ushort CRC16(byte[] Data)
        {
            int i = 0;
            int ii = 0;
            int k = 0;
            int crc = 0;
            int temp = 0;

            ii = Data.Length - 1;

            crc = 0xffff;
            for (i = 0; i <= ii; i++)
            {
                crc = crc ^ Data[i];
                for (k = 0; k <= 7; k++)
                {
                    temp = crc >> 1;
                    if ((crc & 0x1) == 0x1)
                    {
                        crc = temp ^ 0xa001;
                    }
                    else
                    {
                        crc = temp;
                    }
                }
            }

            crc = crc & 0xffff;
            //take two low byte

            return Convert.ToUInt16(crc);
        }

        public byte CRC8Ex(byte[] Data, int Length)
        {
            byte crc = 0;
            int i = 0;
            int ii = 0;

            ii = Length - 1;
            for (i = 0; i <= ii; i++)
            {
                crc = (byte)(crc ^ Data[i]);
                crc = CRC8_Code_Table[crc];
            }

            return crc;
        }
    }


    public class RotatingBuffer
    {
        private double[] mData;
        private int mLength;

        private int mIndex;
        public RotatingBuffer(int Length)
        {
            mLength = Length;
            this.Clear();
        }

        public void Add(double Value)
        {
            //full?
            if (mIndex == mLength)
                mIndex = 0;
            //add data
            mData[mIndex] = Value;
            //next
            mIndex += 1;
        }

        public void Clear()
        {
            mData = new double[mLength];
            mIndex = 0;
        }

        public int Length
        {
            get { return mLength; }
        }

        public double[] Data
        {
            get { return mData; }
        }

        //public double Data
        //{
        //    get { return mData[Index]; }
        //}

        public double Average
        {
            get { return w2Array.Mean(mData); }
        }

        public double Range
        {
            get { return w2Array.Range(mData); }
        }

        public double Min
        {
            get { return w2Array.Minimum(mData); }
        }

        public double Max
        {
            get { return w2Array.Maximum(mData); }
        }

        public double StandardDeviation
        {
            get { return w2Array.StandardDeviation(mData); }
        }
    }
}
