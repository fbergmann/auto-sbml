using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;

namespace LibAutoCSharp
{
	public class DataPointCollection: CollectionBase
	{
		// public methods...
		#region Add
		public int Add(DataPoint dataPoint)
		{
			return List.Add(dataPoint);
		}
		#endregion
		#region IndexOf
		public int IndexOf(DataPoint dataPoint)
		{
			for(int i = 0; i < List.Count; i++)
				if (this[i] == dataPoint)    // Found it
					return i;
			return -1;
		}
		#endregion
		#region Insert
		public void Insert(int index, DataPoint dataPoint)
		{
			List.Insert(index, dataPoint);
		}
		#endregion
		#region Remove
		public void Remove(DataPoint dataPoint)
		{
			List.Remove(dataPoint);
		}
		#endregion
		#region Find
		// TODO: If desired, change parameters to Find method to search based on a property of DataPoint.
		public DataPoint Find(DataPoint dataPoint)
		{
			foreach(DataPoint dataPointItem in this)
				if (dataPointItem == dataPoint)    // Found it
					return dataPointItem;
			return null;    // Not found
		}
		#endregion
		#region Contains
		// TODO: If you changed the parameters to Find (above), change them here as well.
		public bool Contains(DataPoint dataPoint)
		{
			return (Find(dataPoint) != null);
		}
		#endregion
		    	
		// public properties...
		#region this[int index]
		public DataPoint this[int index] 
		{
			get
			{
				return (DataPoint) List[index];
			}
			set
			{
				List[index] = value;
			}
		}
		#endregion
	}
}
