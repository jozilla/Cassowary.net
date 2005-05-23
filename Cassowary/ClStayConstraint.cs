using System;

namespace Cassowary
{
	public class ClStayConstraint : ClEditOrStayConstraint
	{
		public ClStayConstraint(ClVariable var, ClStrength strength, double weight) 
			: base(var, strength, weight)
		{}

		public ClStayConstraint(ClVariable var, ClStrength strength)
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
