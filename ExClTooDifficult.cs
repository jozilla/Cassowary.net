using System;

namespace Cassowary.net
{
	public class ExClTooDifficult : ExClError
	{
		public override string Description()
		{ 
			return "(ExCLTooDifficult) The constraints are too difficult to solve"; 
		}
	}
}
