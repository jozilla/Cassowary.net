using System;

namespace Cassowary
{
	public class ClLinearEquation : ClLinearConstraint
	{
		public ClLinearEquation(ClLinearExpression cle,
														ClStrength strength,
														double weight) : base(cle, strength, weight)
		{}

		public ClLinearEquation(ClLinearExpression cle,
														ClStrength strength) : base(cle, strength)
		{}

		public ClLinearEquation(ClLinearExpression cle) : base(cle)
		{}

		public ClLinearEquation(ClAbstractVariable clv,
														ClLinearExpression cle,
														ClStrength strength,
														double weight) : base(cle, strength, weight)
		{
			_expression.AddVariable(clv, -1.0);
		}

		public ClLinearEquation(ClAbstractVariable clv,
														ClLinearExpression cle,
														ClStrength strength) : this(clv, cle, strength, 1.0)
		{}

		public ClLinearEquation(ClAbstractVariable clv, 
														ClLinearExpression cle) : this(clv, cle, ClStrength.Required, 1.0)
		{}

		public ClLinearEquation(ClAbstractVariable clv, 
														double val, 
														ClStrength strength, 
														double weight) : base(new ClLinearExpression(val), strength, weight)
		{
			_expression.AddVariable(clv, -1.0);
		}

		public ClLinearEquation(ClAbstractVariable clv,
														double val,
														ClStrength strength) : this(clv, val, strength, 1.0)
		{}

		public ClLinearEquation(ClAbstractVariable clv,
														double val) : this(clv, val, ClStrength.Required, 1.0)
		{}

		public ClLinearEquation(ClLinearExpression cle,
														ClAbstractVariable clv,
														ClStrength strength,
														double weight) : base((ClLinearExpression) cle.Clone(), strength, weight)
		{
			_expression.AddVariable(clv, -1.0);
		}

		public ClLinearEquation(ClLinearExpression cle,
														ClAbstractVariable clv,
														ClStrength strength) : this(cle, clv, strength, 1.0)
		{}

		public ClLinearEquation(ClLinearExpression cle, ClAbstractVariable clv) : this(cle, clv, ClStrength.Required, 1.0)
		{}

		public ClLinearEquation(ClLinearExpression cle1,
														ClLinearExpression cle2,
														ClStrength strength,
														double weight) : base((ClLinearExpression) cle1.Clone(), strength, weight)
		{
			_expression.AddExpression(cle2, -1.0);
		}

		public ClLinearEquation(ClLinearExpression cle1,
														ClLinearExpression cle2,
														ClStrength strength) : this(cle1, cle2, strength, 1.0)
		{}
				
		public ClLinearEquation(ClLinearExpression cle1, 
														ClLinearExpression cle2) : this(cle1, cle2, ClStrength.Required, 1.0)
		{}

		public override string ToString()
		{
			return base.ToString() + " = 0 )";
		}
	}
}
