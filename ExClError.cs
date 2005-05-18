using System;

namespace Cassowary
{
	public class ExClError : Exception
	{
		public virtual string Description()
		{ 
			return "(ExClError) An error has occured in CL";
		}
	}
}
