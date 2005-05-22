using System;

namespace Cassowary
{
	public class ExClInternalError : ExClError
	{
		public ExClInternalError(string s) 
		{
			description_ = s;
		}

		public override string Description()
		{ 
			return string.Format("(ExClInternalError) ", description_);
		}

		private String description_;
	}
}
