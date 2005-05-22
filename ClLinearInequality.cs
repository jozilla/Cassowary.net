using System;

namespace Cassowary
{
	public class ClLinearInequality : ClLinearConstraint
	{
		public ClLinearInequality(ClLinearExpression cle,
															ClStrength strength,
															double weight) : base(cle, strength, weight)
		{}

		public ClLinearInequality(ClLinearExpression cle,
															ClStrength strength) : base(cle, strength)
		{}

		public ClLinearInequality(ClLinearExpression cle) : base(cle)
		{}

		public ClLinearInequality(ClVariable clv1,
															byte op_enum,
															ClVariable clv2,
															ClStrength strength,
															double weight) : base(new ClLinearExpression(clv2), strength, weight)
															/* throws ExClInternalError */
		{
			switch (op_enum)
			{
				case Cl.GEQ:
					_expression.MultiplyMe(-1.0);
					_expression.AddVariable(clv1);
					break;
				case Cl.LEQ:
					_expression.AddVariable(clv1, -1.0);
					break;
				default:
					// invalid operator
					throw new ExClInternalError("Invalid operator in ClLinearInequality constructor");
			}
		}

		public ClLinearInequality(ClVariable clv1,
															byte op_enum,
															ClVariable clv2,
															ClStrength strength) : this(clv1, op_enum, clv2, strength, 1.0)
															/* throws ExClInternalError */
		{}

		public ClLinearInequality(ClVariable clv1,
															byte op_enum,
															ClVariable clv2) : this(clv1, op_enum, clv2, ClStrength.Required, 1.0)
															/* throws ExClInternalError */
		{}

		public ClLinearInequality(ClVariable clv,
															byte op_enum,
															double val,
															ClStrength strength,
															double weight) : base(new ClLinearExpression(val), strength, weight)
															/* throws ExClInternalError */
		{
			switch (op_enum)
			{
				case Cl.GEQ:
					_expression.MultiplyMe(-1.0);
					_expression.AddVariable(clv);
					break;
				case Cl.LEQ:
					_expression.AddVariable(clv, -1.0);
					break;
				default:
					// invalid operator
					throw new ExClInternalError("Invalid operator in ClLinearInequality constructor");
			}
		}

		public ClLinearInequality(ClVariable clv,
															byte op_enum,
															double val,
															ClStrength strength) : this(clv, op_enum, val, strength, 1.0)
															/* throws ExClInternalError */
		{}

		public ClLinearInequality(ClVariable clv,
															byte op_enum,
															double val) : this(clv, op_enum, val, ClStrength.Required, 1.0)
															/* throws ExClInternalError */
		{}

		public ClLinearInequality(ClLinearExpression cle1,
															byte op_enum,
															ClLinearExpression cle2,
															ClStrength strength,
															double weight) : base((ClLinearExpression) cle2.Clone(), strength, weight)
															/* throws ExClInternalError */
		{
			switch (op_enum)
			{
				case Cl.GEQ:
					_expression.MultiplyMe(-1.0);
					_expression.AddExpression(cle1);
					break;
				case Cl.LEQ:
					_expression.AddExpression(cle1, -1.0);
					break;
				default:
					// invalid operator
					throw new ExClInternalError("Invalid operator in ClLinearInequality constructor");
			}
		}

		public ClLinearInequality(ClLinearExpression cle1,
															byte op_enum,
															ClLinearExpression cle2,
															ClStrength strength) : this(cle1, op_enum, cle2, strength, 1.0)
															/* throws ExClInternalError */
		{}

		public ClLinearInequality(ClLinearExpression cle1,
															byte op_enum,
															ClLinearExpression cle2) : this(cle1, op_enum, cle2, ClStrength.Required, 1.0)
															/* throws ExClInternalError */
		{}

		public ClLinearInequality(ClAbstractVariable clv,
															byte op_enum,
															ClLinearExpression cle,
															ClStrength strength,
															double weight) : base((ClLinearExpression) cle.Clone(), strength, weight)
															/* throws ExClInternalError */
		{
			switch (op_enum)
			{
				case Cl.GEQ:
					_expression.MultiplyMe(-1.0);
					_expression.AddVariable(clv);
					break;
				case Cl.LEQ:
					_expression.AddVariable(clv, -1.0);
					break;
				default:
					// invalid operator
					throw new ExClInternalError("Invalid operator in ClLinearInequality constructor");
			}
		}

		public ClLinearInequality(ClAbstractVariable clv,
															byte op_enum,
															ClLinearExpression cle,
															ClStrength strength) : this(clv, op_enum, cle, strength, 1.0)
															/* throws ExClInternalError */
		{}

		public ClLinearInequality(ClAbstractVariable clv,
															byte op_enum,
															ClLinearExpression cle) : this(clv, op_enum, cle, ClStrength.Required, 1.0)
															/* throws ExClInternalError */
		{}

		public ClLinearInequality(ClLinearExpression cle,
															byte op_enum,
															ClAbstractVariable clv,
															ClStrength strength,
															double weight) : base((ClLinearExpression) cle.Clone(), strength, weight)
															/* throws ExClInternalError */
		{
			switch (op_enum)
			{
				case Cl.LEQ:
					_expression.MultiplyMe(-1.0);
					_expression.AddVariable(clv);
					break;
				case Cl.GEQ:
					_expression.AddVariable(clv, -1.0);
					break;
				default:
					// invalid operator
					throw new ExClInternalError("Invalid operator in ClLinearInequality constructor");
			}
		}

		public ClLinearInequality(ClLinearExpression cle,
															byte op_enum,
															ClAbstractVariable clv,
															ClStrength strength) : this(cle, op_enum, clv, strength, 1.0)
															/* throws ExClInternalError */
		{}

		public ClLinearInequality(ClLinearExpression cle,
															byte op_enum,
															ClAbstractVariable clv) : this(cle, op_enum, clv, ClStrength.Required, 1.0)
															/* throws ExClInternalError */
		{}

		public sealed override bool IsInequality
		{
			get { return true; }
		}

		public sealed override string ToString()
		{
			return base.ToString() + " >= 0 )";
		}
	}
}
