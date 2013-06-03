using System;
using System.Threading;
using System.Windows.Forms;
using SBW;
namespace AutoFrontend
{
	/// <summary>
	/// Summary description for Program.
	/// </summary>
	public class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) 
		{

            System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CreateSpecificCulture("en");
            culture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = culture;

            //Application.SetCompatibleTextRenderingDefault(false); 
            Application.EnableVisualStyles();
			Application.DoEvents();
			

			Form1 newForm1 = Form1.Instance;
			object oBox = newForm1;

			ModuleImplementation oImpl = new ModuleImplementation(
				"AutoCSharp", "Auto CSharp", LowLevel.ModuleManagementType.UniqueModule,
				"Direct Interface to AUTO2000 with C# frontend");
			oImpl.addService("auto", "Auto2000 (C#)", "/Analysis", "Direct Interface to AUTO2000 with C# frontend", ref oBox);
			oImpl.EnableServices(args);

			Application.Run(newForm1);
		}

	}
}
