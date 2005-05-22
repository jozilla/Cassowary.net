using System;

namespace Cassowary
{
	public class ClLinearConstraint : ClConstraint
	{
		public ClLinearConstraint(ClLinearExpression cle,
															ClStrength strength, 
															double weight) : base(strength, weight)
		{
			_expression = cle;
		}

		public ClLinearConstraint(ClLinearExpression cle,
															ClStrength strength) : base(strength, 1.0)
		{		
			_expression = cle;
		}

		public ClLinearConstraint(ClLinearExpression cle) : base(ClStrength.Required, 1.0)
		{
			_expression = cle;
		}

		public override ClLinearExpression Expression
		{
			get { return _expression; }
		}

		protected void SetExpression(ClLinearExpression expr)
		{
			_expression = expr;
		}

		protected ClLinearExpression _expression;
	}
}
