using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Delta
{
    public partial class fController : Form
    {
        private enum XpsColIndex
        {
            Axis = 0,
            Enabled = 1,
            Position = 2,
            Min = 3,
            Max = 4,

            NewPosition = 5,
            RelativeMove = 6,

            Move = 7,
            Home = 8,

            iStatus = 9,
            sStatus = 10
        }

        private Instrument.iXPS mXPS;
        private iStage mStageBase;
        private DeltaFunction.StageFunctions mStageTool;
        private DeltaFunction mTool;

        private Control[] mDgvConfig;
        private bool mStop;
        private bool mSync;
        private w2ConfigTableHelper mSettingHelper;

        public fController(ref DeltaFunction hTool)
        {
            InitializeComponent();

            mTool = hTool;

            mXPS = mTool.Instruments.XPS;

            mStageBase = mTool.Instruments.StageBase;
            mStageTool = mTool.StageTool;

        }



        private void SetupGUI()
        {
            List<Control> ctrl = new List<Control>();
            w2NumericUpDownHelper x = default(w2NumericUpDownHelper);

            //flag
            mSync = true;

            //buttons
            ctrl.Clear();
            w2.w2Misc.GetAllControls<Button>(this, ctrl);
            foreach (Button btn in ctrl)
            {
                btn.Click += btn_Click;
            }

            //text box for readout
            ctrl.Clear();
            w2.w2Misc.GetAllControls<TextBox>(this, ctrl);
            foreach (TextBox txt in ctrl)
            {
                //we need to eliminate the nud being collected as text box
                if (txt.Parent is NumericUpDown)
                    continue;
                txt.BackColor = System.Drawing.Color.FromArgb(235, 235, 235);
                txt.BorderStyle = BorderStyle.FixedSingle;
                txt.KeyDown += txt_KeyDown;
            }

            //nud
            ctrl.Clear();
            w2.w2Misc.GetAllControls<NumericUpDown>(this, ctrl);
            foreach (NumericUpDown nud in ctrl)
            {
                nud.ValueChanged += nud_ValueChanged;
                x = new w2NumericUpDownHelper(nud, "ControllerGUI.ini");
                nud.BorderStyle = BorderStyle.FixedSingle;
            }

            //check
            ctrl.Clear();
            w2.w2Misc.GetAllControls<CheckBox>(this, ctrl);
            foreach (CheckBox chk in ctrl)
            {
                chk.CheckedChanged += chk_CheckedChanged;
                chk.FlatStyle = FlatStyle.Standard;
            }

            //radio button
            ctrl.Clear();
            w2.w2Misc.GetAllControls<RadioButton>(this, ctrl);
            foreach (RadioButton opt in ctrl)
            {
                opt.CheckedChanged += opt_CheckedChanged;
                opt.FlatStyle = FlatStyle.Standard;
            }

            this.lstConfiguredPositions.Size = new Size(195, this.Height - 115);
            //done
            mSync = false;
        }

        #region "Sync"

        private void SyncStagePositions()
        {
            mSync = true;

            //stage
            if (mXPS == null)
            {
                //disable all the buttons? 
            }
            else
            {
                //raw stage info
                if (mStageBase.IsStageReady(iStage.AxisNameEnum.PiLS))
                {
                    txtPiLS.Text = mStageBase.GetStagePosition(iStage.AxisNameEnum.PiLS).ToString("0.0000");
                    btnMovePiLS.Enabled = true;
                    btnJogAddPiLS.Enabled = true;
                    btnJogSubPiLS.Enabled = true;
                }
                else
                {
                    //txtAngle.Text = "N/A"
                    //txtAngle.Enabled = False
                    btnMovePiLS.Enabled = false;
                    btnJogAddPiLS.Enabled = false;
                    btnJogSubPiLS.Enabled = false;
                }

                if (mStageBase.IsStageReady(iStage.AxisNameEnum.AngleMain))
                {
                    txtAngleLens.Text = mStageBase.GetStagePosition(iStage.AxisNameEnum.AngleMain).ToString("0.0000");
                    btnMoveAngleLens.Enabled = true;
                    btnJogAddAngleLens.Enabled = true;
                    btnJogSubAngleLens.Enabled = true;
                }
                else
                {
                    //txtAngle.Text = "N/A"
                    //txtAngle.Enabled = False
                    btnMoveAngleLens.Enabled = false;
                    btnJogAddAngleLens.Enabled = false;
                    btnJogSubAngleLens.Enabled = false;
                }

                if (mStageBase.IsStageReady(iStage.AxisNameEnum.AngleHexapod))
                {
                    txtAnglePbs.Text = mStageBase.GetStagePosition(iStage.AxisNameEnum.AngleHexapod).ToString("0.0000");
                    btnMoveAnglePbs.Enabled = true;
                    btnJogAddAnglePbs.Enabled = true;
                    btnJogSubAnglePbs.Enabled = true;
                }
                else
                {
                    //txtAngle.Text = "N/A"
                    //txtAngle.Enabled = False
                    btnMoveAnglePbs.Enabled = false;
                    btnJogAddAnglePbs.Enabled = false;
                    btnJogSubAnglePbs.Enabled = false;
                }

                if (mStageBase.IsStageReady(iStage.AxisNameEnum.Probe))
                {
                    txtProbe.Text = mStageBase.GetStagePosition(iStage.AxisNameEnum.Probe).ToString("0.0000");
                    //txtProbe2.Text = txtProbe.Text
                    btnMoveProbe.Enabled = true;
                    btnJogAddProbe.Enabled = true;
                    btnJogSubProbe.Enabled = true;
                }
                else
                {
                    //txtBeamGage.Text = "N/A"
                    //txtBeamGage.Enabled = False
                    btnMoveProbe.Enabled = false;
                    btnJogAddProbe.Enabled = false;
                    btnJogSubProbe.Enabled = false;
                }

                if (mStageBase.IsStageReady(iStage.AxisNameEnum.BeamScanX))
                {
                    txtBeamScanX.Text = mStageBase.GetStagePosition(iStage.AxisNameEnum.BeamScanX).ToString("0.0000");
                    btnMoveBeamScanX.Enabled = true;
                    btnJogAddBeamScanX.Enabled = true;
                    btnJogSubBeamScanX.Enabled = true;
                }
                else
                {
                    //txtPigtailX.Text = "N/A"
                    //txtPigtailX.Enabled = False
                    btnMoveBeamScanX.Enabled = false;
                    btnJogAddBeamScanX.Enabled = false;
                    btnJogSubBeamScanX.Enabled = false;
                }

                if (mStageBase.IsStageReady(iStage.AxisNameEnum.BeamScanY))
                {
                    txtBeamScanY.Text = mStageBase.GetStagePosition(iStage.AxisNameEnum.BeamScanY).ToString("0.0000");
                    btnMoveBeamScanY.Enabled = true;
                    btnJogAddBeamScanY.Enabled = true;
                    btnJogSubBeamScanY.Enabled = true;
                }
                else
                {
                    //txtPigtailY.Text = "N/A"
                    //txtPigtailY.Enabled = False
                    btnMoveBeamScanY.Enabled = false;
                    btnJogAddBeamScanY.Enabled = false;
                    btnJogSubBeamScanY.Enabled = false;
                }

                if (mStageBase.IsStageReady(iStage.AxisNameEnum.BeamScanZ))
                {
                    txtBeamScanZ.Text = mStageBase.GetStagePosition(iStage.AxisNameEnum.BeamScanZ).ToString("0.0000");
                    btnMoveBeamScanZ.Enabled = true;
                    btnJogAddBeamScanZ.Enabled = true;
                    btnJogSubBeamScanZ.Enabled = true;
                }
                else
                {
                    //txtPigtailZ.Text = "N/A"
                    //txtPigtailZ.Enabled = False
                    btnMoveBeamScanZ.Enabled = false;
                    btnJogAddBeamScanZ.Enabled = false;
                    btnJogSubBeamScanZ.Enabled = false;
                }

                if (mStageBase.IsStageReady(iStage.AxisNameEnum.StageX))
                {
                    txtStageX.Text = mStageBase.GetStagePosition(iStage.AxisNameEnum.StageX).ToString("0.0000");
                    btnMoveStageX.Enabled = true;
                    btnJogAddStageX.Enabled = true;
                    btnJogSubStageX.Enabled = true;
                }
                else
                {
                    //txtStageX.Text = "N/A"
                    //txtStageX.Enabled = False
                    btnMoveStageX.Enabled = false;
                    btnJogAddStageX.Enabled = false;
                    btnJogSubStageX.Enabled = false;
                }

                if (mStageBase.IsStageReady(iStage.AxisNameEnum.StageY))
                {
                    txtStageY.Text = mStageBase.GetStagePosition(iStage.AxisNameEnum.StageY).ToString("0.0000");
                    btnMoveStageY.Enabled = true;
                    btnJogAddStageY.Enabled = true;
                    btnJogSubStageY.Enabled = true;
                }
                else
                {
                    //txtStageY.Text = "N/A"
                    txtStageY.Enabled = false;
                    btnMoveStageY.Enabled = false;
                    btnJogAddStageY.Enabled = false;
                    btnJogSubStageY.Enabled = false;
                }

                if (mStageBase.IsStageReady(iStage.AxisNameEnum.StageZ))
                {
                    txtStageZ.Text = mStageBase.GetStagePosition(iStage.AxisNameEnum.StageZ).ToString("0.0000");
                    btnMoveStageZ.Enabled = true;
                    btnJogAddStageZ.Enabled = true;
                    btnJogSubStageZ.Enabled = true;
                }
                else
                {
                    //txtStageZ.Text = "N/A"
                    txtStageZ.Enabled = false;
                    btnMoveStageZ.Enabled = false;
                    btnJogAddStageZ.Enabled = false;
                    btnJogSubStageZ.Enabled = false;
                }

                //If mStageBase.IsStageReady(iStage.AxisNameEnum.Probe) Then
                //    txtClampPos.Text = mStageBase.GetStagePosition(iStage.AxisNameEnum.Probe).ToString("0.0000")
                //    btnMoveProbe.Enabled = True
                //    btnJogAddProbe.Enabled = True
                //    btnJogSubProbe.Enabled = True
                //Else
                //    txtClampPos.Enabled = False
                //    btnMoveProbe.Enabled = False
                //    btnJogAddProbe.Enabled = False
                //    btnJogSubProbe.Enabled = False
                //End If

                //check actual/target 
                this.SyncStageTargetColor();

            }

            mSync = false;
        }

        private void SyncStageTargetColor()
        {
            this.SetStageTargetColor(ref txtStageX, nudStageX);
            this.SetStageTargetColor(ref txtStageY, nudStageY);
            this.SetStageTargetColor(ref txtStageZ, nudStageZ);

            this.SetStageTargetColor(ref txtBeamScanX, nudBeamScanX);
            this.SetStageTargetColor(ref txtBeamScanY, nudBeamScanY);
            this.SetStageTargetColor(ref txtBeamScanZ, nudBeamScanZ);

            this.SetStageTargetColor(ref txtAngleLens, nudAngleLens);

            this.SetStageTargetColor(ref txtPiLS, nudPiLS);

            this.SetStageTargetColor(ref txtProbe, nudProbe);
        }

        private void SetStageTargetColor(ref TextBox txt, NumericUpDown nud)
        {
            string s = null;
            decimal v = default(decimal);

            const double Tol = 0.001;

            s = txt.Text.Trim();
            if (string.IsNullOrEmpty(s))
            {
                nud.BackColor = SystemColors.Window;
            }
            else
            {
                v = Convert.ToDecimal(s);
                if (Math.Abs(v - nud.Value) < Convert.ToDecimal(Tol))
                {
                    nud.BackColor = SystemColors.Window;
                }
                else
                {
                    nud.BackColor = Color.LightPink;
                }
            }

        }

        private void SetStageTargetToCurrentPosition()
        {
            string s = null;

            s = txtStageX.Text;
            if (!string.IsNullOrEmpty(s))
                nudStageX.Value = Convert.ToDecimal(s);
            s = txtStageY.Text;
            if (!string.IsNullOrEmpty(s))
                nudStageY.Value = Convert.ToDecimal(s);
            s = txtStageZ.Text;
            if (!string.IsNullOrEmpty(s))
                nudStageZ.Value = Convert.ToDecimal(s);

            s = txtBeamScanX.Text;
            if (!string.IsNullOrEmpty(s))
                nudBeamScanX.Value = Convert.ToDecimal(s);
            s = txtBeamScanY.Text;
            if (!string.IsNullOrEmpty(s))
                nudBeamScanY.Value = Convert.ToDecimal(s);
            s = txtBeamScanZ.Text;
            if (!string.IsNullOrEmpty(s))
                nudBeamScanZ.Value = Convert.ToDecimal(s);

            s = txtAngleLens.Text;
            if (!string.IsNullOrEmpty(s))
                nudAngleLens.Value = Convert.ToDecimal(s);
            s = txtAnglePbs.Text;
            if (!string.IsNullOrEmpty(s))
                nudAnglePbs.Value = Convert.ToDecimal(s);

            s = txtPiLS.Text;
            if (!string.IsNullOrEmpty(s))
                nudPiLS.Value = Convert.ToDecimal(s);

            s = txtProbe.Text;
            if (!string.IsNullOrEmpty(s))
                nudProbe.Value = Convert.ToDecimal(s);
        }
        #endregion

        #region "Configured position management"
        public void LoadConfiguredPositionList()
        {
            string s = null;
            //iStage.ConfiguredStagePosition x = default(iStage.ConfiguredStagePosition);

            //build the configured position list
            lstConfiguredPositions.SelectionMode = SelectionMode.One;
            lstConfiguredPositions.Items.Clear();
            foreach (iStage.ConfiguredStagePosition x in mStageBase.ConfiguredPositions.Values)
            {
                //we will not load the safe position to GUI to avoid accidental changes
                s = x.Label.ToLower();
                if (s.Contains("safe"))
                    continue;
                lstConfiguredPositions.Items.Add(x);
            }
            lstConfiguredPositions.SelectedIndex = 0;
        }

        private void SaveConfiguredPositions(bool NewEntry)
        {
            string s = null;
            string Label = null;
            int i = 0;
            int ii = 0;
             DialogResult r = default( DialogResult);
            double[] Positions = new double[iStage.AxisCount];
            iStage.ConfiguredStagePosition x = default(iStage.ConfiguredStagePosition);
            iStage.ConfiguredStagePosition x2 = default(iStage.ConfiguredStagePosition);

            //get currently displayed positions
            Positions[(int)iStage.AxisNameEnum.StageX] = Conversion.Val(txtStageX.Text);
            Positions[(int)iStage.AxisNameEnum.StageY]= Conversion.Val(txtStageY.Text);
            Positions[(int)iStage.AxisNameEnum.StageZ] = Conversion.Val(txtStageZ.Text);

            Positions[(int)iStage.AxisNameEnum.BeamScanX] = Conversion.Val(txtBeamScanX.Text);
            Positions[(int)iStage.AxisNameEnum.BeamScanY] = Conversion.Val(txtBeamScanY.Text);
            Positions[(int)iStage.AxisNameEnum.BeamScanZ] = Conversion.Val(txtBeamScanZ.Text);

            Positions[(int)iStage.AxisNameEnum.AngleMain] = Conversion.Val(txtAngleLens.Text);
            Positions[(int)iStage.AxisNameEnum.AngleHexapod] = Conversion.Val(txtAnglePbs.Text);
            Positions[(int)iStage.AxisNameEnum.PiLS] = Conversion.Val(txtPiLS.Text);

            //get label
            if (NewEntry)
            {
                //get label
                Label = Interaction.InputBox("Please enter the label for this position configuration", "Configured Position");
                Label = Label.Trim();
                if (string.IsNullOrEmpty(Label))
                    return;
                //make sure label is unique
                if (mStageBase.HaveConfiguredPosition(Label))
                {
                    s = "The label [" + Label + "] is already used. Do you want to overwrite the original position?";
                    r = MessageBox.Show(s, "Save Configured Position", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (r ==  DialogResult.No)
                        return;
                }

            }
            else
            {
                //get label
                i = lstConfiguredPositions.SelectedIndex;
                if (i == -1)
                {
                    s = "No current position selected. Please use Save As New to save a new configured position!";
                    MessageBox.Show(s, "Save Configured Position", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Label = ((iStage.ConfiguredStagePosition)lstConfiguredPositions.Items[i]).Label;
            }

            //build a new class
            x = new iStage.ConfiguredStagePosition(Label, Positions);

            //re-ask the question to confirm 
            if (mStageBase.HaveConfiguredPosition(Label))
            {
                s = "We are going to overwrite the configured position for " + Label + ". Do you want to continue?";
                r = MessageBox.Show(s, "Save Configured Position", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (r ==  DialogResult.No)
                    return;
                //update the new value
                mStageBase.UpdateConfiguredPosition(x);
                //update the new value to the list box
                ii = lstConfiguredPositions.Items.Count - 1;
                for (i = 0; i <= ii; i++)
                {
                    x2 = (iStage.ConfiguredStagePosition)lstConfiguredPositions.Items[i];
                    if (x2.Label == x.Label)
                    {
                        lstConfiguredPositions.Items[i] = x;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }
            else
            {
                //add new 
                mStageBase.AddConfiguredPosition(x);
                //add new item to the list
                lstConfiguredPositions.Items.Add(x);
            }

            //commit this to config file
            mStageBase.SaveConfiguredPositions();

        }

        private void DeleteConfiguredPositions()
        {
            string s = null;
            string Label = null;
            int i = 0;
            int ii = 0;
             DialogResult r = default( DialogResult);
            double[] Positions = new double[iStage.AxisCount];
            iStage.ConfiguredStagePosition x = default(iStage.ConfiguredStagePosition);
            iStage.ConfiguredStagePosition x2 = default(iStage.ConfiguredStagePosition);

            i = lstConfiguredPositions.SelectedIndex;
            Label = ((iStage.ConfiguredStagePosition)lstConfiguredPositions.Items[i]).Label;

            s = "We are going to delete the configured position for " + Label + ". Do you want to continue?";
            r = MessageBox.Show(s, "Delete Configured Position", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (r ==  DialogResult.No)
                return;

            lstConfiguredPositions.Items.Remove(i);

            mStageBase.SaveConfiguredPositions();


        }

        public void LoadHexapodConfiguredPositionList()
        {
            string s = null;
            //iStage.ConfiguredStagePosition x = default(iStage.ConfiguredStagePosition);

            //build the configured position list
            lstHexapodConfiguredPositions.SelectionMode = SelectionMode.One;
            lstHexapodConfiguredPositions.Items.Clear();
            foreach (iStage.ConfiguredStagePosition x in mStageBase.HexapodConfiguredPositions.Values)
            {
                //we will not load the safe position to GUI to avoid accidental changes
                s = x.Label.ToLower();
                if (s.Contains("safe"))
                    continue;
                lstHexapodConfiguredPositions.Items.Add(x);
            }
            lstHexapodConfiguredPositions.SelectedIndex = 0;
        }

        private void SaveHexapodConfiguredPositions(bool NewEntry)
        {
            string s = null;
            string Label = null;
            int i = 0;
            int ii = 0;
             DialogResult r = default( DialogResult);
            double[] Positions = new double[iStage.AxisCount];
            iStage.ConfiguredStagePosition x = default(iStage.ConfiguredStagePosition);
            iStage.ConfiguredStagePosition x2 = default(iStage.ConfiguredStagePosition);

            //get currently displayed positions
            Positions[(int)Instrument.iPiGCS.AxisEnum.X] = Conversion.Val(txtHexapodX.Text);
            Positions[(int)Instrument.iPiGCS.AxisEnum.Y] = Conversion.Val(txtHexapodY.Text);
            Positions[(int)Instrument.iPiGCS.AxisEnum.Z] = Conversion.Val(txtHexapodZ.Text);

            Positions[(int)Instrument.iPiGCS.AxisEnum.Rx] = Conversion.Val(txtHexapodU.Text);
            Positions[(int)Instrument.iPiGCS.AxisEnum.Ry] = Conversion.Val(txtHexapodV.Text);
            Positions[(int)Instrument.iPiGCS.AxisEnum.Rz] = Conversion.Val(txtHexapodW.Text);

            //get label
            if (NewEntry)
            {
                //get label
                Label = Interaction.InputBox("Please enter the label for this position configuration", "Configured Position");
                Label = Label.Trim();
                if (string.IsNullOrEmpty(Label))
                    return;
                //make sure label is unique
                if (mStageBase.HaveConfiguredPosition(Label))
                {
                    s = "The label [" + Label + "] is already used. Do you want to overwrite the original position?";
                    r = MessageBox.Show(s, "Save Configured Position", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (r ==  DialogResult.No)
                        return;
                }

            }
            else
            {
                //get label
                i = lstHexapodConfiguredPositions.SelectedIndex;
                if (i == -1)
                {
                    s = "No current position selected. Please use Save As New to save a new configured position!";
                    MessageBox.Show(s, "Save Configured Position", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Label = ((iStage.ConfiguredStagePosition)lstHexapodConfiguredPositions.Items[i]).Label;
            }

            //build a new class
            x = new iStage.ConfiguredStagePosition(Label, Positions);

            //re-ask the question to confirm 
            if (mStageBase.HaveHexapodConfiguredPosition(Label))
            {
                s = "We are going to overwrite the configured position for " + Label + ". Do you want to continue?";
                r = MessageBox.Show(s, "Save Configured Position", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (r ==  DialogResult.No)
                    return;
                //update the new value
                mStageBase.UpdateHexapodConfiguredPosition(x);
                //update the new value to the list box
                ii = lstHexapodConfiguredPositions.Items.Count - 1;
                for (i = 0; i <= ii; i++)
                {
                    x2 = (iStage.ConfiguredStagePosition)lstHexapodConfiguredPositions.Items[i];
                    if (x2.Label == x.Label)
                    {
                        lstHexapodConfiguredPositions.Items[i] = x;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }
            else
            {
                //add new 
                mStageBase.AddHexapodConfiguredPosition(x);
                //add new item to the list
                lstHexapodConfiguredPositions.Items.Add(x);
            }

            //commit this to config file
            mStageBase.SaveHexapodConfiguredPositions();

        }

        #endregion

        #region "Call Back"
        private void btn_Click(System.Object sender, System.EventArgs e)
        {
            Button btn = default(Button);

            btn = (Button)sender;

            //prevent double hits
            btn.Click -= btn_Click;

            //clear stop
            if (btn.Name != btnStopMove.Name)
                mTool.ClearStop();

            switch (btn.Name)
            {
                //---------------------------------------misc 
               
            }

            //sync stage once if stage is moved
            if (btn.Name.Contains("MoveHexapod") | btn.Name.Contains("JogAddHexapod") | btn.Name.Contains("JogSubHexapod"))
            {
                //this.SyncHexapodPositions();
            }
            else if (btn.Name.Contains("Move") | btn.Name.Contains("Jog"))
            {
                this.SyncStagePositions();
            }

            //re-enable event
            btn.Click += btn_Click;
        }

        private void opt_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            RadioButton opt = default(RadioButton);
            bool Checked = false;

            //return if the value is changed internally
            if (mSync)
                return;

            //get handler
            opt = (RadioButton)sender;
            Checked = opt.Checked;

            //we will only process "on"
            if (!Checked)
                return;

            //process event
            switch (opt.Name)
            {

            }
        }

        private void chk_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            CheckBox chk = default(CheckBox);
            bool Checked = false;

            //return if the value is changed internally
            if (mSync)
                return;

            //get handler
            chk = (CheckBox)sender;
            Checked = chk.Checked;

            //process event
            switch (chk.Name)
            {
               
            }

            //done, do a sync
            //this.SyncSignals();
            this.Refresh();
        }

        private void txt_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            TextBox txt = default(TextBox);

            //we are using text box for display only here, just supress any key input
            txt = (TextBox)sender;
            e.SuppressKeyPress = true;
        }

        private void nud_ValueChanged(System.Object sender, System.EventArgs e)
        {
            NumericUpDown nud = default(NumericUpDown);
            decimal value = default(decimal);

            if (mSync)
                return;

            //get handler
            nud = (NumericUpDown)sender;
            value = nud.Value;

            //update few - stage will not be automatically moved. 
            switch (nud.Name)
            {
               
            }

            //this.SyncSignals();
        }

        private void lstConfiguredPositions_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            double v = 0;
            double[] Positions = null;

            if (lstConfiguredPositions.SelectedIndex < 0)
                return;

            //first, let's copy the actual position to the target so that we do not mess up color later
            this.SetStageTargetToCurrentPosition();

            //now load the configured position, those nulled will be left as the actual position we loaded in the first step
            Positions = ((iStage.ConfiguredStagePosition)lstConfiguredPositions.Items[lstConfiguredPositions.SelectedIndex]).Positions;

            v = Positions[(int)iStage.AxisNameEnum.StageX];
            if (!double.IsNaN(v))
                nudStageX.Value = Convert.ToDecimal(v);

        }

        private void lstHexapodConfiguredPositions_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            double v = 0;
            double[] Positions = null;

            if (lstHexapodConfiguredPositions.SelectedIndex < 0)
                return;

            //first, let's copy the actual position to the target so that we do not mess up color later
           

        }

        #endregion

        #region "Config file settings"
        private void SetupSettingGUI()
        {
            List<Control> ctrl = new List<Control>();

            lblSaveSetting.Visible = false;

            w2.w2Misc.GetAllControls<DataGridView>(TabPageSetting, ctrl);

            mDgvConfig = ctrl.ToArray();

            foreach (DataGridView dgv in mDgvConfig)
            {
                dgv.CellValidating += dgv_CellValidating;
                mSettingHelper.SetupTable(dgv);
                mSettingHelper.PopulateTable(dgv);
            }

            dgvAlignment.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;

        }

        private void SaveSettings()
        {
            //wait
            this.Cursor = Cursors.WaitCursor;

            //frist save data to the table and commit
            foreach (DataGridView dgv in mDgvConfig)
            {
                mSettingHelper.SaveTable(dgv);
            }

            //do parameter read again to read the new table into the memory
            mTool.Parameter.ReadParameters();

            //done
            this.Cursor = Cursors.Default;
            lblSaveSetting.Text = "Parameters saved on " + System.DateTime.Now.ToString();
        }

        private void dgv_CellValidating(object sender, System.Windows.Forms.DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex < 0)
                return;
            if (e.RowIndex < 0)
                return;

            DataGridView dgv = (DataGridView)sender;
            if (dgv.EditingControl == null)
                return;
            
            if (dgv.EditingControl.Text == dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
                return;

            lblSaveSetting.Text = "Parameter changed, but not saved!";
            lblSaveSetting.ForeColor = Color.Red;
            lblSaveSetting.Visible = true;
        }
        #endregion
    }
}
