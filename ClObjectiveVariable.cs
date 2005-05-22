using System;

namespace Cassowary
{
	public class ClObjectiveVariable : ClAbstractVariable
	{
		public ClObjectiveVariable(string name) : base(name)
		{}

		public ClObjectiveVariable(long number, string prefix) : base(number, prefix)
		{}

		public override string ToString()
		{
			return string.Format("[{0}:obj]", Name);
		}

		public override bool IsExternal
		{
			get { return false; }
		}

		public override bool IsPivotable
		{
			get { return false; }
		}

		public override bool IsRestricted
		{
			get { return false; }
		}
	}
}
