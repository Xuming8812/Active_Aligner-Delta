using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace Delta
{
    public class w2String
    {

        #region "Concatenate"
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
        #endregion

        #region "regular expression"
        public class RegexPatterns
        {
            public const string DecimalNumber = "[+-]?(?:\\d+\\.?\\d*|\\d*\\.?\\d+)";
            public const string SpaceDelimitedValues = "(?:(?(\")\".+\"|\\S+))";
            public const string UnsignedIntegerNumber = "(?:\\d+)";
            public const string EndingDigits = "(?:\\d+)$";
        }

        public static string[] ExtractMatches(string s, string Pattern)
        {
            List<string> L = new List<string>();
            Regex r = new Regex(Pattern);
            Match m = default(Match);

            m = r.Match(s);
            while (m.Success)
            {
                L.Add(m.Value);
                m = m.NextMatch();
            }

            return L.ToArray();
        }

        public static string[] ExtractDecimalNumbers(string s)
        {
            return ExtractMatches(s, RegexPatterns.DecimalNumber);
        }

        public static string ExtractDecimalNumberFirst(string s)
        {
            string[] x = ExtractDecimalNumbers(s);
            if (x.Length == 0)
            {
                return "";
            }
            else
            {
                return x[0];
            }
        }

        public static string ExtractDecimalNumberLast(string s)
        {
            string[] x = ExtractDecimalNumbers(s);
            if (x.Length == 0)
            {
                return "";
            }
            else
            {
                return x[x.Length - 1];
            }
        }

        public static double[] ExtractDecimalNumbersAsDouble(string s)
        {
            List<double> L = new List<double>();
            double v = 0;
            Regex r = new Regex(RegexPatterns.DecimalNumber);
            Match m = default(Match);

            m = r.Match(s);
            while (m.Success)
            {
                if (double.TryParse(m.Value, out v))
                    L.Add(v);
                m = m.NextMatch();
            }

            return L.ToArray();
        }

        public static string[] ExtractIntegerNumbers(string s)
        {
            return ExtractMatches(s, RegexPatterns.UnsignedIntegerNumber);
        }

        public static string ExtractIntegerNumberFirst(string s)
        {
            string[] x = ExtractIntegerNumbers(s);
            if (x.Length == 0)
            {
                return "";
            }
            else
            {
                return x[0];
            }
        }

        public static string ExtractIntegerNumberLast(string s)
        {
            string[] x = ExtractIntegerNumbers(s);
            if (x.Length == 0)
            {
                return "";
            }
            else
            {
                return x[x.Length - 1];
            }
        }

        public static int[] ExtractIntegerNumbersAsDouble(string s)
        {
            List<int> L = new List<int>();
            int v = 0;
            Regex r = new Regex(RegexPatterns.UnsignedIntegerNumber);
            Match m = default(Match);

            m = r.Match(s);
            while (m.Success)
            {
                if (int.TryParse(m.Value, out v))
                    L.Add(v);
                m = m.NextMatch();
            }

            return L.ToArray();
        }

        public static bool GetCommandValuePair(string s, ref string Command, ref double Value)
        {
            List<string> L = new List<string>();
            Regex r = default(Regex);
            Match m = default(Match);

            //try both cmd and value
            r = new Regex("^(?<cmd>[a-zA-Z]+)\\s+(?<value>[+-]?(?:\\d+\\.?\\d*|\\d*\\.?\\d+))");
            m = r.Match(s);
            if (m.Success)
            {

                Command = m.Groups["cmd"].Value;
                Value = double.Parse(m.Groups["value"].Value);
                return true;
            }

            //cmd only
            r = new Regex("^(?<cmd>[a-zA-Z]+)\\s*");
            m = r.Match(s);
            if (m.Success)
            {
                Command = m.Groups["cmd"].Value;
                Value = double.NaN;
                return true;
            }

            //false
            return false;

        }

        public static string[] SplitSpaceDelimitedValues(string s)
        {
            return ExtractMatches(s, RegexPatterns.SpaceDelimitedValues);
        }

        public static string ExtractLeadingAlphaCharacters(string s)
        {
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("^(?:[a-zA-Z]+)");
            System.Text.RegularExpressions.Match m = default(System.Text.RegularExpressions.Match);
            m = r.Match(s);
            if (m.Success)
            {
                return m.Value;
            }
            else
            {
                return s;
            }
        }

        public static int ExtractEndingDigits(string s)
        {
            string[] v = ExtractMatches(s, RegexPatterns.EndingDigits);
            if (v.Length > 0)
            {
                return int.Parse(v[0]);
            }
            else
            {
                return -1;
            }
        }

        private string GetQueryString(string sTemplate, params string[] sValue)
        {
            int i = 0;
            Regex r = default(Regex);
            Match m = default(Match);

            //add quotations to avoid reserved characters used in product name, such as "-" and etc
            for (i = 0; i <= sValue.Length - 1; i++)
            {
                if (Information.IsNumeric(sValue[i]))
                    continue;
                if ((!sValue[i].StartsWith("'")))
                    sValue[i] = "'" + sValue[i] + "'";
            }

            r = new Regex("(?:\\@[^@]+\\@)");
            //r = New Regex("\@[^@]+\@")
            m = r.Match(sTemplate);
            i = 0;
            while (m.Success & (i < sValue.Length))
            {
                sTemplate = sTemplate.Replace(m.Value, sValue[i]);
                m = m.NextMatch();
                i += 1;
            }

            return sTemplate;
        }
        #endregion

        public static string AddSpaceBetweenWords(string s)
        {
            int i = 0;
            int ii = 0;
            string C = null;
            string NextC = null;

            ii = s.Length - 2;
            C = s.Substring(i + 1, 1);

            //do this backword because we are keep inserting spaces to the string
            for (i = ii; i >= 1; i += -1)
            {
                NextC = C;
                C = s.Substring(i, 1);
                //from Up to Lo
                if (C.ToUpper() == C && NextC.ToLower() == NextC)
                    s = s.Insert(i, " ");
                //from Lo to Up
                if (C.ToLower() == C && NextC.ToUpper() == NextC)
                    s = s.Insert(i + 1, " ");
            }

            //remove the double sapce
            s = s.Replace("  ", " ");

            //back
            return s;
        }

        #region "split data by comma, space, or tab"
        public static string[] SplitDataAsString(string sData)
        {
            return SplitDataAsString(sData, true);
        }

        private static string[] SplitDataAsString(string sData, bool bRemoveDuplicatedDelimiter)
        {
            char Delimiter = '\0';

            if (sData.Contains(ControlChars.Tab))
            {
                Delimiter = ControlChars.Tab;
            }
            else if (sData.Contains(','))
            {
                Delimiter = ',';
            }
            else
            {
                Delimiter = ' ';
            }

            string[] ss = sData.Split(Delimiter);
            if (bRemoveDuplicatedDelimiter)
            {
                List<string> x = new List<string>();
                foreach (string s in ss)
                {
                    if (!string.IsNullOrEmpty(s))
                        x.Add(s);
                }
                ss = x.ToArray();
            }

            return ss;
        }

        public static double[] SplitDataAsDouble(string sData)
        {
            return SplitDataAsDouble(sData, true);
        }

        private static double[] SplitDataAsDouble(string sData, bool bRemoveDuplicatedDelimiter)
        {
            double[] v = null;
            string[] s = SplitDataAsString(sData, bRemoveDuplicatedDelimiter);
            try
            {
                v = Array.ConvertAll(s, new System.Converter<string, double>(Convert.ToDouble));
            }
            catch (Exception ex)
            {
                v = new double[s.Length];
                for (int i = 0; i <= s.Length - 1; i++)
                {
                    v[i] = Conversion.Val(s[i]);
                }
            }

            return v;
        }
        #endregion
    }
}
