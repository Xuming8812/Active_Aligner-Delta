using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace Delta
{
    public class w2NumericUpDownHelper
    {
        private ContextMenuStrip mMenu;
        private ToolStripItem mDisplay;

        private ToolTip mToolTip;
        private NumericUpDown mNud;
        private decimal mMin;

        private decimal mMax;
        public w2NumericUpDownHelper(NumericUpDown hNud, string sConfigFile)
        {
            w2.w2IniFile iniFile = default(w2.w2IniFile);
            string s = null;
            string[] section = null;

            //ctrl
            mNud = hNud;
            s = mNud.Name;

            //config
            sConfigFile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, sConfigFile);
            if (File.Exists(sConfigFile))
            {
                iniFile = new w2.w2IniFile(sConfigFile);
                //check if we have condig
                section = iniFile.GetAllSections();
                if (Array.IndexOf(section, s) >= 0)
                {
                    //config
                    var _with1 = mNud;
                    _with1.DecimalPlaces = iniFile.ReadParameter(s, "DecimalPlaces", 1);
                    _with1.Minimum = Convert.ToDecimal(iniFile.ReadParameter(s, "Minimum", 0.0));
                    _with1.Maximum = Convert.ToDecimal(iniFile.ReadParameter(s, "Maximum", 100.0));
                    _with1.Increment = Convert.ToDecimal(iniFile.ReadParameter(s, "Increment", 1.0));
                    _with1.Value = Convert.ToDecimal(iniFile.ReadParameter(s, "Value", 1.0));
                }
                else
                {
                    MessageBox.Show("No configuration entry for " + s, "UI Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Cannot found user interface configuration file " + sConfigFile, "UI Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //the maximum increment will be one tenth of maximum value
            mMax = 0.1m * mNud.Maximum;
            //minimum will be harder as it may be zero or negative
            mMin = Convert.ToDecimal(1.0 / Math.Pow(10, mNud.DecimalPlaces));
            if (mNud.Minimum > 0 & mNud.Minimum < mMin)
            {
                mMin = mNud.Minimum;
            }

            //set up menu
            mMenu = new ContextMenuStrip();
            var _with2 = mMenu.Items;
            mDisplay = _with2.Add("Increment Step = " + mNud.Increment);
            _with2.Add("Increase Increment 10X");
            _with2.Add("Decrease Increment 10X");
            _with2.Add("Set Increment to ...");

            //tool tip
            mToolTip = new ToolTip();
            mToolTip.SetToolTip(mNud.Controls[0], mDisplay.Text);

            //pass down
            mNud.ContextMenuStrip = mMenu;

            //add event handler
            mMenu.ItemClicked += ContextMenu_ItemClicked;

        }

        private void ContextMenu_ItemClicked(System.Object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            decimal v = default(decimal);
            string s = null;

            switch (e.ClickedItem.Text.Substring(0, 2))
            {
                case "In":
                    v = 10 * mNud.Increment;

                    break;
                case "De":
                    v = 0.1m * mNud.Increment;

                    break;
                case "Se":
                    s = Interaction.InputBox("Set the new increment value", "", mNud.Increment.ToString());
                    s = s.Trim();
                    if (string.IsNullOrEmpty(s))
                        return;
                    if (!decimal.TryParse(s, out v))
                        return;
                    break;
            }

            if (v > mMax)
                v = mMax;
            if (v < mMin)
                v = mMin;

            mNud.Increment = v;
            mDisplay.Text = ("Increment Step = " + mNud.Increment);
            mToolTip.SetToolTip(mNud.Controls[0], mDisplay.Text);
        }
    }

    public class w2ConfigTableHelper
    {
        private enum ColIndex
        {
            Text,
            Value,
            Section,
            Key
        }

        private w2.w2IniFileXML mIniFile;

        private w2.w2IniFileXML mGuiFile;
        public w2ConfigTableHelper(string sConfigFile, string sGuiFile)
        {
            mIniFile = new w2.w2IniFileXML(sConfigFile);
            mGuiFile = new w2.w2IniFileXML(sGuiFile);
        }

        public void SetupTable(DataGridView dgv)
        {
            //add event handler for error handling
            dgv.DataError += dgv_DataError;

            //build GUI
            var _with3 = dgv;
            _with3.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;

            _with3.EditMode = DataGridViewEditMode.EditOnEnter;

            _with3.RowCount = 0;
            _with3.RowHeadersVisible = false;
            _with3.ColumnCount = Enum.GetValues(typeof(ColIndex)).Length;
            _with3.Columns[(int)ColIndex.Section].Visible = false;
            _with3.Columns[(int)ColIndex.Key].Visible = false;

            _with3.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _with3.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);

            _with3.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular);

            _with3.AllowUserToAddRows = false;
            _with3.AllowUserToDeleteRows = false;
            _with3.AllowUserToResizeRows = false;
            _with3.ReadOnly = false;

            _with3.Columns[(int)ColIndex.Text].HeaderText = "Function";
            _with3.Columns[(int)ColIndex.Text].ReadOnly = true;

            _with3.Columns[(int)ColIndex.Value].HeaderText = "Value";
            _with3.Columns[(int)ColIndex.Value].ReadOnly = false;
            _with3.Columns[(int)ColIndex.Value].ValueType = typeof(decimal);

            for (int i = 0; i <= _with3.ColumnCount - 1; i++)
            {
                _with3.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            _with3.Columns[(int)ColIndex.Value].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            _with3.AutoResizeColumns();
            _with3.AutoResizeRows();

            _with3.ScrollBars = ScrollBars.Vertical;
            //do not show selection
            _with3.MultiSelect = false;
        }

        public void PopulateTable(DataGridView dgv)
        {
            int r = 0;
            string s = null;
            string msg = null;
            string sText = null;
            string sSection = null;
            string sKey = null;
            string sValue = null;
            string[] Lines = null;
            string[] Data = null;
            Font xFont = default(Font);

            //build header font
            xFont = new Font(dgv.DefaultCellStyle.Font.Name, dgv.DefaultCellStyle.Font.Size, FontStyle.Bold);

            //get GUI instructions
            sSection = dgv.Name;
            s = mGuiFile.ReadParameter(sSection, "Data", "");
            Lines = s.Split(ControlChars.Cr);

            //populate the table
            dgv.Rows.Clear();
            foreach (string s_loopVariable in Lines)
            {
                s = s_loopVariable;
                s = s.Trim();

                if (string.IsNullOrEmpty(s))
                    continue;
                if (s.StartsWith(";"))
                    continue;

                Data = s.Split(ControlChars.Tab);
                sText = Data[0].Trim();

                if (sText.StartsWith("[") & sText.EndsWith("]"))
                {
                    r = dgv.Rows.Add(Data[0]);
                    dgv.Rows[r].DefaultCellStyle.Font = xFont;
                    dgv.Rows[r].DefaultCellStyle.ForeColor = Color.DarkCyan;
                    dgv.Rows[r].ReadOnly = true;

                }
                else if (Data.Length == 3)
                {
                    sSection = Data[1].Trim();
                    sKey = Data[2].Trim();
                    sValue = mIniFile.ReadParameter(sSection, sKey, "").Trim();

                    r = dgv.Rows.Add(sText, sValue, sSection, sKey);
                    if (Information.IsNumeric(sValue))
                    {
                        dgv.Rows[r].Cells[(int)ColIndex.Value].ValueType = typeof(decimal);

                        //dgv.Item(ColIndex.Value, r).ValueType = typeof(decimal);
                    }
                    else
                    {
                        dgv.Rows[r].Cells[(int)ColIndex.Value].ValueType = typeof(string);
                        //dgv.Item(ColIndex.Value, r).ValueType = typeof(string);
                    }
                }
                else
                {
                    msg = "Error parsing GUI instruction file ";
                    msg += ControlChars.CrLf + "File" + ControlChars.CrLf + mGuiFile.FileName;
                    msg += ControlChars.CrLf + "Table" + ControlChars.CrLf + dgv.Name;
                    msg += ControlChars.CrLf + "Line" + ControlChars.CrLf + s;

                    MessageBox.Show(msg, "Config File GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
        }

        public void SaveTable(DataGridView dgv)
        {
            string sValue = null;
            string sSection = null;
            string sKey = null;
            //DataGridViewRow row = default(DataGridViewRow);

            foreach (DataGridViewRow row in dgv.Rows)
            {
                //note that the order is fixed when the table is populated
                sValue = Convert.ToString(row.Cells[(int)ColIndex.Value].Value);
                sSection = Convert.ToString(row.Cells[(int)ColIndex.Section].Value);
                sKey = Convert.ToString(row.Cells[(int)ColIndex.Key].Value);
                //save data
                if (string.IsNullOrEmpty(sSection) | string.IsNullOrEmpty(sKey))
                    continue;
                mIniFile.WriteParameter(sSection, sKey, sValue);
            }
        }


        private void dgv_DataError(object sender, System.Windows.Forms.DataGridViewDataErrorEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex < 0)
                return;
            DataGridViewCell Cell = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex];
            //DataGridViewCell Cell = ((DataGridView)sender).Item(e.ColumnIndex, e.RowIndex);

            if (Cell.ValueType.Equals(typeof(System.DateTime)))
            {
                MessageBox.Show("Please enter a valid date!", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (Cell.ValueType.Equals(typeof(decimal)))
            {
                MessageBox.Show("Please enter an enumeric number!", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(e.Exception.Message, "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    public class w2ComboListItem
    {

        private int mValue;

        private string mText;
        public w2ComboListItem(int Value, string Text)
        {
            mValue = Value;
            mText = Text;
        }

        public int Value
        {
            get { return mValue; }
        }

        public string Text
        {
            get { return mText; }
        }

        public override string ToString()
        {
            return mText;
        }

        public static w2ComboListItem[] BuildList(System.Type EnumType)
        {
            Array Value = default(Array);
            Array Text = default(Array);
            int i = 0;
            int ii = 0;
            w2ComboListItem[] data = null;

            Value = Enum.GetValues(EnumType);
            Text = Enum.GetNames(EnumType);

            ii = Value.Length - 1;
            data = new w2ComboListItem[ii + 1];
            for (i = 0; i <= ii; i++)
            {
                data[i] = new w2ComboListItem((Int32)Value.GetValue(i), Convert.ToString(Text.GetValue(i)));
            }
            return data;
        }
    }

}

