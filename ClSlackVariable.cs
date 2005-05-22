using System;

namespace Cassowary
{
	public class ClSlackVariable : ClAbstractVariable
	{
		public ClSlackVariable(string name) : base(name)
		{}

		public ClSlackVariable()
		{}

		public ClSlackVariable(long number, string prefix) : base(number, prefix)
		{}

		public override string ToString()
		{
			return string.Format("[{0}:slack]", Name);
		}

		public override bool IsExternal
		{
			get { return false; }
		}

		public override bool IsPivotable
		{
			get { return true; }
		}

		public override bool IsRestricted
		{
			get { return true; }
		}
	}
}
