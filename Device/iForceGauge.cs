using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using Microsoft.VisualBasic;

namespace Instrument
{
    public class iOmegaDP40
    {
        private SerialPort mPort;

        private int mSubAdrs;
        public string Name
        {
            get { return "Omega DP40 COntroller"; }
        }

        public bool Initialize(string sPort, int BaudRate, bool RaiseError)
        {
            double v = 0;

            //set port
            mPort = new SerialPort(sPort, BaudRate, Parity.Odd, 7, StopBits.One);
            mPort.ReadTimeout = 2000;

            //open port
            try
            {
                mPort.Open();
                System.Threading.Thread.Sleep(200);
                v = this.ReadProcessValue();
                return true;
            }
            catch (Exception e)
            {
                if (RaiseError)
                    MessageBox.Show(e.Message, this.Name);
                if (mPort.IsOpen)
                    mPort.Close();
                return false;
            }

        }

        public int SubAddress
        {
            get { return mSubAdrs; }
            set { mSubAdrs = value; }
        }

        public double ReadProcessValue()
        {
            string s = null;
            s = this.QueryString("V01");
      
            return Conversion.Val(s);
        }

        public double ReadProcessValue(int SubAddress)
        {
            this.SubAddress = SubAddress;
            return this.ReadProcessValue();
        }

        private string QueryString(string sCmd)
        {
            string s = null;
            //clear buffer
            mPort.DiscardInBuffer();
            mPort.ReadExisting();
            System.Threading.Thread.Sleep(100);
            //send command
            this.SendCmd(sCmd);
            System.Threading.Thread.Sleep(100);
            //read data
            s = mPort.ReadExisting();
            //remove known characters
            s = s.Replace(sCmd, "");
            s = s.Replace(ControlChars.Cr, Convert.ToChar(""));
            return s;
        }

        private void SendCmd(string sCmd)
        {
            string s = null;

            mPort.DiscardOutBuffer();
            s = "*" + sCmd + ControlChars.Cr;
            mPort.Write(s);

            System.Threading.Thread.Sleep(100);
        }

        //Private Sub SendCmdWithAdrs(ByVal sCmd As String)
        //    Dim s As String

        //    mPort.DiscardOutBuffer()

        //    s = "*" + Convert.ToString(mSubAdrs, 16)
        //    s += sCmd + ControlChars.Cr
        //    mPort.Write(s)

        //    System.Threading.Thread.Sleep(100)
        //End Sub
    }
}
