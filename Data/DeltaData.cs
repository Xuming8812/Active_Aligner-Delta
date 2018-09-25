using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Delta
{
    public class DeltaData
    {
        public enum DataFileEnum
        {
            Log,
            Result
        }

        #region "internal constant and utility"
        private string Text = "Delta Data";
        private const string SectionFile = "Files";

        private const string SectionDatabase = "Database";

        private w2.w2IniFile mIniFile;

        public w2.w2IniFile IniFile
        {
            get { return mIniFile; }
        }
        #endregion

        public bool Initialize(string sIniFile)
        {
            bool success = false;

            if (string.IsNullOrEmpty(Path.GetDirectoryName(sIniFile)))
            {
                sIniFile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, sIniFile);
            }
            if (!File.Exists(sIniFile))
            {
                sIniFile = "Cannot find the configuration file " + ControlChars.CrLf + sIniFile;
                MessageBox.Show(sIniFile, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            mIniFile = new w2.w2IniFile(sIniFile);

            success = this.ConnectToTestDatabase();

            return success;
        }

        #region "Error log"
        public void ExtractErrorToSingleFile(string SN, System.DateTime Time)
        {
            string s = null;
            string sErrorLog = null;
            string sDataLog = null;
            StreamReader r = default(StreamReader);
            StreamWriter w = default(StreamWriter);

            //get the last log file
            s = this.GetDataFile(SN, DataFileEnum.Log, false, Time);
            //only file with suffix is static, we will try to get the last one
            sDataLog = this.GetBackupFiles(s)[1];
            r = new StreamReader(sDataLog);

            //we will append data only
            sErrorLog = mIniFile.ReadParameter("Files", "ErrorSummary", "C:\\Data\\ErrorLog.txt");
            w = new StreamWriter(sErrorLog, true);

            s = "";
            while (r.Peek() > 0)
            {
                s = r.ReadLine().Trim();
                if (s.StartsWith("X   ") | s.StartsWith("!   "))
                {
                    w.WriteLine(w2String.Concatenate(Constants.vbTab, System.DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), SN, sDataLog, s));
                }
            }

            //close files
            r.Close();
            w.Close();
        }
        #endregion

        #region "data path and file"
        public string BackupDataFile(string sFile)
        {
            string s = null;
            int i = 0;
            string sExt = Path.GetExtension(sFile);
            string sRoot = Path.GetFileNameWithoutExtension(sFile);

            sRoot = Path.Combine(Path.GetDirectoryName(sFile), sRoot);

            s = sFile;
            while (File.Exists(s))
            {
                s = sRoot + "-" + i.ToString() + sExt;
                i += 1;
            }

            File.Copy(sFile, s, false);

            return s;
        }

        public string[] GetBackupFiles(string sFile)
        {
            string[] sFiles = null;
            int[] index = null;
            int i = 0;
            int ii = 0;
            int k = 0;
            int j = 0;
            string s = null;
            string sPath = null;

            s = sFile;
            sFile = Path.GetFileName(s);
            sPath = Path.GetFullPath(s);
            sPath = sPath.Replace(sFile, "");
            sFile = sFile.Replace(".txt", "*.txt");

            sFiles = Directory.GetFiles(sPath, sFile);

            //sort files by index descending
            ii = sFiles.Length - 1;
            index = new int[ii + 1];
            for (i = 0; i <= ii; i++)
            {
                k = sFiles[i].LastIndexOf("-");
                j = sFiles[i].LastIndexOf(".txt");
                s = sFiles[i].Substring(k + 1, (j - k - 1));
                //index[i] = Convert.ToInt32(s)
                if (!int.TryParse(s, out index[i]))
                {
                    index[i] = 999;
                }
            }

            Array.Sort(index, sFiles);
            Array.Reverse(sFiles);

            return sFiles;
        }

        public string RootDataPath
        {
            get { return mIniFile.ReadParameter(SectionFile, "RootPath", "C:\\Data\\"); }
        }

        public string ClassifyLogPath
        {
            get { return "D:\\Delta Data\\Log\\"; }
        }

        public string ClassifyCsvResultPath
        {
            get { return "D:\\Delta Data\\CsvResult\\"; }
        }

        public string ClassifyErrorPath
        {
            get { return "D:\\Delta Data\\Error Log\\"; }
        }

        public string GetDataPath(string SerialNumber, bool CreatePath, System.DateTime Time)
        {
            string sPath = null;
            string sLetter = null;
            string s = null;
            int i = 0;
            int Number = 0;

            //root path
            sPath = this.RootDataPath;
            if (!Directory.Exists(sPath))
            {
                if (CreatePath)
                {
                    Directory.CreateDirectory(sPath);
                }
                else
                {
                    return "";
                }
            }

            //parse serial number
            SerialNumber = SerialNumber.ToUpper();

            //If IsNumeric(SerialNumber) Then
            //    sPath = Path.Combine(sPath, SerialNumber)
            //    Return sPath
            //End If

            if (Information.IsNumeric(SerialNumber))
            {
                sLetter = "";
                s = SerialNumber;
            }
            else
            {
                sLetter = w2String.ExtractLeadingAlphaCharacters(SerialNumber);
                s = SerialNumber.Replace(sLetter, "");
            }

            //get path for the numeric part of the serial number
            try
            {
                Number = int.Parse(s);

                i = 1000 * (Number / 1000);
                s = sLetter + i + "-" + sLetter + (i + 999);
                sPath = Path.Combine(sPath, s);
                if (!Directory.Exists(sPath))
                {
                    if (CreatePath)
                        Directory.CreateDirectory(sPath);
                }

                i = 100 * (Number / 100);
                s = sLetter + i + "-" + sLetter + (i + 99);
                sPath = Path.Combine(sPath, s);

                if (!Directory.Exists(sPath))
                {
                    if (CreatePath)
                        Directory.CreateDirectory(sPath);
                }

                sPath = Path.Combine(sPath, SerialNumber);
                sPath += "_" + Time.ToString("yyyyMMddHH");
                if (!Directory.Exists(sPath))
                {
                    if (CreatePath)
                        Directory.CreateDirectory(sPath);
                }

            }
            catch (Exception ex)
            {
                if (!CreatePath)
                {
                    s = ex.ToString();
                    s = "Failed to parse the serial number into file path structures " + SerialNumber;
                    s = s + ControlChars.CrLf + "Numeric number expected for the serial number after the letters.";
                    MessageBox.Show(s, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return "";
                }
            }

            //return
            return sPath;
        }

        public string GetDataFile(string SerialNumber, DataFileEnum FileType, System.DateTime Time)
        {
            return GetDataFile(SerialNumber, FileType, false, Time);
        }

        public string GetDataFile(string SerialNumber, DataFileEnum FileType, bool CreateFile, System.DateTime Time)
        {
            string s = null;
            string sPath = null;
            string sFileName = null;
            string sFile = null;

            //get data path
            sPath = this.GetDataPath(SerialNumber, CreateFile, Time);
            if (string.IsNullOrEmpty(sPath))
                return "";

            //add space to the file name
            sFileName = Enum.GetName(typeof(DataFileEnum), FileType);
            sFileName = w2String.AddSpaceBetweenWords(sFileName);
            sFileName += ".txt";

            //add serial number to the file name
            sFileName = SerialNumber + " " + sFileName;

            //build full file name
            sFile = Path.Combine(sPath, sFileName);

            //validate file
            if (!File.Exists(sFile))
            {
                if (!CreateFile)
                {
                    s = "Cannot find the default data file " + sFile;
                    MessageBox.Show(s, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    sFile = "";
                }
            }

            //return 
            return sFile;
        }

        public string GetClassifyLogFile(string SerialNumber, DateTime Time)
        {
            string sPath = null;
            string sFileName = null;
            string sFile = null;

            sPath = this.ClassifyLogPath;
            sFileName = "LOG.txt";
            sFileName = SerialNumber + " " + Time.ToString("yyyyMMddHHmm") + sFileName;

            sFile = Path.Combine(sPath, sFileName);

            return sFile;
        }

        public string InsertTextToFileName(string FileName, string Text)
        {
            string s = null;

            //add space to the inserted text
            if (!Text.StartsWith(" "))
                Text = " " + Text;

            //get file extension
            s = Path.GetExtension(FileName);

            //insert text
            if (string.IsNullOrEmpty(s))
            {
                FileName += Text;
            }
            else
            {
                Text += s;
                FileName = FileName.Replace(s, Text);
            }

            return FileName;
        }
        #endregion

        #region "database"
        public enum dbCommandEnum
        {
            ProcessData
        }


        private SqlConnection mDataConnection;
        public SqlDataAdapter GetDataAdapter(string SerialNumber, dbCommandEnum DataType, bool RaiseError)
        {
            string s = null;
            string sQuery = null;
            SqlConnection dbConnection = default(SqlConnection);

            sQuery = GetQueryCommand(DataType, SerialNumber);
            if (string.IsNullOrEmpty(sQuery))
                return null;

            //which database to connect?
            if (!ConnectToTestDatabase())
                return null;
            dbConnection = mDataConnection;

            //open
            try
            {
                //open connection, get adpater
                SqlCommand DataCommand = new SqlCommand(sQuery, dbConnection);
                SqlDataAdapter DataAdapter = new SqlDataAdapter(DataCommand);
                //build insert command
                SqlCommandBuilder CmdBuilder = new SqlCommandBuilder(DataAdapter);
                DataAdapter.InsertCommand = CmdBuilder.GetInsertCommand();
                //get data table
                return DataAdapter;
            }
            catch (Exception ex)
            {
                if (RaiseError)
                {
                    s = "Database error " + Constants.vbCrLf + ex.Message;
                    MessageBox.Show(s);
                }
                return null;
            }
        }


        private bool ConnectToTestDatabase()
        {
            //fine if it is already open
            if (mDataConnection != null)
                return true;

            //get connection string
            string s = "";
            s = mIniFile.ReadParameter(SectionDatabase, "Connection", "");
            if (string.IsNullOrEmpty(s))
            {
                //s = "Cannot found the connection string in the configuration file for database."
                //MessageBox.Show(s, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                return true;
            }

            //OK, try a new connection
            mDataConnection = new SqlConnection(s);
            try
            {
                mDataConnection.Open();
                return true;

            }
            catch (Exception ex)
            {
                s = "Database error " + ControlChars.CrLf + ex.Message;
                MessageBox.Show(s);
                mDataConnection = null;
                return false;
            }
        }

        private string GetQueryCommand(dbCommandEnum CommandType, params string[] sParameter)
        {
            string s = null;
            string sKey = null;

            //get key
            sKey = "Cmd" + Enum.GetName(typeof(dbCommandEnum), CommandType);

            //get SQL
            s = mIniFile.ReadParameter(SectionDatabase, sKey, "");
            if (string.IsNullOrEmpty(s))
                return "";

            if (sParameter.Length == 0)
            {
                return s;
            }
            else
            {
                return GetQueryString(s, sParameter);
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

        //Private Function GetQueryString(ByVal sTemplate As String, ByVal ParamArray sValue() As String) As String
        //    Dim i As Integer
        //    Dim r As Regex
        //    Dim m As Match

        //    'add quotations to avoid reserved characters used in product name, such as "-" and etc
        //    For i = 0 To sValue.Length - 1
        //        If (Not sValue[i].StartsWith("'")) Then sValue[i] = "'" & sValue[i] & "'"
        //    Next

        //    r = New Regex("(?:\[[^[]+\])")
        //    m = r.Match(sTemplate)
        //    i = 0
        //    While m.Success And (i < sValue.Length)
        //        sTemplate = sTemplate.Replace(m.Value, sValue[i])
        //        i += 1
        //    End While

        //    Return sTemplate
        //End Function

        #endregion
    }
}
