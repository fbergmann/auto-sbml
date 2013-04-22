using System;

namespace AutoFrontend
{
	/// <summary>
	/// Summary description for Util.
	/// </summary>
	internal class Util
	{
		public static string ConvertToSBML(string sJarnac)
		{
			try
			{
				return SBW.HighLevel.call("JarnacLiteConsole", "translator", "string translateSBML(string)", new SBW.DataBlockWriter(new object[] { sJarnac })).getString();
			}
			catch
			{
				return "";
			}
		}

		public static string ConvertToJarnac(string sSBML)
		{
			try
			{
				return SBW.HighLevel.call("JarnacLiteConsole", "translator", "string translate(string)", new SBW.DataBlockWriter(new object[] { sSBML })).getString();
			}
			catch
			{
				return "";
			}
		}
		public static double ConvertToDouble(string s, double dDefault)
		{
			try
			{
				return Convert.ToDouble(s);
			}
			catch (Exception)
			{
				return dDefault;
			}
		}
		public static int ConvertToInt(string s, int nDefault)
		{
			try
			{
				return Convert.ToInt32(s);
			}
			catch (Exception)
			{
				return nDefault;
			}
		}

		public static string GetSBMLForParameter(NameValuePair oParameter)
		{
			SBWModules.SBMLSupport.loadSBML(Form1.Simulator.getSBML());
			SBWModules.SBMLSupport.setValue(oParameter.Name, (double)oParameter.Value);
			return SBWModules.SBMLSupport.getSBML();
		}

		public static void SentToSimulator(string sSBML)
		{
			try
			{
				SBW.HighLevel.send("edu.kgi.roadRunner.sim Simulation Service", "wrapper", "void doAnalysis(string)", new SBW.DataBlockWriter(new object[] { sSBML }));
			}
			catch
			{
			}

		}
	}
}
