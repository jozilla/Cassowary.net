namespace Cassowary
{
	public class ClEditConstraint : ClEditOrStayConstraint
	{
		public ClEditConstraint(ClVariable clv,
														ClStrength strength,
														double weight) : base(clv, strength, weight)
		{}

		public ClEditConstraint(ClVariable clv, 
														ClStrength strength) : base(clv, strength)
		{}

		public ClEditConstraint(ClVariable clv) : base(clv)
		{}

		public override bool IsEditConstraint
		{
			get { return true; }
		}

		public override string ToString()
		{
			return "edit" + base.ToString();
		}
	}
}
