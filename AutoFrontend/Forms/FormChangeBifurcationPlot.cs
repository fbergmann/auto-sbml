using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AutoFrontend.Forms
{
    public partial class FormChangeBifurcationPlot : Form
    {
        public FormChangeBifurcationPlot(ZedGraph.ZedGraphControl control)
        {
            InitializeComponent();
            _ZedControl = control;

            chkXAuto.CheckedChanged += new EventHandler(chkXAuto_CheckedChanged);
            chkYAuto.CheckedChanged += new EventHandler(chkYAuto_CheckedChanged);
        }

        void chkYAuto_CheckedChanged(object sender, EventArgs e)
        {
            txtMaxY.Enabled = !chkYAuto.Checked;
            txtMinY.Enabled = !chkYAuto.Checked;
        }

        void chkXAuto_CheckedChanged(object sender, EventArgs e)
        {
            txtMaxX.Enabled = !chkXAuto.Checked;
            txtMinX.Enabled = !chkXAuto.Checked;
        }


        private ZedGraph.ZedGraphControl _ZedControl;
        public ZedGraph.ZedGraphControl ZedControl
        {
            get
            {
                return _ZedControl;
            }
            set
            {
                _ZedControl = value;
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            cmdApply_Click(sender, e);
            this.Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            ApplyChangesToGraphControl(ZedControl);
        }


        private void FormChangeBifurcationPlot_Load(object sender, EventArgs e)
        {
            InitializeFromControl(ZedControl);
        }

        private void InitializeFromControl(ZedGraph.ZedGraphControl graphControl)
        {
            chkXAuto.Checked = graphControl.GraphPane.XAxis.Scale.MaxAuto & graphControl.GraphPane.XAxis.Scale.MinAuto; 
            chkYAuto.Checked = graphControl.GraphPane.YAxis.Scale.MaxAuto & graphControl.GraphPane.YAxis.Scale.MinAuto;

            chkLogX.Checked = (graphControl.GraphPane.XAxis.Scale.IsLog);
            chkLogY.Checked = (graphControl.GraphPane.YAxis.Scale.IsLog);

            chkAntiAlias.Checked = graphControl.IsAntiAlias;

            txtMinX.Text = graphControl.GraphPane.XAxis.Scale.Min.ToString();
            txtMaxX.Text = graphControl.GraphPane.XAxis.Scale.Max.ToString();

            txtMinY.Text = graphControl.GraphPane.YAxis.Scale.Min.ToString();
            txtMaxY.Text = graphControl.GraphPane.YAxis.Scale.Max.ToString();

        }

        private void ApplyChangesToGraphControl(ZedGraph.ZedGraphControl graphControl)
        {
            if (chkLogX.Checked)
                graphControl.GraphPane.XAxis.Type = ZedGraph.AxisType.Log;
            else
                graphControl.GraphPane.XAxis.Type = ZedGraph.AxisType.Linear;


            if (chkLogY.Checked)
                graphControl.GraphPane.YAxis.Type = ZedGraph.AxisType.Log;
            else
                graphControl.GraphPane.YAxis.Type = ZedGraph.AxisType.Linear;


            graphControl.IsAntiAlias = chkAntiAlias.Checked;

            if (chkXAuto.Checked)
            {
                graphControl.GraphPane.XAxis.Scale.MaxAuto = chkXAuto.Checked;
                graphControl.GraphPane.XAxis.Scale.MinAuto = chkXAuto.Checked; 
            }
            else
            {
                graphControl.GraphPane.XAxis.Scale.Min = Util.ConvertToDouble(txtMinX.Text, graphControl.GraphPane.XAxis.Scale.Min);
                graphControl.GraphPane.XAxis.Scale.Max = Util.ConvertToDouble(txtMaxX.Text, graphControl.GraphPane.XAxis.Scale.Max);
            }

            if (chkYAuto.Checked)
            {
                graphControl.GraphPane.YAxis.Scale.MaxAuto = chkYAuto.Checked;
                graphControl.GraphPane.YAxis.Scale.MinAuto = chkYAuto.Checked;
            }
            else
            {
                graphControl.GraphPane.YAxis.Scale.Min = Util.ConvertToDouble(txtMinY.Text, graphControl.GraphPane.YAxis.Scale.Min);
                graphControl.GraphPane.YAxis.Scale.Max = Util.ConvertToDouble(txtMaxY.Text, graphControl.GraphPane.YAxis.Scale.Max);
            }

            graphControl.AxisChange();
            graphControl.Refresh();
        }

        private void FormChangeBifurcationPlot_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void FormChangeBifurcationPlot_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                InitializeFromControl(ZedControl);
        }

    }
}
