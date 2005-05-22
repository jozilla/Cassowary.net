using System;

namespace Cassowary
{
	public abstract class ClEditOrStayConstraint : ClConstraint
	{
		public ClEditOrStayConstraint(ClVariable var, 
																	ClStrength strength, 
																	double weight) : base(strength, weight)
		{
			_variable = var;
			_expression = new ClLinearExpression(_variable, -1.0, _variable.Value);
		}

		public ClEditOrStayConstraint(ClVariable var, 
																	ClStrength strength) : this(var, strength, 1.0)
		{}

		public ClEditOrStayConstraint(ClVariable var) : this(var, ClStrength.Required, 1.0)
		{
			_variable = var;
		}

		public ClVariable Variable
		{
			get { return _variable; }
			private set { _variable = value; }
		}

		public override ClLinearExpression Expression
		{
			get { return _expression; }
		}
		
		protected ClVariable _variable;
		// cache the expression
		private ClLinearExpression _expression;
	}
}
