using System;

namespace Cassowary.net
{
	public class ClDummyVariable : ClAbstractVariable
	{
		public ClDummyVariable(string name) : base(name)
		{}

		public ClDummyVariable()
		{}

		public ClDummyVariable(long number, string prefix) : base(number, prefix)
		{}

		public override string ToString()
		{
			return "[" + Name +":dummy]";
		}

		public override bool IsDummy
		{
			get { return true; }
		}

		public override bool IsExternal
		{
			get { return false; }
		}

		public override bool IsPivotable
		{
			get{ return false; }
		}

		public override bool IsRestricted
		{
			get { return false; }
		}
	}
}
