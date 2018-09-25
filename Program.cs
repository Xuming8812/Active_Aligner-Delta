using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;

namespace Delta
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            const string Title = "Delta";
            string s, sFile;

            //read config

            sFile = Title + ".ini";
            sFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sFile);

            if (!File.Exists(sFile))
            {
                s = "Cannot found the application configuration file "  + sFile;
                s += "Application will be terminated.";
                MessageBox.Show(s);
            }

            MainForm f = new MainForm();

            // Run fMain()

            if (f.Initialize(sFile))
            {
                Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(f);
            }
            else
            {
                s = "Initialization failed! " ;
                s += "Application will be terminated.";
                MessageBox.Show(s);
            }

            

        }
    }
}
