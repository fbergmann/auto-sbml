using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using ZedGraph;

namespace AutoFrontend
{
	public class NameValuePair
	{
		private string _Name;
		public string Name
		{
			get {	return _Name; }
			set { _Name = value;	}
		}

		private object _Value;
		public object Value
		{
			get {	return _Value; }
			set { _Value = value;	}
		}

		/// <summary>
		/// Initializes a new instance of the NameValuePair class.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public NameValuePair(string name, object value)
		{
			_Name = name;
			_Value = value;
		}
	}
}
