using System;

namespace Cassowary
{
	public class ExClNonlinearExpression : ExClError
	{
		public override string Description()
		{ 
			return "(ExClNonlinearExpression) The resulting expression would be nonlinear"; 
		}
	}
}
