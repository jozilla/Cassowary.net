using System;

namespace Cassowary
{
	public abstract class ClAbstractVariable
	{
		public ClAbstractVariable(string name)
		{
			_name = name;
			iVariableNumber++;
		}
	
		public ClAbstractVariable()
		{
			_name = "v" + iVariableNumber;
			iVariableNumber++;
		}
	
		public ClAbstractVariable(long varnumber, string prefix)
		{
			_name = prefix + varnumber;
			iVariableNumber++;
		}
	
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	
		public virtual bool IsDummy
		{
			get { return false; }
		}

		public abstract bool IsExternal
		{
			get;
		}

		public abstract bool IsPivotable
		{
			get;
		}

		public abstract bool IsRestricted
		{
			get;
		}

		public override abstract string ToString();
		
		public static int NumCreated
		{
			get { return iVariableNumber; }
		}
	
		private string _name;
		private static int iVariableNumber;
	}
}
