using System;

namespace Cassowary
{
	public class ClStrayConstraint : ClEditOrStayConstraint
	{
		public ClStrayConstraint(ClVariable var, ClStrength strength, double weight) 
			: base(var, strength, weight)
		{}

		public ClStrayConstraint(ClVariable var, ClStrength strength)
			: base(var, strength, 1.0)
		{}

		public override bool IsStayConstraint
		{
			get { return true; }
		}

		public override string ToString()
		{
			return "stay" + base.ToString();
		}
	}
}
