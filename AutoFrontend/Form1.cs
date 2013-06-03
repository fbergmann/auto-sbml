using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;

using LibAutoCSharp;
using LibRoadRunner;
using ZedGraph;
using SBW;

using AutoFrontend.Forms;

namespace AutoFrontend
{

    public class TagData
    {
        public IntTripple Tripple { get; set; }
        public int Series { get; set; }
        public string Label { get; set; }
    }

    public delegate void VoidStringDelegate(string s);

    public delegate void VoidDelegate();

    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class Form1 : System.Windows.Forms.Form
    {
        private AutoResult currentResult = null;
        private Button cmdParseConfig;
        private Button cmdAssignAdvanced;
        private MenuItem menuItem2;
        private MenuItem changeBifurcationPlot;
        private IContainer components;

        public Form1()
        {
            System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CreateSpecificCulture("en");
            culture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = culture;

            InitializeComponent();

            Icon = Properties.Resources.ICON_Auto;

            _SelectForm = new FormSelectVariables();

            AllowDrop = true;

            DragDrop += new DragEventHandler(OnDragDrop);
            DragEnter += new DragEventHandler(OnDragEnter);

            ChangeBifurcationPlot = new FormChangeBifurcationPlot(graphControl1.ZedControl);
            graphControl1.ZedControl.MouseDoubleClick += new MouseEventHandler(GraphDoubleClick);

        }

        private FormChangeBifurcationPlot _ChangeBifurcationPlot;

        public FormChangeBifurcationPlot ChangeBifurcationPlot
        {
            get { return _ChangeBifurcationPlot; }
            set { _ChangeBifurcationPlot = value; }
        }

        void GraphDoubleClick(object sender, MouseEventArgs e)
        {
            object graphObject; int index;

            graphControl1.ZedControl.GraphPane.FindNearestObject(new PointF(e.X, e.Y), graphControl1.ZedControl.CreateGraphics(), out graphObject, out index);
            if (graphObject != null)
            {
                object tag = null;
                if (graphObject is ZedGraph.TextObj)
                    tag = (graphObject as ZedGraph.TextObj).Tag;
                else if (graphObject is ZedGraph.EllipseObj)
                    tag = (graphObject as ZedGraph.EllipseObj).Tag;

                if (tag != null)
                {
                    var tripple = (tag as TagData).Tripple;
                    if (tripple != null)
                    {
                        RunContinuationForTripple(tripple);
                    }
                }
            }
            
        }

        private void OnDragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            try
            {
                string[] sFilenames = (string[])e.Data.GetData(DataFormats.FileDrop);
                FileInfo oInfo = new FileInfo(sFilenames[0]);
                if (oInfo.Extension.ToLower().EndsWith("xml") || oInfo.Extension.ToLower().EndsWith("sbml") || oInfo.Extension.ToLower().EndsWith("jan"))
                {
                    LoadFile(sFilenames[0]);
                }
            }
            catch (Exception)
            {
            }
        }

        private void OnDragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] sFilenames = (string[])e.Data.GetData(DataFormats.FileDrop);
                FileInfo oInfo = new FileInfo(sFilenames[0]);
                if (oInfo.Extension.ToLower().EndsWith("xml") || oInfo.Extension.ToLower().EndsWith("sbml") || oInfo.Extension.ToLower().EndsWith("jan"))
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            LibAutoCSharp.AutoInputConstants autoInputConstants1 = new LibAutoCSharp.AutoInputConstants();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdLoad = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabRun = new System.Windows.Forms.TabPage();
            this.panel7 = new System.Windows.Forms.Panel();
            this.graphControl1 = new AutoFrontend.Controls.GraphControl();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel6 = new System.Windows.Forms.Panel();
            this.setupControl1 = new AutoFrontend.Controls.SetupControl();
            this.tabAdvanced = new System.Windows.Forms.TabPage();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.txtConfig = new AutoFrontend.Controls.CodeTextBox();
            this.panel9 = new System.Windows.Forms.Panel();
            this.cmdAssignAdvanced = new System.Windows.Forms.Button();
            this.cmdParseConfig = new System.Windows.Forms.Button();
            this.chkTakeAsIs = new System.Windows.Forms.CheckBox();
            this.tabJarnac = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.txtJarnac = new AutoFrontend.Controls.CodeTextBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.cmdSendToAuto = new System.Windows.Forms.Button();
            this.tabFort7 = new System.Windows.Forms.TabPage();
            this.txtResult = new AutoFrontend.Controls.CodeTextBox();
            this.tabCode = new System.Windows.Forms.TabPage();
            this.txtCode = new AutoFrontend.Controls.CodeTextBox();
            this.tabSBML = new System.Windows.Forms.TabPage();
            this.txtSBML = new AutoFrontend.Controls.CodeTextBox();
            this.codeTextBox1 = new AutoFrontend.Controls.CodeTextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.mnuOpen = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.mnuExit = new System.Windows.Forms.MenuItem();
            this.oSBWMenu = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.changeBifurcationPlot = new System.Windows.Forms.MenuItem();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabRun.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.tabAdvanced.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel9.SuspendLayout();
            this.tabJarnac.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tabFort7.SuspendLayout();
            this.tabCode.SuspendLayout();
            this.tabSBML.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(144, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdLoad);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 566);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(760, 40);
            this.panel1.TabIndex = 1;
            this.panel1.Visible = false;
            // 
            // cmdLoad
            // 
            this.cmdLoad.Location = new System.Drawing.Point(16, 8);
            this.cmdLoad.Name = "cmdLoad";
            this.cmdLoad.Size = new System.Drawing.Size(75, 23);
            this.cmdLoad.TabIndex = 1;
            this.cmdLoad.Text = "&Load";
            this.cmdLoad.Click += new System.EventHandler(this.cmdLoad_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3);
            this.panel2.Size = new System.Drawing.Size(760, 566);
            this.panel2.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabRun);
            this.tabControl1.Controls.Add(this.tabAdvanced);
            this.tabControl1.Controls.Add(this.tabJarnac);
            this.tabControl1.Controls.Add(this.tabFort7);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(754, 560);
            this.tabControl1.TabIndex = 0;
            // 
            // tabRun
            // 
            this.tabRun.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabRun.Controls.Add(this.panel7);
            this.tabRun.Controls.Add(this.splitter1);
            this.tabRun.Controls.Add(this.panel6);
            this.tabRun.Location = new System.Drawing.Point(4, 22);
            this.tabRun.Name = "tabRun";
            this.tabRun.Padding = new System.Windows.Forms.Padding(3);
            this.tabRun.Size = new System.Drawing.Size(746, 534);
            this.tabRun.TabIndex = 0;
            this.tabRun.Text = "Run AUTO";
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.graphControl1);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(206, 3);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(537, 528);
            this.panel7.TabIndex = 2;
            // 
            // graphControl1
            // 
            this.graphControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphControl1.Location = new System.Drawing.Point(0, 0);
            this.graphControl1.Name = "graphControl1";
            this.graphControl1.Size = new System.Drawing.Size(537, 528);
            this.graphControl1.TabIndex = 0;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(203, 3);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 528);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.setupControl1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel6.Location = new System.Drawing.Point(3, 3);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(200, 528);
            this.panel6.TabIndex = 0;
            // 
            // setupControl1
            // 
            this.setupControl1.CalculateSteadyState = true;
            autoInputConstants1.A0 = 0;
            autoInputConstants1.A1 = 10000;
            autoInputConstants1.DS = 0.001;
            autoInputConstants1.DSMAX = 0.1;
            autoInputConstants1.DSMIN = 1E-05;
            autoInputConstants1.EPSL = 1E-08;
            autoInputConstants1.EPSS = 1E-06;
            autoInputConstants1.EPSU = 1E-08;
            autoInputConstants1.IAD = 3;
            autoInputConstants1.IADS = 1;
            autoInputConstants1.IID = 0;
            autoInputConstants1.ILP = 1;
            autoInputConstants1.IPLT = 0;
            autoInputConstants1.IPS = 1;
            autoInputConstants1.IRS = 0;
            autoInputConstants1.ISP = 1;
            autoInputConstants1.ISW = 1;
            autoInputConstants1.ITMX = 8;
            autoInputConstants1.ITNW = 5;
            autoInputConstants1.JAC = 0;
            autoInputConstants1.MXBF = 1000;
            autoInputConstants1.NBC = 0;
            autoInputConstants1.NCOL = 3;
            autoInputConstants1.NDIM = 2;
            autoInputConstants1.NICP = 1;
            autoInputConstants1.NINT = 0;
            autoInputConstants1.NMX = 1000;
            autoInputConstants1.NPR = 50;
            autoInputConstants1.NTHL = 0;
            autoInputConstants1.NTHU = 0;
            autoInputConstants1.NTST = 15;
            autoInputConstants1.NUZR = 0;
            autoInputConstants1.NWTN = 3;
            autoInputConstants1.RL0 = 0.01;
            autoInputConstants1.RL1 = 30;
            this.setupControl1.CurrentConfig = autoInputConstants1;
            this.setupControl1.DirectionPositive = true;
            this.setupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.setupControl1.EndValue = 30;
            this.setupControl1.Label = 0;
            this.setupControl1.Location = new System.Drawing.Point(0, 0);
            this.setupControl1.MaxBifurcations = -1;
            this.setupControl1.MaxSteps = 1000;
            this.setupControl1.Name = "setupControl1";
            this.setupControl1.Parameter = "";
            this.setupControl1.ParameterAvailable = false;
            this.setupControl1.Parameters = new object[0];
            this.setupControl1.RunContinuation = false;
            this.setupControl1.SimulationEndTime = 10;
            this.setupControl1.SimulationNumPoints = 100;
            this.setupControl1.SimulationStartTime = 0;
            this.setupControl1.Size = new System.Drawing.Size(200, 528);
            this.setupControl1.StartValue = 0.01;
            this.setupControl1.TabIndex = 0;
            // 
            // tabAdvanced
            // 
            this.tabAdvanced.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabAdvanced.Controls.Add(this.panel8);
            this.tabAdvanced.Location = new System.Drawing.Point(4, 22);
            this.tabAdvanced.Name = "tabAdvanced";
            this.tabAdvanced.Padding = new System.Windows.Forms.Padding(3);
            this.tabAdvanced.Size = new System.Drawing.Size(746, 534);
            this.tabAdvanced.TabIndex = 5;
            this.tabAdvanced.Text = "Advanced Configuration";
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.panel10);
            this.panel8.Controls.Add(this.panel9);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(3, 3);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(740, 528);
            this.panel8.TabIndex = 1;
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.txtConfig);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel10.Location = new System.Drawing.Point(0, 32);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(740, 496);
            this.panel10.TabIndex = 1;
            // 
            // txtConfig
            // 
            this.txtConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConfig.Font = new System.Drawing.Font("Courier New", 11F);
            this.txtConfig.Location = new System.Drawing.Point(0, 0);
            this.txtConfig.MaxLength = 0;
            this.txtConfig.Multiline = true;
            this.txtConfig.Name = "txtConfig";
            this.txtConfig.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtConfig.Size = new System.Drawing.Size(740, 496);
            this.txtConfig.TabIndex = 0;
            this.txtConfig.WordWrap = false;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.cmdAssignAdvanced);
            this.panel9.Controls.Add(this.cmdParseConfig);
            this.panel9.Controls.Add(this.chkTakeAsIs);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(740, 32);
            this.panel9.TabIndex = 0;
            // 
            // cmdAssignAdvanced
            // 
            this.cmdAssignAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAssignAdvanced.Location = new System.Drawing.Point(581, 4);
            this.cmdAssignAdvanced.Name = "cmdAssignAdvanced";
            this.cmdAssignAdvanced.Size = new System.Drawing.Size(75, 23);
            this.cmdAssignAdvanced.TabIndex = 2;
            this.cmdAssignAdvanced.Text = "Assign";
            this.cmdAssignAdvanced.UseVisualStyleBackColor = true;
            this.cmdAssignAdvanced.Click += new System.EventHandler(this.cmdAssignAdvanced_Click);
            // 
            // cmdParseConfig
            // 
            this.cmdParseConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdParseConfig.Location = new System.Drawing.Point(662, 4);
            this.cmdParseConfig.Name = "cmdParseConfig";
            this.cmdParseConfig.Size = new System.Drawing.Size(75, 23);
            this.cmdParseConfig.TabIndex = 1;
            this.cmdParseConfig.Text = "Parse";
            this.cmdParseConfig.UseVisualStyleBackColor = true;
            this.cmdParseConfig.Click += new System.EventHandler(this.cmdParseConfig_Click);
            // 
            // chkTakeAsIs
            // 
            this.chkTakeAsIs.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkTakeAsIs.Location = new System.Drawing.Point(8, 4);
            this.chkTakeAsIs.Name = "chkTakeAsIs";
            this.chkTakeAsIs.Size = new System.Drawing.Size(384, 24);
            this.chkTakeAsIs.TabIndex = 0;
            this.chkTakeAsIs.Text = "Use Advanced Configuration as is (don\'t overwrite settings)";
            // 
            // tabJarnac
            // 
            this.tabJarnac.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabJarnac.Controls.Add(this.panel3);
            this.tabJarnac.Location = new System.Drawing.Point(4, 22);
            this.tabJarnac.Name = "tabJarnac";
            this.tabJarnac.Padding = new System.Windows.Forms.Padding(3);
            this.tabJarnac.Size = new System.Drawing.Size(746, 534);
            this.tabJarnac.TabIndex = 1;
            this.tabJarnac.Text = "Model";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(740, 528);
            this.panel3.TabIndex = 1;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.txtJarnac);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(740, 488);
            this.panel5.TabIndex = 1;
            // 
            // txtJarnac
            // 
            this.txtJarnac.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtJarnac.Font = new System.Drawing.Font("Courier New", 11F);
            this.txtJarnac.Location = new System.Drawing.Point(0, 0);
            this.txtJarnac.MaxLength = 0;
            this.txtJarnac.Multiline = true;
            this.txtJarnac.Name = "txtJarnac";
            this.txtJarnac.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtJarnac.Size = new System.Drawing.Size(740, 488);
            this.txtJarnac.TabIndex = 0;
            this.txtJarnac.WordWrap = false;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.cmdSendToAuto);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 488);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(740, 40);
            this.panel4.TabIndex = 0;
            // 
            // cmdSendToAuto
            // 
            this.cmdSendToAuto.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdSendToAuto.Location = new System.Drawing.Point(8, 8);
            this.cmdSendToAuto.Name = "cmdSendToAuto";
            this.cmdSendToAuto.Size = new System.Drawing.Size(104, 23);
            this.cmdSendToAuto.TabIndex = 0;
            this.cmdSendToAuto.Text = "Send to AUTO";
            this.cmdSendToAuto.Click += new System.EventHandler(this.cmdSendToAuto_Click);
            // 
            // tabFort7
            // 
            this.tabFort7.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabFort7.Controls.Add(this.txtResult);
            this.tabFort7.Location = new System.Drawing.Point(4, 22);
            this.tabFort7.Name = "tabFort7";
            this.tabFort7.Padding = new System.Windows.Forms.Padding(3);
            this.tabFort7.Size = new System.Drawing.Size(746, 534);
            this.tabFort7.TabIndex = 2;
            this.tabFort7.Text = "Raw Result";
            // 
            // txtResult
            // 
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.Font = new System.Drawing.Font("Courier New", 11F);
            this.txtResult.Location = new System.Drawing.Point(3, 3);
            this.txtResult.MaxLength = 0;
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResult.Size = new System.Drawing.Size(740, 528);
            this.txtResult.TabIndex = 0;
            this.txtResult.WordWrap = false;
            // 
            // tabCode
            // 
            this.tabCode.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabCode.Controls.Add(this.txtCode);
            this.tabCode.Location = new System.Drawing.Point(4, 22);
            this.tabCode.Name = "tabCode";
            this.tabCode.Padding = new System.Windows.Forms.Padding(3);
            this.tabCode.Size = new System.Drawing.Size(746, 534);
            this.tabCode.TabIndex = 3;
            this.tabCode.Text = "(Debug Code)";
            // 
            // txtCode
            // 
            this.txtCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCode.Font = new System.Drawing.Font("Courier New", 11F);
            this.txtCode.Location = new System.Drawing.Point(3, 3);
            this.txtCode.MaxLength = 0;
            this.txtCode.Multiline = true;
            this.txtCode.Name = "txtCode";
            this.txtCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCode.Size = new System.Drawing.Size(740, 528);
            this.txtCode.TabIndex = 0;
            this.txtCode.WordWrap = false;
            // 
            // tabSBML
            // 
            this.tabSBML.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabSBML.Controls.Add(this.txtSBML);
            this.tabSBML.Location = new System.Drawing.Point(4, 22);
            this.tabSBML.Name = "tabSBML";
            this.tabSBML.Padding = new System.Windows.Forms.Padding(3);
            this.tabSBML.Size = new System.Drawing.Size(746, 534);
            this.tabSBML.TabIndex = 4;
            this.tabSBML.Text = "(Debug SBML)";
            // 
            // txtSBML
            // 
            this.txtSBML.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSBML.Font = new System.Drawing.Font("Courier New", 11F);
            this.txtSBML.Location = new System.Drawing.Point(3, 3);
            this.txtSBML.MaxLength = 0;
            this.txtSBML.Multiline = true;
            this.txtSBML.Name = "txtSBML";
            this.txtSBML.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSBML.Size = new System.Drawing.Size(740, 528);
            this.txtSBML.TabIndex = 0;
            this.txtSBML.WordWrap = false;
            // 
            // codeTextBox1
            // 
            this.codeTextBox1.Font = new System.Drawing.Font("Courier New", 10F);
            this.codeTextBox1.Location = new System.Drawing.Point(416, 104);
            this.codeTextBox1.MaxLength = 0;
            this.codeTextBox1.Multiline = true;
            this.codeTextBox1.Name = "codeTextBox1";
            this.codeTextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.codeTextBox1.Size = new System.Drawing.Size(100, 200);
            this.codeTextBox1.TabIndex = 0;
            this.codeTextBox1.Text = "codeTextBox1";
            this.codeTextBox1.WordWrap = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "xml";
            this.openFileDialog1.Filter = "All supported files (SBML and Jarnac)|*.xml;*.sbml;*.jan|SBML files|*.xml;*.sbml|" +
                "Jarnac files|*.jan|All files|*.*";
            this.openFileDialog1.Title = "Load Model file";
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2,
            this.oSBWMenu});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuOpen,
            this.menuItem3,
            this.mnuExit});
            this.menuItem1.Text = "&File";
            // 
            // mnuOpen
            // 
            this.mnuOpen.Index = 0;
            this.mnuOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.mnuOpen.Text = "&Open";
            this.mnuOpen.Click += new System.EventHandler(this.cmdLoad_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.Text = "-";
            // 
            // mnuExit
            // 
            this.mnuExit.Index = 2;
            this.mnuExit.Shortcut = System.Windows.Forms.Shortcut.CtrlQ;
            this.mnuExit.Text = "E&xit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // oSBWMenu
            // 
            this.oSBWMenu.Index = 2;
            this.oSBWMenu.Text = "&SBW";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.changeBifurcationPlot});
            this.menuItem2.Text = "&Edit";
            // 
            // changeBifurcationPlot
            // 
            this.changeBifurcationPlot.Index = 0;
            this.changeBifurcationPlot.Text = "Change Bifurcation Plot Settings";
            this.changeBifurcationPlot.Click += new System.EventHandler(this.changeBifurcationPlot_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(760, 606);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            try
            {
                this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            }
            catch
            {
            }
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "Auto2000 C# ";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabRun.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.tabAdvanced.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.panel9.ResumeLayout(false);
            this.tabJarnac.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.tabFort7.ResumeLayout(false);
            this.tabFort7.PerformLayout();
            this.tabCode.ResumeLayout(false);
            this.tabCode.PerformLayout();
            this.tabSBML.ResumeLayout(false);
            this.tabSBML.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        #region // SBW Menu
        void oItem_Click(object sender, EventArgs e)
        {
            try
            {
                if ((SBML == null) || SBML == "")
                {
                    MessageBox.Show("There is no model to analyze. Load a model first.", "No Model loaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                SBWAnalyzer oTemp = (SBWAnalyzer)oAnalyzers[((MenuItem)sender).Text];
                SBW.Module oModule = new SBW.Module(oTemp.ModuleName);
                Service oService = oModule.getService(oTemp.ServiceName);
                oService.getMethod("void doAnalysis(string)").Send(SBML);
            }
            catch (SBWException ex)
            {
                MessageBox.Show(String.Format("An error while calling the Analyzer.{0}{0}{0}({1}{0}{1})", Environment.NewLine, ex.Message, ex.DetailedMessage),
                    "Error calling the Analyzer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception)
            {
            }

        }

        private void updateSBWMenu()
        {
            try
            {
                bool bWasConnected = SBW.LowLevel.SBWGetConnectionStatus();
                if (!bWasConnected)
                    SBW.LowLevel.SBWConnect();

                oSBWMenu.MenuItems.Clear();
                oAnalyzers = new Hashtable();
                SBW.Module oBroker = new SBW.Module("BROKER");
                Service oService = oBroker.getService("BROKER");
                Method oMethod = oService.getMethod("{}[] findServices(string,boolean)");
                ArrayList[] oList = oMethod.Call("/Analysis", true).get1DListArray();

                SortedList oModules = new SortedList(CaseInsensitiveComparer.Default);

                foreach (ArrayList list in oList)
                {
                    try
                    {
                        string uniqueName = (string)list[2];
                        if (oModules.Contains(uniqueName))
                            uniqueName += " (" + list[0] + ")";
                        oModules.Add(uniqueName, list);
                    }
                    catch (Exception)
                    {
                    }
                }

                foreach (DictionaryEntry entry in oModules)
                {
                    string sKey = (string)entry.Key;
                    ArrayList list = (ArrayList)entry.Value;
                    if ("AutoCSharp" != (string)list[0])
                    {
                        string uniqueName = (string)list[2];
                        if (oAnalyzers.Contains(uniqueName))
                            uniqueName += " (" + list[0] + ")";
                        MenuItem oItem = new MenuItem(uniqueName);

                        oAnalyzers.Add(uniqueName, new SBWAnalyzer((string)list[0], (string)list[1]));

                        oItem.Click += new EventHandler(oItem_Click);
                        oSBWMenu.MenuItems.Add(oItem);
                    }
                }


                if (!bWasConnected)
                    SBW.LowLevel.SBWDisconnect();
            }
            catch (Exception)
            {
            }
        }

        class SBWAnalyzer
        {
            private string _sModuleName;

            public string ModuleName
            {
                get { return _sModuleName; }
                set { _sModuleName = value; }
            }
            private string _sService;

            public string ServiceName
            {
                get { return _sService; }
                set { _sService = value; }
            }
            public SBWAnalyzer(string sModule, string sService)
            {
                _sModuleName = sModule;
                _sService = sService;
            }

        }

        Hashtable oAnalyzers;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button cmdLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabRun;
        private System.Windows.Forms.TabPage tabJarnac;
        private System.Windows.Forms.TabPage tabFort7;
        private System.Windows.Forms.TabPage tabCode;
        private System.Windows.Forms.TabPage tabSBML;
        private AutoFrontend.Controls.CodeTextBox codeTextBox1;
        private AutoFrontend.Controls.CodeTextBox txtResult;
        private AutoFrontend.Controls.CodeTextBox txtCode;
        private AutoFrontend.Controls.CodeTextBox txtSBML;
        private AutoFrontend.Controls.CodeTextBox txtJarnac;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button cmdSendToAuto;
        private System.Windows.Forms.TabPage tabAdvanced;
        private AutoFrontend.Controls.CodeTextBox txtConfig;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel7;
        private AutoFrontend.Controls.GraphControl graphControl1;
        private AutoFrontend.Controls.SetupControl setupControl1;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem mnuOpen;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem mnuExit;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.CheckBox chkTakeAsIs;
        private System.Windows.Forms.MenuItem oSBWMenu;
        //ContextMenu oSBWMenu = new ContextMenu();

        #endregion

        #region // Oscli Example Hard Coded


        internal static void ExampleAutoSetup()
        {
            FuncCallBack2 oFunc2 = new FuncCallBack2(FunctionCallback2);
            StpntCallBack oStpnt = new StpntCallBack(StpntCallback);
            BcndCallBack oBcnd = new BcndCallBack(BoundaryCallback);

            AutoInterface.SetAutoNumParameters(36);
            AutoInterface.setCallbackFunc2(oFunc2);
            AutoInterface.setCallbackStpnt(oStpnt);
            AutoInterface.setCallbackBcnd(oBcnd);
        }

        static int BoundaryCallback(int ndim, IntPtr par, IntPtr icp, int nbx, IntPtr u0, IntPtr u1, int ijac, IntPtr fb, IntPtr dbc)
        {
            return 0;
        }

        static void FunctionCallback2(IntPtr u, IntPtr par, IntPtr f)
        {
            // pp2 example actually 

            //Console.WriteLine("start function callback");

            double[] oPar = new double[10];
            double[] oU = new double[2];
            double[] oF = new double[2];
            double[] oRate = new double[4];

            Marshal.Copy(par, oPar, 0, 10);
            Marshal.Copy(u, oU, 0, 2);

            double[] oGp = new double[7];
            for (int i = 0; i < 7; i++)
            {
                oGp[i] = oPar[i];
            }

            //double[] oBp = new double[3];
            //for (int i = 0; i < 3; i++)
            //{
            //    oBp[i] = oPar[7 + i];
            //}


            oRate[0] = oGp[0];
            oRate[1] = oGp[1] * oU[0];
            oRate[2] = (oGp[2] * oU[0] - oGp[3] * oU[1]) * (1.0 + oGp[4] * Math.Pow(oU[1], oGp[5]));
            oRate[3] = oGp[6] * oU[1];

            oF[0] = +oRate[0] - oRate[1] - oRate[2];
            oF[1] = +oRate[2] - oRate[3];

            //double e = Math.Exp(-oPar[2] * oU[0]);
            //oF[0] = oPar[1] * oU[0] * (1 - oU[0]) - oU[0] * oU[1] - oPar[0] * (1 - e);
            //oF[1] = -oU[1] + oPar[3] * oU[0] * oU[1];

            //oF = new double[]{ oU[1], oPar[0] } ;
            Marshal.Copy(oF, 0, f, 2);

            //  if (nCount-- < 1)
            //    System.Diagnostics.Debugger.Break();

            //System.Threading.Thread.Sleep(10);
            //Console.WriteLine("leave function callback");            
        }

        protected internal int FunctionCallback(int ndim, IntPtr u, IntPtr icp, IntPtr par, int ijac, IntPtr f, IntPtr dfdu, IntPtr dfdp)
        {
            // pp2 example actually 

            //Console.WriteLine("start function callback");

            double[] oPar = new double[10];
            double[] oU = new double[ndim];
            double[] oF = new double[ndim];
            double[] oRate = new double[4];

            Marshal.Copy(par, oPar, 0, 10);
            Marshal.Copy(u, oU, 0, ndim);

            double[] oGp = new double[7];
            for (int i = 0; i < 7; i++)
            {
                oGp[i] = oPar[i];
            }

            //double[] oBp = new double[3];
            //for (int i = 0; i < 3; i++)
            //{
            //    oBp[i] = oPar[7 + i];
            //}


            oRate[0] = oGp[0];
            oRate[1] = oGp[1] * oU[0];
            oRate[2] = (oGp[2] * oU[0] - oGp[3] * oU[1]) * (1.0 + oGp[4] * Math.Pow(oU[1], oGp[5]));
            oRate[3] = oGp[6] * oU[1];

            oF[0] = +oRate[0] - oRate[1] - oRate[2];
            oF[1] = +oRate[2] - oRate[3];

            //double e = Math.Exp(-oPar[2] * oU[0]);
            //oF[0] = oPar[1] * oU[0] * (1 - oU[0]) - oU[0] * oU[1] - oPar[0] * (1 - e);
            //oF[1] = -oU[1] + oPar[3] * oU[0] * oU[1];

            //oF = new double[]{ oU[1], oPar[0] } ;
            Marshal.Copy(oF, 0, f, 2);

            //  if (nCount-- < 1)
            //    System.Diagnostics.Debugger.Break();

            //System.Threading.Thread.Sleep(10);
            //Console.WriteLine("leave function callback");
            return 0;
        }

        static int StpntCallback(int ndim, double t, IntPtr u, IntPtr par)
        {
            //double[] oPar = new double[] { -.9, .5, -.6, .6, .328578, .933578}; 
            //double[] oU = new double[] {0.0, 0.0, 0.0};

            //Marshal.Copy(oPar, 0, par, 6);
            //Marshal.Copy(oU, 0, u, 3);


            // pp2 example

            //double[] oPar = new double[] { 0.0, 3.0, 5.0, 3.0}; 
            //double[] oU = new double[] {0.0, 0.0};

            double[] oPar = new double[] { 1.0, 0.0, 1.0, 0.0, 1.0, 3.0, 5.0, 1.0, 0.0, 0.0 };
            double[] oU = new double[] { 0.992063492063492, 0.2 };


            Marshal.Copy(oPar, 0, par, 10);
            Marshal.Copy(oU, 0, u, oU.Length);

            return 0;
        }
        #endregion

        #region // Callback Functions for Current SBML model

        static StpntCallBack _InitializationCallBack;
        static FuncCallBack2 _FunctionCallBack;
        //static BcndCallBack _BCNDCallBack;
        //static IcndCallBack _IcndCallBack;
        //static FoptCallBack _FoptCallBack;
        //static PvlsCallBack _PvlsCallBack; 

        internal void SetupUsingModel(IModel oCurrentModel)
        {
            if (oCurrentModel == null)
                throw new ArgumentNullException("oCurrentModel", "Need to load SBML file first.");

            setupControl1.CurrentConfig.NDIM = oCurrentModel.y.Length;

            _InitializationCallBack = new StpntCallBack(ModelInitializationCallback);
            _FunctionCallBack = new FuncCallBack2(ModelFunctionCallback);
            //_BCNDCallBack = new BcndCallBack(ModelBcndCallBack);
            //_IcndCallBack = new IcndCallBack(ModelIcndCallBack);
            //_FoptCallBack = new FoptCallBack(ModelFoptCallBack);
            //_PvlsCallBack = new PvlsCallBack(ModelPvlsCallBack);


            AutoInterface.setCallbackStpnt(_InitializationCallBack);
            AutoInterface.setCallbackFunc2(_FunctionCallBack);
            //AutoInterface.setCallbackBcnd(_BCNDCallBack);
            //AutoInterface.setCallbackIcnd(_IcndCallBack);
            //AutoInterface.setCallbackFopt(_FoptCallBack);
            //AutoInterface.setCallbackPvls(_PvlsCallBack);

        }

        int nDim = 0;

        int ModelInitializationCallback(int ndim, double t, IntPtr u, IntPtr par)
        {

            nDim = ndim;

            int numBoundaries = _SelectForm.NumSelectedBoundaries;
            int numParameters = _SelectForm.NumSelectedParameters;

            double[] oBoundary = new double[numBoundaries];
            double[] oGlobalParameters = new double[numParameters];

            if (numBoundaries > 0)
            {
                int[] oSelectedBoundary = _SelectForm.SelectedBoundarySpecies;
                for (int i = 0; i < numBoundaries; i++)
                {
                    oBoundary[i] = Simulator.getBoundarySpeciesByIndex(oSelectedBoundary[i]);
                }
            }


            if (numParameters > 0)
            {
                int[] oSelectedParameters = _SelectForm.SelectedParameters;
                for (int i = 0; i < numParameters; i++)
                {
                    oGlobalParameters[i] = Simulator.getGlobalParameterByIndex(oSelectedParameters[i]);
                }
            }

            double[] oParameters = new double[numBoundaries + numParameters];

            Array.Copy(oBoundary, oParameters, oBoundary.Length);
            Array.Copy(oGlobalParameters, 0, oParameters, oBoundary.Length, oGlobalParameters.Length);

            Marshal.Copy(oParameters, 0, par, oParameters.Length);

            Marshal.Copy(CurrentModel.y, 0, u, Math.Min(CurrentModel.y.Length, ndim));

            return 0;
        }

        private static void PrintArray(double[] p, TextWriter textWriter)
        {
            for (int i = 0; i < p.Length; i++)
            {
                textWriter.Write(p[i]);
                if (i < p.Length - 1)
                    textWriter.Write(", ");
            }
            textWriter.WriteLine();
        }


        int ModelBcndCallBack(int ndim, IntPtr par, IntPtr icp, int nbx, IntPtr u0, IntPtr u1, int ijac, IntPtr fb, IntPtr dbc)
        {
            return 0;
        }

        int ModelIcndCallBack(int ndim, IntPtr par, IntPtr icp, int nint, IntPtr u, IntPtr uold, IntPtr udot, IntPtr upold, int ijac, IntPtr fi, IntPtr dint)
        {
            return 0;
        }

        public int ModelFoptCallBack(int ndim, IntPtr u, IntPtr icp, IntPtr par, int ijac, IntPtr fs, IntPtr dfdu, IntPtr dfdp)
        {
            return 0;
        }

        public int ModelPvlsCallBack(int ndim, IntPtr u, IntPtr par)
        {
            return 0;
        }

        void ModelFunctionCallback(IntPtr oVariables, IntPtr par, IntPtr oResult)
        {

            int numBoundaries = _SelectForm.NumSelectedBoundaries;
            int numParameters = _SelectForm.NumSelectedParameters;

            if (numBoundaries > 0)
            {
                double[] oBoundary = new double[numBoundaries];
                Marshal.Copy(par, oBoundary, 0, numBoundaries);
                int[] oSelectedBoundary = _SelectForm.SelectedBoundarySpecies;
                for (int i = 0; i < numBoundaries; i++)
                {
                    Simulator.setBoundarySpeciesByIndex(oSelectedBoundary[i], (double.IsNaN(oBoundary[i]) ? oSelectedBoundary[i] : oBoundary[i]));
                }
            }

            if (numParameters > 0)
            {
                double[] oParameters = new double[numParameters];
                Marshal.Copy(par, oParameters, numBoundaries, numParameters);
                int[] oSelectedParameters = _SelectForm.SelectedParameters;
                for (int i = 0; i < numParameters; i++)
                {
                    Simulator.setGlobalParameterByIndex(oSelectedParameters[i], (double.IsNaN(oParameters[i]) ? oSelectedParameters[i] : oParameters[i]));
                }
            }


            double[] variableTemp = new double[CurrentModel.y.Length];
            Marshal.Copy(oVariables, variableTemp, 0, Math.Min(CurrentModel.y.Length, nDim));

            bool containsNaN = ContainsNaN(variableTemp);
            if (!containsNaN)
            {
                CurrentModel.y = variableTemp;
            }

            //Console.WriteLine("Eval");
            //PrintArray(CurrentModel.y, Console.Out);

            CurrentModel.convertToAmounts();
            CurrentModel.evalModel(CurrentModel.time, CurrentModel.y);
            //Simulator.oneStep(0.0, 0.01);

            Marshal.Copy(CurrentModel.dydt, 0, oResult, Math.Min(CurrentModel.dydt.Length, nDim));

            
            //PrintArray(CurrentModel.y, Console.Out);
            //PrintArray(CurrentModel.dydt, Console.Out);

        }

        private bool ContainsNaN(double[] variableTemp)
        {
            for (int i = 0; i < variableTemp.Length; i++)
            {
                if (double.IsNaN(variableTemp[i]))
                    return true;
            }
            return false;
        }
        #endregion

        internal static FormSelectVariables _SelectForm = null;

        public AutoResult CurrentResult
        {
            get
            {
                return currentResult;
            }
            set
            {
                currentResult = value;
            }
        }
        
        public static FormSelectVariables SelectForm
        {
            get
            {
                return _SelectForm;
            }
            set
            {
                _SelectForm = value;
            }
        }

        static RoadRunner _Simulator = new RoadRunner();

        public static RoadRunner Simulator
        {
            get
            {
                return _Simulator;
            }
            set
            {
                _Simulator = value;
            }
        }

        private IModel _CurrentModel = null;

        public IModel CurrentModel
        {
            get { return _CurrentModel; }
            set { _CurrentModel = value; }
        }        

        private void ResetSetupControl()
        {
            setupControl1.MaxSteps = 1000;
            setupControl1.StartValue = 0.01;
            setupControl1.EndValue = 30;
            setupControl1.DirectionPositive = true;
        }

        public void doAnalysis(string sSBML)
        {

            this.Invoke(new VoidDelegate(ResetSetupControl));
            
            loadSBML(sSBML);
        }

        private static string _SBML;

        internal static string SBML
        {
            get { return _SBML; }
            set { _SBML = value; }
        }

        static ModelState _InitialState = null;

        public static ModelState InitialState
        {
            get
            {
                return _InitialState;
            }
            set
            {
                _InitialState = value;
            }
        }

        private void SetTitle(string fileName)
        {
            this.Text = "Auto2000 C# ";
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                this.Text += " - [" + Path.GetFileName(fileName) + "]";
            }
        }


        public void loadSBML(string sSBML)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new VoidStringDelegate(loadSBML), new object[] { sSBML });
            }
            else
            {
                try
                {
                    if (sSBML.Contains("jd:") && !sSBML.Contains("xmlns:jd = ")) 
                    {
                        sSBML = sSBML.Replace("xmlns:jd2", "xmlns:jd = \"http://www.sys-bio.org/sbml\" xmlns:jd2");
                    }
                    chkTakeAsIs.Checked = false;
                    Simulator.model = null;
                    Simulator.modelLoaded = false;
                    Simulator.loadSBML(sSBML);
                    CurrentModel = Simulator.model;

                    _InitialState = new ModelState(CurrentModel);

                    txtCode.Text = Simulator.getCSharpCode();

                    _SelectForm.InitializeFromModel(CurrentModel);

                    ArrayList oList = new ArrayList(new string[] { "Time" });
                    oList.AddRange(Simulator.getFloatingSpeciesNames());

                    FormChangeTimeCoursePlot.Instance.XAxisValues = oList.ToArray();

                    if (CurrentModel != null)
                    {
                        setupControl1.Parameters = _SelectForm.Parameters.ToArray();
                    }
                    else
                    {
                        setupControl1.ParameterAvailable = false;
                    }


                    //_SelectForm.Show();
                    //_SelectForm.Focus();

                    SBML = sSBML;

                    txtSBML.Text = SBML;
                    txtJarnac.Text = Util.ConvertToJarnac(sSBML);

                    SetTitle("SBML.xml");
                }
                catch (SBW.SBWException ex)
                {
                    MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine + ex.DetailedMessage, "Loading the model failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadFile(string sFileName)
        {
            var oReader = new StreamReader(sFileName);
            string sSBML = oReader.ReadToEnd();
            oReader.Close();

            string fileExtension = new FileInfo(sFileName).Extension.ToUpper();
            if (fileExtension == ".JAN" || fileExtension == "JAN")
                sSBML = Util.ConvertToSBML(sSBML);



            setupControl1.MaxSteps = 1000;
            setupControl1.StartValue = 0.01;
            setupControl1.EndValue = 30;
            setupControl1.DirectionPositive = true;

            ResetAuto();

            loadSBML(sSBML);

            SetTitle(sFileName);

        }

        private string GetNthName(int i)
        {
            try
            {
                string sLabel = "var" + i;
                if (CurrentModel != null && i < Simulator.getNumberOfFloatingSpecies())
                {
                    sLabel = (string)Simulator.getFloatingSpeciesNames()[i];
                }
                return sLabel;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private void SetParameterAxisTitle()
        {
            if (CurrentModel != null)
            {
                graphControl1.ZedControl.GraphPane.XAxis.Title.IsVisible = true;
                if (_SelectForm.NumSelectedParameters > 0)
                    graphControl1.ZedControl.GraphPane.XAxis.Title.Text = (string)Simulator.getGlobalParameterNames()[_SelectForm.SelectedParameters[0]];
                else if (_SelectForm.NumSelectedBoundaries > 0)
                    graphControl1.ZedControl.GraphPane.XAxis.Title.Text = (string)Simulator.getBoundarySpeciesNames()[_SelectForm.SelectedBoundarySpecies[0]];
                else
                    graphControl1.ZedControl.GraphPane.XAxis.Title.IsVisible = false;


            }
            else
            {
                graphControl1.ZedControl.GraphPane.XAxis.Title.IsVisible = false;
            }
        }
        
        private void AddNthSegmentedCurveToGraph(int i, ArrayList oThickSegments, ArrayList series, string sLabel, System.Drawing.Color oLineColor)
        {
            for (int k = 0; k < oThickSegments.Count; k++)
            {
                LineItem oThickItem = graphControl1.ZedControl.GraphPane.AddCurve(sLabel, (PointPairList)oThickSegments[k], oLineColor, SymbolType.None);
                oThickItem.Line.Width = 4f;
                oThickItem.Line.IsSmooth = true;
                oThickItem.Label.IsVisible = false;                
            }

            LineItem oThinItem = graphControl1.ZedControl.GraphPane.AddCurve(sLabel, (double[])series[0], (double[])series[i + 1], oLineColor, SymbolType.None);
            oThinItem.Line.Width = 1f;
            oThinItem.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            oThinItem.Line.IsSmooth = true;
        }

        private void AddNthCurveToGraph(int i, ArrayList series, string sLabel, System.Drawing.Color oLineColor)
        {
            LineItem oItem = graphControl1.ZedControl.GraphPane.AddCurve(sLabel, (double[])series[0], (double[])series[1 + i], oLineColor, SymbolType.None);
            oItem.Line.Width = 2f;
            oItem.Line.IsSmooth = true;
        }

        private static ArrayList FindNthThickSegments(int i, AutoResult oResult, ArrayList series)
        {
            ArrayList oThickSegments = new ArrayList();

            int nLast = 0;
            int nLastDir = -1;
            for (int j = 0; j < oResult.Positions.Count; j++)
            {

                IntPair oPos = oResult.Positions[j];
                //int nCurrentSliceLength = oPos.Key - nLast;
                int nCurrentSliceLength = oPos.Key -1 - nLast;
                double[] oXSlice = new double[nCurrentSliceLength];
                double[] oYSlice = new double[nCurrentSliceLength];

                Array.Copy((double[])series[0], nLast, oXSlice, 0, nCurrentSliceLength);
                Array.Copy((double[])series[i + 1], nLast, oYSlice, 0, nCurrentSliceLength);
                //System.Diagnostics.Debug.WriteLine("from: " + nLast +" to: " + (nLast + nCurrentSliceLength));
                nLastDir = oPos.Value;

                if (nLastDir > 0)
                {
                    oThickSegments.Add(new PointPairList(oXSlice, oYSlice));
                }

                nLast = oPos.Key ;

            }

            if (nLast < ((double[])series[0]).Length)
            {
                int nLastSliceLength = ((double[])series[0]).Length - nLast;

                double[] oXSlice = new double[nLastSliceLength];
                double[] oYSlice = new double[nLastSliceLength];

                Array.Copy((double[])series[0], nLast, oXSlice, 0, nLastSliceLength);
                Array.Copy((double[])series[i + 1], nLast, oYSlice, 0, nLastSliceLength);
                //System.Diagnostics.Debug.WriteLine("from: " + nLast + " to: " + (nLast + nLastSliceLength));

                if (nLastDir * -1 > 0)
                {
                    oThickSegments.Add(new PointPairList(oXSlice, oYSlice));
                }
            }
            return oThickSegments;
        }

        private void PlotResults(AutoResult oResult)
        {
            graphControl1.ZedControl.GraphPane.CurveList.Clear();
            ColorSymbolRotator oRot = new ColorSymbolRotator();

            foreach (AutoResultRun run in oResult.AllRuns)
            {

                ArrayList series = run.DataSeries;

                if (series.Count < 2) continue;


                if ((series[0] as double[]).Length < 10) continue;

                for (int i = 0; i < series.Count - 1; i++)
                {
                    string sLabel = GetNthName(i);
                    System.Drawing.Color oLineColor = oRot.NextColor;

                    if (oResult.Positions.Count == 0)
                    {
                        AddNthCurveToGraph(i, series, sLabel, oLineColor);
                    }
                    else
                    {
                        ArrayList oThickSegments = FindNthThickSegments(i, oResult, series);
                        AddNthSegmentedCurveToGraph(i, oThickSegments, series, sLabel, oLineColor);
                    }
                }


                foreach (IntTripple tripple in oResult.Labels)
                {

                    for (int i = 0; i < series.Count - 1; i++)
                    {
                        int currentPos = tripple.Key;

                        currentPos = UtilLib.FindClosestStablePoint(tripple.Key, oResult.Positions);

                        double x = ((double[])series[0])[currentPos];
                        double y = ((double[])series[1 + i])[currentPos];
                        ZedGraph.TextObj text = new TextObj(UtilLib.ConvertIntTypeToShortString(tripple.Value2), x, y);
                        text.Tag = new TagData { Series = 1 + i, Tripple = tripple, Label = GetNthName(i)};

                        graphControl1.ZedControl.GraphPane.GraphObjList.Add(text);
                    }

                }

            }

            graphControl1.ZedControl.GraphPane.XAxis.Scale.MaxGrace = 0f;
            graphControl1.ZedControl.GraphPane.XAxis.Scale.MinGrace = 0f;
            graphControl1.ZedControl.GraphPane.YAxis.Scale.MaxGrace = 0f;
            graphControl1.ZedControl.GraphPane.YAxis.Scale.MinGrace = 0f;

            SetParameterAxisTitle();

            graphControl1.ZedControl.AxisChange();
            tabControl1.Refresh();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {

            LoadFile("d:/Frank/SBML Models/BorisEjb.xml");
            _SelectForm.InitializeFromModel(CurrentModel);
            _SelectForm.ShowDialog();
        }

        private void cmdLoad_Click(object sender, System.EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadFile(openFileDialog1.FileName);
            }

        }

        private void cmdSendToAuto_Click(object sender, System.EventArgs e)
        {
            try
            {
                // selected value
                string sTemp = setupControl1.Parameter;
                loadSBML(Util.ConvertToSBML(txtJarnac.Text));
                if (setupControl1.Contains(sTemp))
                    setupControl1.Parameter = sTemp;
            }
            catch (SBW.SBWException ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine + ex.DetailedMessage, "Loading the model failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
            }
        }

        internal static Form1 _Instance = null;

        internal static Form1 Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Form1();
                return _Instance;
            }
        }

        internal void OpenSBML()
        {
            cmdLoad_Click(this, EventArgs.Empty);
        }

        private void RunContinuationForTripple(IntTripple tripple)
        {
            try
            {
                if (SBML == null || SBML.Length == 0)
                    return;

                string fort8 = AutoResult.NewFort8;//.Replace(" 7 ", " 12 ");
                //string fort7 = AutoResult.NewFort7;

                ResetAuto();

                setupControl1.RunContinuation = true;
                setupControl1.Label = tripple.Value1;

                string originalConfig = setupControl1.CurrentConfig.ToInputString();

                //setupControl1.CurrentConfig.IPS = 2;                
                setupControl1.CurrentConfig.IRS = tripple.Value1;
                //setupControl1.CurrentConfig.ILP = 1;

                setupControl1.CurrentConfig.NICP = 2;
                setupControl1.CurrentConfig.ICP.Clear();
                setupControl1.CurrentConfig.ICP.Add(0);
                setupControl1.CurrentConfig.ICP.Add(10);

                //setupControl1.CurrentConfig.NCOL = 4;
                //setupControl1.CurrentConfig.IAD = 3;
                //setupControl1.CurrentConfig.ISP = 1;
                //setupControl1.CurrentConfig.ISW = 1;

                setupControl1.CurrentConfig.MXBF = 10;
                                               
                setupControl1.CurrentConfig.NTHL = 1;
                setupControl1.CurrentConfig.THL.Add(new IntDoublePair(10, 0));
                
                txtConfig.Text = setupControl1.Configuration;
                
                SetupAuto();                
                
                AutoInterface.setFort2File(txtConfig.Text, txtConfig.Text.Length);
                
                AutoInterface.setFort3File(fort8, fort8.Length);

                if (setupControl1.CurrentConfig.IPS != 1)
                {
                    if (MessageBox.Show("Currently only IPS = 1 is supported, any other value will make the library unstable. It is recommendet that you Quit (yes).", "Unsupported Values", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        return;
                    }
                }


                AutoInterface.CallAuto();
                string lastMessage = AutoInterface.GetLastMessage();

                //fort7 = fort7 + AutoResult.NewFort7;
                //AutoInterface.setFort7File(fort7, fort7.Length);

                fort8 = fort8 + AutoResult.NewFort8;
                AutoInterface.setFort8File(fort8, fort8.Length);

                CurrentResult = new AutoResult();
                txtResult.Text = CurrentResult.Fort7;

                RemoveWartermark();
                if (CurrentResult.ErrorOccured && CurrentResult.NumPoints < 10)
                {
                    AddWaterMark("Auto\ndid not\nreturn\nresults.");
                }
                else if (CurrentResult.NumPoints < 10)
                {
                    AddWaterMark("Not\nenough\npoints\nreturned.");
                }


                setupControl1.CurrentConfig = LibAutoCSharp.AutoInputConstants.FromContent(originalConfig);


                if (!string.IsNullOrEmpty(lastMessage))
                {
                    RemoveWartermark();
                    AddWaterMark(lastMessage);
                    return;
                }


                PlotResults(CurrentResult);



                if (setupControl1.ReloadAfterRun)
                {
                    try
                    {
                        //Simulator.loadSBML(SBML);
                        cmdSendToAuto_Click(this, EventArgs.Empty);
                    }
                    catch (Exception)
                    {
                        //
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An Error occured while running Auto", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetAuto()
        {
            string[] files = Directory.GetFiles(UtilLib.GetTempPath(), "fort.*");
            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception)
                {
                    
                }
            }            
        }

        internal void RunAuto()
        {
            Cursor originalCursor = this.Cursor;
            try
            {

                this.Cursor = Cursors.WaitCursor;

                if (SBML == null || SBML.Length == 0)
                    OpenSBML();

                ResetAuto();
                setupControl1.RunContinuation = false;
                SetupAuto();



                AutoInterface.setFort2File(txtConfig.Text, txtConfig.Text.Length);

                if (setupControl1.CurrentConfig.IPS != 1)
                {
                    if (MessageBox.Show("Currently only IPS = 1 is supported, any other value will make the library unstable. It is recommendet that you Quit (yes).", "Unsupported Values", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        return;
                    }
                }


                AutoInterface.CallAuto();

                CurrentResult = new AutoResult();
                txtResult.Text = CurrentResult.Fort7;

                RemoveWartermark();
                if (CurrentResult.ErrorOccured)
                {
                    AddWaterMark("Auto\ndid not\nreturn\nresults.");
                }
                else if (CurrentResult.NumPoints < 10)
                {
                    AddWaterMark("Not\nenough\npoints\nreturned.");
                }

                PlotResults(CurrentResult);



                //if (setupControl1.ReloadAfterRun)
                //{
                //    try
                //    {
                //        //Simulator.loadSBML(SBML);
                //        cmdSendToAuto_Click(this, EventArgs.Empty);
                //    }
                //    catch (Exception)
                //    {
                //        //
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An Error occured while running Auto", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = originalCursor;
            }
        }

        private void SetupAuto()
        {
            if (CurrentModel == null)
                ExampleAutoSetup();
            else
            {
                //InitialState.AssignToModel(Simulator.model);
                //Simulator.loadSBML(SBML);
                Simulator.reset();
                

                if (setupControl1.DirectionPositive)
                    Simulator.setValue(setupControl1.Parameter, setupControl1.StartValue);
                else
                    Simulator.setValue(setupControl1.Parameter, setupControl1.EndValue);

                try
                {
                    setupControl1.CalculateSteadyStateIfNecessary();
                }
                catch
                {
                    // maybe being at steady stat is not all that counts
                }
                if (!chkTakeAsIs.Checked)
                    txtConfig.Text = setupControl1.Configuration;
                SetupUsingModel(CurrentModel);
            }

            _SelectForm.Parameters = new ArrayList(new object[] { setupControl1.Parameter });            
        }

        public void RemoveWartermark()
        {
            graphControl1.ZedControl.GraphPane.GraphObjList.Clear();
        }

        private GraphObj GetWaterMark(string sMessage)
        {
            TextObj text = new TextObj(sMessage, 0.5F, 0.5F);
            text.Location.CoordinateFrame = CoordType.PaneFraction;
            text.FontSpec.Angle = 30.0F;
            text.FontSpec.FontColor = Color.FromArgb(70, 255, 100, 100);
            text.FontSpec.IsBold = true;
            text.FontSpec.Size = 100;
            text.FontSpec.Border.IsVisible = false;
            text.FontSpec.Fill.IsVisible = false;
            text.Location.AlignH = AlignH.Center;
            text.Location.AlignV = AlignV.Center;
            text.ZOrder = ZOrder.A_InFront;
            return text;


        }

        public void AddWaterMark(string sMessage)
        {
            graphControl1.ZedControl.GraphPane.GraphObjList.Add(GetWaterMark(sMessage));
        }

        private void mnuExit_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            updateSBWMenu();


            //HACK: Add this for debug
            //this.tabControl1.Controls.Add(this.tabCode);
            //this.tabControl1.Controls.Add(this.tabSBML);

        }
        
        public Controls.SetupControl SetupControl
        {
            get { return setupControl1; }            
        }

        private void cmdParseConfig_Click(object sender, EventArgs e)
        {
            try
            {
                AutoInputConstants constants = AutoInputConstants.FromContent(txtConfig.Text);
                txtConfig.Text = constants.ToInputString();
            }
            catch (Exception)
            {                                
            }
        }

        private void cmdAssignAdvanced_Click(object sender, EventArgs e)
        {
            try
            {
                AutoInputConstants constants = AutoInputConstants.FromContent(txtConfig.Text);
                txtConfig.Text = constants.ToInputString();

                setupControl1.CurrentConfig = constants;

                setupControl1.MaxSteps = constants.NMX;
                setupControl1.MaxBifurcations = constants.MXBF;
                setupControl1.StartValue = constants.RL0;
                setupControl1.EndValue = constants.RL1;

                setupControl1.DirectionPositive = (constants.DS >= 0);                
            }
            catch (Exception)
            {
                
            }
        }

        private void changeBifurcationPlot_Click(object sender, EventArgs e)
        {
            ChangeBifurcationPlot.StartPosition = FormStartPosition.CenterParent;
            ChangeBifurcationPlot.Show();
        }

    }
}
