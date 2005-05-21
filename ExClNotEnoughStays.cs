using System;

namespace Cassowary.net
{
	public class ExClNotEnoughStays : ExClError
	{
		public override string Description()
		{ 
			return "(ExCLNotEnoughStays) There are not enough stays to give specific values to every variable";
		}
	}
}
