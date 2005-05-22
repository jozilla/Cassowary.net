using System;

namespace Cassowary
{
	public class ClStrength
	{
		public ClStrength(string name, ClSymbolicWeight symbolicWeight)
		{
			_name = name;
			_symbolicWeight = symbolicWeight;
		}

		public ClStrength(string name, double w1, double w2, double w3)
		{
			_name = name;
			_symbolicWeight = new ClSymbolicWeight(w1, w2, w3);
		}

		public bool IsRequired
		{
			get { return (this == Required); }
		}

		public ClSymbolicWeight SymbolicWeight
		{
			get { return _symbolicWeight; } 
			set { _symbolicWeight = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		
		public override string ToString()
		{
			if (!IsRequired)
				return string.Format("{0}:{1}", Name, SymbolicWeight);
			else
				return Name;
		}
		
		public static ClStrength Required
		{
			get { return _required; }
		}

		public static ClStrength Strong
		{
			get { return _strong; }
		}

		public static ClStrength Medium
		{
			get { return _medium; }
		}

		public static ClStrength Weak
		{
			get { return _weak; }
		}
		
		private string _name;
		private ClSymbolicWeight _symbolicWeight;

		private static readonly ClStrength _required = new ClStrength("<Required>", 1000, 1000, 1000);
		private static readonly ClStrength _strong = new ClStrength("strong", 1.0, 0.0, 0.0);
		private static readonly ClStrength _medium = new ClStrength("medium", 0.0, 1.0, 0.0);
		private static readonly ClStrength _weak = new ClStrength("weak", 0.0, 0.0, 1.0);
	}
}
