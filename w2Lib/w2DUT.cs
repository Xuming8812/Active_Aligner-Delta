using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace Delta
{
    public class w2DUTEx
    {
        public enum ColIndex
        {
            Selected,
            SN,
            Status,
            Results,
            Log
        }

        public enum ItemAccessStatus
        {
            Running,
            Passed,
            Stopped,
            Failed,
            UnkownSkipped
        }

        public List<w2DutItem> mItems;
        private DataGridView withEventsField_mdgv;
        private DataGridView mdgv
        {
            get { return withEventsField_mdgv; }
            set
            {
                if (withEventsField_mdgv != null)
                {
                    withEventsField_mdgv.CellValueChanged -= mdgv_CellValueChanged;
                    withEventsField_mdgv.CurrentCellDirtyStateChanged -= mdgv_CurrentCellDirtyStateChanged;
                    withEventsField_mdgv.DoubleClick -= mdgv_DoubleClick;
                    withEventsField_mdgv.VisibleChanged -= mdgv_VisibleChanged;
                }
                withEventsField_mdgv = value;
                if (withEventsField_mdgv != null)
                {
                    withEventsField_mdgv.CellValueChanged += mdgv_CellValueChanged;
                    withEventsField_mdgv.CurrentCellDirtyStateChanged += mdgv_CurrentCellDirtyStateChanged;
                    withEventsField_mdgv.DoubleClick += mdgv_DoubleClick;
                    withEventsField_mdgv.VisibleChanged += mdgv_VisibleChanged;
                }
            }
        }

        private bool mInternalChange;
        public w2DUTEx(DataGridView dgv) : base()
        {
            mdgv = dgv;
            mItems = new List<w2DutItem>();

            this.SetupGUI();
        }

        public bool Enabled
        {
            get { return mdgv.Enabled; }
            set
            {
                mdgv.Enabled = value;
                if (value)
                {
                    mdgv.DefaultCellStyle.BackColor = Color.White;
                }
                else
                {
                    mdgv.DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
        }

        public void ClearTable()
        {
            //DataGridViewRow Row = default(DataGridViewRow);
            mInternalChange = true;
            foreach (DataGridViewRow Row in mdgv.Rows)
            {
                Row.DefaultCellStyle.BackColor = mdgv.DefaultCellStyle.BackColor;

                Row.Cells[(int)ColIndex.Status].Value = "";
                Row.Cells[(int)ColIndex.Status].Style.BackColor = mdgv.DefaultCellStyle.BackColor;
                Row.Cells[(int)ColIndex.Results].Value = "";
                Row.Cells[(int)ColIndex.Log].Value = "";
            }
        }

        public bool IsSelectedToRun(int Index)
        {
            return Convert.ToBoolean(mdgv.Rows[Index].Cells[(int)ColIndex.Selected].Value);
        }

        public void HighLightRow(int Index, ItemAccessStatus highlight, string ResultText, string LogFilePath)
        {
            DataGridViewRow Row = mdgv.Rows[Index];

            //do base class
            this.LastAccessedItem = Index;
            this.LastAccessedItemStatus = highlight;

            //do GUI
            switch (highlight)
            {
                case ItemAccessStatus.Running:
                    //If StatusText = "" Then StatusText = "RUNNING"
                    Row.DefaultCellStyle.BackColor = Color.LightSeaGreen;
                    Row.Cells[(int)ColIndex.Status].Value = "RUNNING";
                    Row.Cells[(int)ColIndex.Status].Style.BackColor = Color.Red;

                    break;
                case ItemAccessStatus.Failed:
                    //If StatusText = "" Then StatusText = Date.Now.ToString("HH:mm:ss")
                    Row.DefaultCellStyle.BackColor = mdgv.DefaultCellStyle.BackColor;
                    Row.Cells[(int)ColIndex.Status].Value = "FAIL";
                    Row.Cells[(int)ColIndex.Status].Style.BackColor = Color.Red;

                    break;
                case ItemAccessStatus.Stopped:
                    //If StatusText = "" Then StatusText = Date.Now.ToString("HH:mm:ss")
                    Row.DefaultCellStyle.BackColor = mdgv.DefaultCellStyle.BackColor;
                    Row.Cells[(int)ColIndex.Status].Value = "STOPPED";
                    Row.Cells[(int)ColIndex.Status].Style.BackColor = Color.Yellow;

                    break;
                case ItemAccessStatus.Passed:
                    //If StatusText = "" Then StatusText = Date.Now.ToString("HH:mm:ss")
                    Row.DefaultCellStyle.BackColor = mdgv.DefaultCellStyle.BackColor;
                    Row.Cells[(int)ColIndex.Status].Value = "PASS";
                    Row.Cells[(int)ColIndex.Status].Style.BackColor = Color.LawnGreen;

                    break;
                case ItemAccessStatus.UnkownSkipped:
                    //If StatusText = "" Then StatusText = "Unknown Script!"
                    Row.DefaultCellStyle.BackColor = mdgv.DefaultCellStyle.BackColor;
                    Row.Cells[(int)ColIndex.Status].Value = "SKIPPED";
                    Row.Cells[(int)ColIndex.Status].Style.BackColor = Color.BlueViolet;

                    break;
            }

            Row.Cells[(int)ColIndex.Results].Value = ResultText;
            Row.Cells[(int)ColIndex.Log].Value = LogFilePath;

            if (mdgv.Visible)
            {
                mdgv.FirstDisplayedScrollingRowIndex = Index;
                mdgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }

            mdgv.Refresh();
        }


        public void LoadDuts(int number)
        {
            mdgv.Rows.Add(number);
            for (int i = 1; i <= number; i++)
            {
                mdgv.Rows[i - 1].HeaderCell.Value = i.ToString();
            }

        }

        private void SetupGUI()
        {
            int i = 0;

            mInternalChange = true;

            var _with1 = mdgv;
            _with1.RowHeadersVisible = true;
            _with1.ColumnHeadersVisible = true;
            _with1.AllowUserToAddRows = false;
            _with1.AllowUserToDeleteRows = false;
            _with1.AllowUserToResizeRows = false;
            _with1.AllowUserToResizeColumns = false;

            _with1.ColumnCount = Enum.GetValues(typeof(ColIndex)).Length;

            _with1.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8);
            _with1.RowHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
            _with1.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
            _with1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //.ColumnHeadersHeight = 30
            _with1.RowHeadersWidth = 50;

            _with1.Columns.RemoveAt((int)ColIndex.Selected);
            _with1.Columns.Insert((int)ColIndex.Selected, new DataGridViewCheckBoxColumn());
            //.Columns.RemoveAt(ColIndex.Alps)
            //.Columns.Insert(ColIndex.Alps, New DataGridViewCheckBoxColumn)
            //.Columns.RemoveAt(ColIndex.LightPath)
            //.Columns.Insert(ColIndex.LightPath, New DataGridViewCheckBoxColumn)

            _with1.Columns[(int)ColIndex.Selected].HeaderText = "Selected?";
            _with1.Columns[(int)ColIndex.SN].HeaderText = "   SN   ";
            //.Columns(ColIndex.Alps).HeaderText = "Alps"
            //.Columns(ColIndex.LightPath).HeaderText = "LightPath"
            _with1.Columns[(int)ColIndex.Status].HeaderText = "Status";
            _with1.Columns[(int)ColIndex.Results].HeaderText = "Results";
            _with1.Columns[(int)ColIndex.Log].HeaderText = "Log";

            _with1.ReadOnly = false;
            for (i = 0; i <= _with1.ColumnCount - 1; i++)
            {
                _with1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                _with1.Columns[i].ReadOnly = true;
            }

            _with1.Columns[(int)ColIndex.SN].ReadOnly = false;
            _with1.Columns[(int)ColIndex.Selected].ReadOnly = false;
            //.Columns(ColIndex.Alps).ReadOnly = False
            //.Columns(ColIndex.LightPath).ReadOnly = False
            _with1.Columns[(int)ColIndex.Log].ReadOnly = false;

            _with1.Columns[(int)ColIndex.SN].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            _with1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            _with1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;


            w2.w2ContextMenu cm = new w2.w2ContextMenu(mdgv);
            cm.Remove("Delete");
            cm.MenuHandlingSuppressed = true;
            mdgv.ContextMenuStrip.ItemClicked += dgvContextMenu_ItemClicked;

            mInternalChange = false;

        }

        private void dgvContextMenu_ItemClicked(System.Object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            //flag
            mInternalChange = true;

            //do change
            ((ContextMenuStrip)sender).Visible = false;
            switch (e.ClickedItem.Text)
            {
                case "Copy":
                    mdgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
                    mdgv.Focus();
                    SendKeys.Send("^C");

                    break;
                case "Select None":
                    w2.DataGridViewHelper.Helper.ClearColumnValue(mdgv.Columns[mdgv.SelectedCells[0].ColumnIndex]);
                    break;
                //Select Case mdgv.SelectedCells(0).ColumnIndex
                //    Case ColIndex.Alps
                //        w2.DataGridViewHelper.Helper.ClearColumnValue(mdgv.Columns(ColIndex.Alps))
                //    Case 3
                //        w2.DataGridViewHelper.Helper.ClearColumnValue(mdgv.Columns(ColIndex.LightPath))
                //    Case Else
                //        w2.DataGridViewHelper.Helper.ClearColumnValue(mdgv.Columns(ColIndex.Selected))
                //End Select

                case "Select All":
                    w2.DataGridViewHelper.Helper.SetColumnValue(mdgv.Columns[mdgv.SelectedCells[0].ColumnIndex], 0, mdgv.RowCount - 1, true);
                    break;
                //Select Case mdgv.SelectedCells(0).ColumnIndex
                //    Case 2
                //        w2.DataGridViewHelper.Helper.SetColumnValue(mdgv.Columns(ColIndex.Alps), 0, mdgv.RowCount - 1, True)
                //    Case 3
                //        w2.DataGridViewHelper.Helper.SetColumnValue(mdgv.Columns(ColIndex.LightPath), 0, mdgv.RowCount - 1, True)
                //    Case Else
                //        w2.DataGridViewHelper.Helper.SetColumnValue(mdgv.Columns(ColIndex.Selected), 0, mdgv.RowCount - 1, True)
                //End Select

                case "Clear All":
                    foreach (DataGridViewRow Row in mdgv.Rows)
                    {
                        Row.DefaultCellStyle.BackColor = mdgv.DefaultCellStyle.BackColor;
                        Row.Cells[(int)ColIndex.Selected].Value = false;
                        //Row.Cells(ColIndex.Alps).Value = False
                        //Row.Cells(ColIndex.LightPath).Value = False
                        Row.Cells[(int)ColIndex.SN].Value = "";
                        Row.Cells[(int)ColIndex.Status].Value = "";
                        Row.Cells[(int)ColIndex.Status].Style.BackColor = mdgv.DefaultCellStyle.BackColor;
                        Row.Cells[(int)ColIndex.Results].Value = "";
                        Row.Cells[(int)ColIndex.Log].Value = "";
                    }

                    break;
            }

            //flag
            mInternalChange = false;
        }

        private void mdgv_CellValueChanged(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            //ignore process if not called from GUI
            if (mInternalChange)
                return;

            //set up flag now
            mInternalChange = true;

            switch (mdgv.CurrentCell.ColumnIndex)
            {
                case (int)ColIndex.Selected:
                    break;
            }

            //clear flag
            mInternalChange = false;
        }

        private void mdgv_CurrentCellDirtyStateChanged(object sender, System.EventArgs e)
        {
            if (mdgv.CurrentCell.ColumnIndex == (int)ColIndex.Selected && mdgv.IsCurrentCellDirty)
            {
                mdgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void mdgv_DoubleClick(object sender, System.EventArgs e)
        {
            string s = null;

            try
            {
                if (mdgv.CurrentCell == null)
                    return;
                if (mdgv.CurrentCell.ColumnIndex != (int)ColIndex.Log)
                    return;
                s = w2.DataGridViewHelper.Helper.GetCellText(mdgv.CurrentCell).ToString();
                if (!string.IsNullOrEmpty(s))
                    Process.Start(s);

            }
            catch (Exception ex)
            {
                string ss = ex.ToString();
            }
        }

        private void mdgv_VisibleChanged(object sender, System.EventArgs e)
        {
            if (mdgv.Visible)
            {
                mdgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
        }

        public bool ParseDuts()
        {
            mItems.Clear();
            LastAccessedItem = 0;

            for (int i = 1; i <= mdgv.Rows.Count; i++)
            {
                mItems.Add(new w2DutItem(i, Convert.ToBoolean(mdgv.Rows[i-1].Cells[(int)ColIndex.Selected].Value), Convert.ToString(mdgv.Rows[i-1].Cells[(int)ColIndex.SN].Value)));

                //make sure SN is not empty
                if (Convert.ToBoolean(mdgv.Rows[i-1].Cells[(int)ColIndex.Selected].Value))
                {
                    if (string.IsNullOrEmpty(Convert.ToString(mdgv.Rows[i-1].Cells[(int)ColIndex.SN].Value)))
                    {
                        MessageBox.Show("SN Cannot Be Empty for Any Selected Unit!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            return true;

        }

        public List<w2DutItem> Items
        {
            get { return mItems; }
        }

        protected int mLastAccessedItem;

        protected ItemAccessStatus mLastAccessedItemStatus;
        public bool DutFinished
        {
            get
            {
                bool Finished = false;
                Finished = (mLastAccessedItemStatus == ItemAccessStatus.UnkownSkipped) | (mLastAccessedItemStatus == ItemAccessStatus.Passed);
                return (mLastAccessedItem == mItems.Count - 1) & Finished;
            }
        }

        public int LastAccessedItem
        {
            get { return mLastAccessedItem; }
            set { mLastAccessedItem = value; }
        }

        public ItemAccessStatus LastAccessedItemStatus
        {
            get { return mLastAccessedItemStatus; }
            set { mLastAccessedItemStatus = value; }
        }

        public int QueryResumeTestEntryIndex()
        {
            string s = null;
            int iStart = 0;
            int iDisplay = 0;
            DialogResult response = default(DialogResult);

            if (this.DutFinished)
            {
                s = "Script already finished. Restart from the beginning?";
                response = MessageBox.Show(s, "Resume Test", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (response == DialogResult.Yes)
                {
                    iStart = 0;
                }
                else
                {
                    iStart = -1;
                }
            }
            else
            {
                switch (mLastAccessedItemStatus)
                {
                    case ItemAccessStatus.Passed:
                    case ItemAccessStatus.UnkownSkipped:
                        iStart = mLastAccessedItem + 1;
                        break;
                    default:
                        iDisplay = mLastAccessedItem + 1;
                        //THE DISPLAY IS 1 BASED, INTERNAL IS 0 BASED
                        s = "Step " + iDisplay.ToString() + ": " + mItems[mLastAccessedItem].SN;
                        s += ControlChars.CrLf + "is not finished successfully.";
                        s += ControlChars.CrLf + "Do you want to skip that and resume from Step " + (iDisplay + 1).ToString();
                        response = MessageBox.Show(s, "Resume Test", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                        if (response == DialogResult.Yes)
                        {
                            iStart = mLastAccessedItem + 1;
                        }
                        else
                        {
                            iStart = mLastAccessedItem;
                        }
                        break;
                }
            }

            return iStart;
        }


    }

    public class w2DutItem
    {
        private int mNumber;
        private bool mSelected;
        private string mSN;
        private string mStatus;
        private string mResults;

        private string mLog;
        public w2DutItem(int Number, bool IsSelected, string SN)
        {
            mNumber = Number;
            mSelected = IsSelected;
            mSN = SN;
        }

        public int Number
        {
            get { return mNumber; }
        }

        public bool IsSelected
        {
            get { return mSelected; }
        }

        public string SN
        {
            get { return mSN; }
        }

        public string Status
        {
            get { return mStatus; }
            set { mStatus = value; }
        }

        public string Results
        {
            get { return mResults; }
            set { mResults = value; }
        }

        public string Log
        {
            get { return mLog; }
            set { mLog = value; }
        }
    }
}
