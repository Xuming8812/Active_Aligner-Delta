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
    public partial class JogPanel : Form
    {
        #region "Decleration"
        private Label[] Lbl_AxisName;
        private TextBox[] Txt_CurrentPosition;
        private Button[] Btn_Move;
        private Button[] Btn_JogAdd;
        private Button[] Btn_JogSub;
        private NumericUpDown[] Nud_TargetPosition;
        private NumericUpDown[] Nud_Jog;

        private int AxisNumber;

        private iStage mStagebase;

        #endregion


        public JogPanel(ref iStage hStageBase)
        {
            InitializeComponent();

            mStagebase = hStageBase;

            AxisNumber = Enum.GetValues(typeof(iStage.AxisNameEnum)).Length;

            this.SetupGUI();

            this.SetupNud();

            tmrSync.Start();
        }

        #region "Setup GUI"

        public void SetupGUI()
        {
            Lbl_AxisName = new Label[AxisNumber];
            Txt_CurrentPosition = new TextBox[AxisNumber];
            Btn_Move = new Button[AxisNumber];
            Btn_JogAdd = new Button[AxisNumber];
            Btn_JogSub = new Button[AxisNumber];
            Nud_TargetPosition = new NumericUpDown[AxisNumber];
            Nud_Jog = new NumericUpDown[AxisNumber];

            for (int i = 0; i < AxisNumber; i++)
            {
                //create label for each axis
                Label lbl = new Label();
                lbl.Size = new Size(70, 22);
                lbl.Location = new Point(10,46+i*30);
                lbl.Text = "AxisName" + i.ToString();
                Lbl_AxisName[i] = lbl;
                this.Controls.Add(lbl);

                //Create txtbox for currentposition
                TextBox txt = new TextBox();
                txt.Size = new Size(100, 22);
                txt.Location = new Point(90, 46 + i * 30);
                txt.Name = "txtCurrentPos" + i.ToString();
                Txt_CurrentPosition[i] = txt;
                this.Controls.Add(txt);

                //create target position nud of each axis
                NumericUpDown nudTarget = new NumericUpDown();
                nudTarget.Size = new Size(100,22);
                nudTarget.Location = new Point(200, 46 + i * 30);
                nudTarget.Name = "nudTarget" + i.ToString();
                Nud_TargetPosition[i] = nudTarget;
                this.Controls.Add(nudTarget);

                //create move button
                Button btnMove = new Button();
                btnMove.Size = new Size(70,22);
                btnMove.Location = new Point(310, 46 + i * 30);
                btnMove.Text = "Move";
                btnMove.Name = "btnJogAddAxis" + i.ToString();
                Btn_Move[i] = btnMove;
                this.Controls.Add(btnMove);

                //create nud Jog value
                NumericUpDown nudJog = new NumericUpDown();
                nudJog.Size = new Size(90, 22);
                nudJog.Location = new Point(390, 46 + i * 30);
                nudJog.Name = "nudJog" + i.ToString();
                Nud_Jog[i] = nudJog;
                this.Controls.Add(nudJog);

                //Create jog add button
                Button btnAdd = new Button();
                btnAdd.Size = new Size(50, 22);
                btnAdd.Location = new Point(490, 46 + i * 30);
                btnAdd.Text = "Add";
                btnAdd.Name = "btnJogAddAxis" + i.ToString();
                Btn_JogAdd[i] = btnAdd;
                this.Controls.Add(btnAdd);

                //Create jog add button
                Button btnSub = new Button();
                btnSub.Size = new Size(50, 22);
                btnSub.Location = new Point(550, 46 + i * 30);
                btnSub.Text = "Sub";
                btnSub.Name = "btnJogSubAxis" + i.ToString();
                Btn_JogSub[i] = btnSub;
                this.Controls.Add(btnSub);
            }

            List<Control> ctrl = new List<Control>();
            ctrl.Clear();
            w2.w2Misc.GetAllControls<Button>(this,ctrl);
            foreach (Button btn in ctrl)
            {
                btn.Click += Btn_Click;
            }
        }

        public void SetupNud()
        {

        }
        #endregion

        #region "Callback"
        private void Btn_Click(object sender, EventArgs e)
        {
            Button btn;
            btn = (Button)sender;

            if (btn.Name.Contains("Close"))
            {
                this.Close();
            }
            else if (btn.Name.Contains("Move"))
            {

            }
            else if (btn.Name.Contains("JogAdd"))
            {

            }
            else if (btn.Name.Contains("JogSub"))
            {

            }

        }

        private void tmrSync_Tick(object sender, EventArgs e)
        {
            this.SyncPosition();
        }
        #endregion

        #region "Sync"
        private void SyncPosition()
        {

        }

        private void StartSync()
        { }

        private void StopSync()
        { }

#endregion
    }
}
