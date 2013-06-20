using System;
using System.Collections.Generic;
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

    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public partial class MainForm : System.Windows.Forms.Form
    {
        private Button cmdParseConfig;
        private Button cmdAssignAdvanced;
        private MenuItem menuItem2;
        private MenuItem changeBifurcationPlot;
        private IContainer components;

        public MainForm()
        {
            CurrentModel = null;
            CurrentResult = null;
            System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CreateSpecificCulture("en");
            culture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = culture;

            InitializeComponent();

            Icon = Properties.Resources.ICON_Auto;

            SelectForm = new FormSelectVariables();

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

            if (oAnalyzers.Count == 0)
                oSBWMenu.Visible = false;

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

            int numBoundaries = SelectForm.NumSelectedBoundaries;
            int numParameters = SelectForm.NumSelectedParameters;

            double[] oBoundary = new double[numBoundaries];
            double[] oGlobalParameters = new double[numParameters];

            if (numBoundaries > 0)
            {
                int[] oSelectedBoundary = SelectForm.SelectedBoundarySpecies;
                for (int i = 0; i < numBoundaries; i++)
                {
                    oBoundary[i] = Simulator.getBoundarySpeciesByIndex(oSelectedBoundary[i]);
                }
            }


            if (numParameters > 0)
            {
                int[] oSelectedParameters = SelectForm.SelectedParameters;
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

            int numBoundaries = SelectForm.NumSelectedBoundaries;
            int numParameters = SelectForm.NumSelectedParameters;

            if (numBoundaries > 0)
            {
                double[] oBoundary = new double[numBoundaries];
                Marshal.Copy(par, oBoundary, 0, numBoundaries);
                int[] oSelectedBoundary = SelectForm.SelectedBoundarySpecies;
                for (int i = 0; i < numBoundaries; i++)
                {
                    Simulator.setBoundarySpeciesByIndex(oSelectedBoundary[i], (double.IsNaN(oBoundary[i]) ? oSelectedBoundary[i] : oBoundary[i]));
                }
            }

            if (numParameters > 0)
            {
                double[] oParameters = new double[numParameters];
                Marshal.Copy(par, oParameters, numBoundaries, numParameters);
                int[] oSelectedParameters = SelectForm.SelectedParameters;
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

        public AutoResult CurrentResult { get; set; }

        public static FormSelectVariables SelectForm { get; set; }

        public static RoadRunner Simulator { get; set; }

        public IModel CurrentModel { get; set; }

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
                this.Text += string.Format(" - [{0}]", Path.GetFileName(fileName));
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

                    SelectForm.InitializeFromModel(CurrentModel);

                    ArrayList oList = new ArrayList(new string[] { "Time" });
                    oList.AddRange(Simulator.getFloatingSpeciesNames());

                    FormChangeTimeCoursePlot.Instance.XAxisValues = oList.ToArray();

                    if (CurrentModel != null)
                    {
                        setupControl1.Parameters = SelectForm.Parameters.ToArray();
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
                if (SelectForm.NumSelectedParameters > 0)
                    graphControl1.ZedControl.GraphPane.XAxis.Title.Text = (string)Simulator.getGlobalParameterNames()[SelectForm.SelectedParameters[0]];
                else if (SelectForm.NumSelectedBoundaries > 0)
                    graphControl1.ZedControl.GraphPane.XAxis.Title.Text = (string)Simulator.getBoundarySpeciesNames()[SelectForm.SelectedBoundarySpecies[0]];
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
            SelectForm.InitializeFromModel(CurrentModel);
            SelectForm.ShowDialog();
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

        internal static MainForm _Instance = null;

        static MainForm()
        {
            Simulator = new RoadRunner();
            SelectForm = null;
        }

        internal static MainForm Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new MainForm();
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

                if (string.IsNullOrEmpty(SBML))
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
                    // maybe being at steady state is not all that counts
                }
                if (!chkTakeAsIs.Checked)
                    txtConfig.Text = setupControl1.Configuration;
                SetupUsingModel(CurrentModel);
            }

            SelectForm.Parameters = new ArrayList(new object[] { setupControl1.Parameter });            
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

        private void OnFormLoad(object sender, System.EventArgs e)
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

        private string GetCurrentData(bool useTab = false)
        {
            var separartor = useTab ? "\t" : ", ";

            return GenerateCsv(separartor);
        }

        /// <summary>
        /// CurrentHeaders is a List of strings representing the column headers for the data contained in CurrentResult. 
        /// CurrentResult and CurrentHeaders are used by the Simulation Tool host application for the generation of 
        /// CSV or SBRML data (and even TeX support)
        /// </summary>
        public virtual List<string> CurrentHeaders { get; set; }
        
        private string GenerateCsv(string separator = ", ")
        {
            if (CurrentResult != null && CurrentResult.Data != null && CurrentResult.Data.Count > 0 )
                return CurrentResult.Data.GenerateCSV(setupControl1.Parameter, Simulator.GetFloatingSpeciesNamesArray(), separator);
            return "";
        }

        private void OnExportClicked(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog { Title = "Export current plot", Filter = "Latex files|*.tex|CSV files|*.csv;*.tab;*.txt|All files|*.*" };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = dialog.FileName;
                string lower = fileName.ToLowerInvariant();
                if (lower.EndsWith(".tex"))
                    File.WriteAllText(fileName, Util.GenerateTex(graphControl1.ZedControl.MasterPane));
                else
                File.WriteAllText(fileName, GetCurrentData(lower.EndsWith(".tab")));
            }
        }

    }
}
