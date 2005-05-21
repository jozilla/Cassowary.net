using System;

namespace Cassowary.net
{
	public class ExClConstraintNotFound : ExClError
	{
		public override string Description()
		{ 
			return "(ExCLConstraintNotFound) Tried to remove a constraint never added to the tableau"; 
		}
	}
}
