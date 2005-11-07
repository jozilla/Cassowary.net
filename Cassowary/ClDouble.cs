/*
  Cassowary.net: an incremental constraint solver for .NET
  (http://lumumba.uhasselt.be/jo/projects/cassowary.net/)
    
  Copyright (C) 2005  Jo Vermeulen (jo.vermeulen@uhasselt.be)
    
  This program is free software; you can redistribute it and/or
  modify it under the terms of the GNU Lesser General Public License
  as published by the Free Software Foundation; either version 2.1
  of  the License, or (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU Lesser General Public License for more details.

  You should have received a copy of the GNU Lesser General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

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
      #if !COMPACT 
        Console.Error.WriteLine("ClDouble.GetHashCode() called!");
      #else
        Console.WriteLine("ClDouble.GetHashCode() called!");
      #endif
      
      return _value.GetHashCode();
    }

    private double _value;
  }
}
