using System;

namespace Cassowary
{
	public class ClDouble : ICloneable
	{
		public ClDouble(double val)
		{ 
			_value = val;
		}

		public ClDouble() : this(0.0)
		{}

		public virtual object Clone()
		{ 
			return new ClDouble(_value); 
		}

		public /*sealed*/ int IntValue
    { 
			get { return (int) _value; }
		}

		public /*sealed*/ long LongValue
		{ 
			get { return (long) _value; }
		}

		public /*sealed*/ float FloatValue
		{ 
			get { return (float) _value; }
		}

		public /*sealed*/ byte ByteValue
		{ 
			get { return (byte) _value; }
		}

		public /*sealed*/ short ShortValue
		{
			get { return (short) _value; }
		}

		public /*sealed*/ double Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public override sealed String ToString()
		{ 
			return Convert.ToString(_value); 
		}

		public override sealed bool Equals(Object o)
		{ 
			try 
			{
				return _value == ((ClDouble) o)._value;
			} 
			catch (Exception) 
			{
				return false;
			} 
		}

		public override sealed int GetHashCode()
		{ 
			Console.Error.WriteLine("ClDouble.GetHashCode() called!");
			return _value.GetHashCode();
		}

		private double _value;
	}
}
