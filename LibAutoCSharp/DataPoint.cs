using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;

namespace LibAutoCSharp
{
    
	public class DataPoint
	{
		private int _Point;
		public int Point
		{
			get { return _Point; }
			set { _Point = value; }
		}

		private int _Type;
		public int Type
		{
			get { return _Type; }
			set { _Type = value; }
		}

		private int _Label;
		public int Label
		{
			get { return _Label; }
			set { _Label = value; }
		}

		private double _Par;
		public double Par
		{
			get { return _Par; }
			set { _Par = value; }
		}


		private double[] _Variables;
		public double[] Variables
		{
			get { return _Variables; }
			set { _Variables = value; }
		}

		public int NumVariables
		{
			get
			{
				if (_Variables == null)
					return 0;
				else return _Variables.Length;
			}
		}

		/// <summary>
		/// Initializes a new instance of the DataPoint class.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="type"></param>
		/// <param name="label"></param>
		/// <param name="par"></param>
		/// <param name="var1"></param>
		/// <param name="var2"></param>
		public DataPoint(int point, int type, int label, double par, double[] variables)
		{
			_Point = point;
			_Type = type;
			_Label = label;
			_Par = par;
			_Variables = variables;
		}



	}
}
