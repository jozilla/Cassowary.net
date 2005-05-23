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

		public /*sealed*/ int intValue()
        { 
			return (int) _value; 
		}

		public /*sealed*/ long longValue()
		{ 
			return (long) _value; 
		}

		public /*sealed*/ float floatValue()
		{ 
			return (float) _value; 
		}

		public /*sealed*/ byte byteValue()
        { 
			return (byte) _value; 
		}

		public /*sealed*/ short shortValue()
        { 
			return (short) _value; 
		}

		public /*sealed*/ double Value
        {
			get {
				return _value;
			}
			set {
				_value = value; 
			}
			
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
			Console.Error.WriteLine("ClDouble.hashCode() called!");
			return _value.GetHashCode();
		}

		private double _value;
	}
}
