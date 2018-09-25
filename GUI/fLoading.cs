using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Delta
{
    public partial class fLoading : Form
    {

        private Thread updateProgressThread = null;
        private Thread updateStatusThread = null;
        private string StatusText = "";
        private int Progress = 0;

        public fLoading()
        {
            InitializeComponent();
            updateProgressThread = new Thread(new ThreadStart(UpdateProgressThreadMethod));
            updateStatusThread = new Thread(new ThreadStart(UpdateStatusTestThreadMethod));
            updateProgressThread.Start();
            updateStatusThread.Start();
        }

        public void SetStatusText(string text)
        {
            this.StatusText = text;
        }

        public void SetProgress(int progress)
        {
            this.Progress = progress;
        }

        public void UpdateText(string appendString)
        {
            if (InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
               {
                   lblStatus.Text = StatusText + appendString;
               });
            }
            else
            {
                lblStatus.Text = StatusText + appendString;
            }
        }

        public void UpdateProgress()
        {
            if (InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    psProgress.Value = Progress;
                });
            }
            else
            {
                psProgress.Value = Progress;
            }
        }

        public void UpdateProgressThreadMethod()
        {
            int count = 0;
            while (true)
            {
                switch (count%4)
                {
                    case 0:
                        UpdateText("");
                        break;
                    case 1:
                        UpdateText(".");
                        break;
                    case 2:
                        UpdateText("..");
                        break;
                    case 3:
                        UpdateText("...");
                        break;
                    default:
                        break;
                }
                count++;
                Thread.Sleep(100);
            }         
        }

        public void UpdateStatusTestThreadMethod()
        {
            while (true)
            {
                UpdateProgress();
                Thread.Sleep(100);
            }
        }

        public void Dispose()
        {
            if (updateProgressThread != null && updateProgressThread.IsAlive)
            {
                updateProgressThread.Abort();
                updateProgressThread = null;
            }

            if (updateStatusThread != null && updateStatusThread.IsAlive)
            {
                updateStatusThread.Abort();
                updateStatusThread = null;
            }

        }
    }
}
