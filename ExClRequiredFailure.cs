using System;

namespace Cassowary
{
	public class ExClRequiredFailure : ExClError
	{
		public override string Description()
		{ 
			return "(ExCLRequiredFailure) A required constraint cannot be satisfied";
		}
	}
}
