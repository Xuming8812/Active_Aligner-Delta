using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Delta
{
    public class w2CSVHelper
    {
        private StreamWriter sw;

        public w2CSVHelper(string sFileName, DataGridViewColumnCollection columns)
        {
            try
            {
                sw = new StreamWriter(new FileStream(sFileName, FileMode.Create), Encoding.GetEncoding("GB2312"));
                StringBuilder strColu = new StringBuilder();

                for (int i = 0; i <= columns.Count - 1; i++)
                {
                    strColu.Append(columns[i].HeaderText);
                    strColu.Append(",");
                }
                strColu.Remove(strColu.Length - 1, 1);
                sw.WriteLine(strColu);
                sw.Flush();

            }
            catch (Exception ex)
            {
               MessageBox.Show("error", ex.ToString());
            }

        }
        public w2CSVHelper(string sFileName, string[] columns)
        {
            if (File.Exists(sFileName))
            {
                sw = new StreamWriter(new FileStream(sFileName, FileMode.Append), Encoding.GetEncoding("GB2312"));
            }
            else
            {
                try
                {
                    sw = new StreamWriter(new FileStream(sFileName, FileMode.Create), Encoding.GetEncoding("GB2312"));
                    StringBuilder strColu = new StringBuilder();

                    for (int i = 0; i <= columns.Length - 1; i++)
                    {
                        strColu.Append(columns[i]);
                        strColu.Append(",");
                    }
                    strColu.Remove(strColu.Length - 1, 1);
                    sw.WriteLine(strColu);
                    sw.Flush();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("error", ex.ToString());
                }
            }
        }

        public void AppendLine(DateTime time, double[] values)
        {
            StringBuilder strValue = new StringBuilder();

            strValue.Append(time);
            strValue.Append(",");
            for (int i = 0; i <= values.Length - 1; i++)
            {
                strValue.Append(values[i]);
                strValue.Append(",");
            }
            strValue.Remove(strValue.Length - 1, 1);
            sw.WriteLine(strValue);
            sw.Flush();
        }

        public void AppendLine(params object[] values)
        {
            StringBuilder strValue = new StringBuilder();

            for (int i = 0; i <= values.Length - 1; i++)
            {
                strValue.Append(values[i]);
                strValue.Append(",");
            }
            strValue.Remove(strValue.Length - 1, 1);
            sw.WriteLine(strValue);
            sw.Flush();
        }

        public void Close()
        {
            sw.Close();
        }
    }
}
