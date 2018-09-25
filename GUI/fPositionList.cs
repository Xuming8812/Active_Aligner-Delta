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
    public partial class PositionList : Form
    {
        #region "Decleration"
        private TextBox[] txtAxisName;
        private TextBox[] txtCurrentPosition;
        private TextBox[] txtConfiguredPosition;
        private CheckBox[] chkEnableAxis;

        private int AxisNumber;

        private bool[] axisCheckedStatus;

        private iStage mStagebase;
        #endregion
        public PositionList(ref iStage hStageBase)
        {
            InitializeComponent();

            mStagebase = hStageBase;

            AxisNumber = Enum.GetValues(typeof(iStage.AxisNameEnum)).Length;

            this.SetupTableLayout();
        }


        #region "Setup GUI"
        private void SetupTableLayout()
        {
            tbLayout.Location = new Point(440, 20);
            tbLayout.AutoScroll = true;
            tbLayout.ColumnCount = 4;
            tbLayout.RowCount = 1;

            tbLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

            tbLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            tbLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            tbLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            tbLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

            for (int i = 0; i < AxisNumber + 1; i++)
            {
                tbLayout.RowCount++;
                tbLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
                // create txtAxisName
                TextBox txt1 = new TextBox();
                txt1.Name = "txtAxisName" + i.ToString();
                txt1.Anchor = AnchorStyles.None;
                txtAxisName[i] = txt1;
                tbLayout.Controls.Add(txt1, 0, i);
                // create txtCurrentPosition
                TextBox txt2 = new TextBox();
                txt2.Name = "txtCurrentPositionAxis" + i.ToString();
                txt2.Anchor = AnchorStyles.None;
                txtCurrentPosition[i] = txt2;
                tbLayout.Controls.Add(txt2, 1, i);
                // create txtSavedPosition
                TextBox txt3 = new TextBox();
                txt3.Name = "txtConfiguredPositionAxis" + i.ToString();
                txt3.Anchor = AnchorStyles.None;
                txtConfiguredPosition[i] = txt3;
                tbLayout.Controls.Add(txt3, 2, i);
                // create txtSavedPosition
                CheckBox chk = new CheckBox();
                chk.Name = "chkEnableAxis" + i.ToString();
                chk.Anchor = AnchorStyles.None;
                chkEnableAxis[i] = chk;
                tbLayout.Controls.Add(chk, 3, i);
            }
        }

        private void FilterEnableAxis()
        {
            for (int i = 0; i < AxisNumber; i++)
            {
                if (!axisCheckedStatus[i])
                {
                    txtAxisName[i].Visible = false;
                    txtCurrentPosition[i].Visible = false;
                    txtConfiguredPosition[i].Visible = false;
                    chkEnableAxis[i].Visible = false;

                    tbLayout.RowStyles[i].Height = 0;
                }
                else
                {
                    txtAxisName[i].Visible = true;
                    txtCurrentPosition[i].Visible = true;
                    txtConfiguredPosition[i].Visible = true;
                    chkEnableAxis[i].Visible = true;

                    tbLayout.RowStyles[i].Height = 30;
                }
            }
        }

        private void SetupTreeView()
        {
            int headerCount = 0;
            int headerIndex = 0;
            int totalIndex = 1;

            
            // count headers
            foreach (iStage.ConfiguredStagePosition x in mStagebase.ConfiguredPositions.Values)
            {
                if (x.isHeader)
                {
                    headerCount += 1;
                }
            }

            string[] ParentNodeName = new string[headerCount];
            TreeNode[] ParentNode = new TreeNode[headerCount];

            foreach (iStage.ConfiguredStagePosition x in mStagebase.ConfiguredPositions.Values)
            {
                if (x.isHeader)
                {
                    TreeNode pNode = new TreeNode();
                    pNode.Text = totalIndex.ToString() + " " + x.Label;
                    pNode.Name = "pNode" + x.Label.Trim().Replace(" ", "");
                    pNode.Tag = headerIndex;
                    ParentNodeName[headerIndex] = x.Label.Trim().Replace(" ", "");
                    ParentNode[headerIndex] = pNode;
                    headerIndex++;
                    treePositionList.Nodes.Add(pNode);
                }
                else
                {
                    if (x.ParentLabel.Contains(ParentNodeName[headerIndex - 1]))
                    {
                        TreeNode cNode = new TreeNode();
                        cNode.Text = totalIndex.ToString() + " " + x.Label;
                        cNode.Text = "cNode" + x.Label.Trim().Replace(" ", "");
                        cNode.Tag = ParentNode[headerIndex - 1].Tag;
                        ParentNode[headerIndex - 1].Nodes.Add(cNode);
                    }
                }

                totalIndex++;
            }

            treePositionList.ExpandAll();

        }

        private void SelectTreeView(int selectIndex)
        {
            treePositionList.Focus();
            int index = 0;

            for (int i = 0; i < treePositionList.Nodes.Count; i++)
            {
                for (int j = 0; j < treePositionList.Nodes[i].Nodes.Count; j++)
                {
                    index += 1;
                    if (index == selectIndex)
                    {
                        if (j == 0)
                        {
                            treePositionList.SelectedNode = treePositionList.Nodes[i];
                        }
                        else
                        {
                            treePositionList.SelectedNode = treePositionList.Nodes[i].Nodes[j - 1];
                        }
                        treePositionList.Nodes[i].Expand();

                        return;

                    }
                }

            }
        }

        #endregion

        private void treePositionList_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            int index = 0;
            double v = 0;

            TreeNode node = e.Node;
            string[] indexStr = node.Text.Split(Convert.ToChar(" "));
            index = Convert.ToInt16(indexStr[0]);

            for (int i = 0; i < AxisNumber; i++)
            {
                v = mStagebase.ConfiguredPosition((iStage.StagePositionEnum)index).Positions[i];
                if (double.IsNaN(v))
                {
                    txtConfiguredPosition[i].Text = "NAN";
                    chkEnableAxis[i].Checked = false;
                }
                else
                {
                    txtConfiguredPosition[i].Text = v.ToString();
                    chkEnableAxis[i].Checked = true ;
                }

            }

        }
    }
}
