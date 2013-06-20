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
}
