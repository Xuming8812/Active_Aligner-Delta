using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Delta
{
    public partial class fPartTray : Form
    {
        #region "Decleration"

        private const int Gap = 5;

        private int PartWidth;
        private int PartHeight;

        public enum PartStatus
        {
            Filled,
            Empty,
            VisionFailed,
            Pass,
            Failed,
            Thrown
        }

        struct PartTrayInfo
        {
            public string PartNameStr;
            public int RowNum;
            public int ColumnNum;
            public double RowSpace;
            public double ColumnSpace;
        }

        private int PartStatusCount = Enum.GetNames(typeof(PartStatus)).Length;

        private int mRows;
        private int mCols;

        public int Rows
        {
            get
            {
                return mRows;
            }
        }

        public int Cols
        {
            get
            {
                return mCols;
            }
        }
        private Color[] mStatusColor;
        private Label[] mParts;

        private PartStatus mPartStatus;

        bool IsMouseDown = false;
        Rectangle MouseRect = Rectangle.Empty;

        private w2.w2IniFile mIniFile;

        #endregion


        public fPartTray(ref w2.w2IniFile hIniFile)
        {
            InitializeComponent();

            mIniFile = hIniFile;

            mRows = mIniFile.ReadParameter("PartTray", "RowNumber", 0);
            mCols = mIniFile.ReadParameter("PartTray", "ColumnNumber", 0);

            PartWidth = (int)(panelTray.Size.Width - (mRows + 1) * Gap) / mRows;
            PartHeight = (int)(panelTray.Size.Height - (mCols + 1) * Gap) / mCols;
        }

        #region "Public Functions"

        #endregion

        #region "Local Functions"
        private void BuildTray()
        {
            Label lbl;
            int left, top;
            int i, ii, idx;

            List<Control> ctrl;

            idx = 0;

            top = Gap;

            for (i = 0; i < mRows; i++)
            {
                left = Gap;
                for (ii = 0; ii < mCols; ii++)
                {
                    idx += 1;

                    lbl = new Label();
                    lbl.Parent = panelTray;
                    lbl.Size = new Size(PartWidth, PartHeight);
                    lbl.Location = new Point(left, top);
                    lbl.BackColor = mStatusColor[(int)PartStatus.Empty];
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.Font = new Font("Courier New", 10, FontStyle.Bold);
                    lbl.Text = idx.ToString();
                    lbl.Visible = true;
                    lbl.Tag = idx;

                    mParts[idx - 1] = lbl;
                    lbl.Click += ctrl_Click;

                    left += (PartWidth + Gap);
                }
                top += (PartHeight + Gap);
            }

            ctrl = new List<Control>();
            w2.w2Misc.GetAllControls<Button>(this, ctrl);

            foreach (Button btn in ctrl)
            {
                btn.Click += ctrl_Click;
            }

            this.KeyPreview = true;

        }

        #endregion

        #region "Callbacks"

        #endregion
    }
}
