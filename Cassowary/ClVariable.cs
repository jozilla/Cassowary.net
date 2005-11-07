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

using System.Collections;

namespace Cassowary
{
  public class ClVariable : ClAbstractVariable
  {
    public ClVariable(string name, double value) : base(name)
    {
      _value = value;
      if (_ourVarMap != null) {
        _ourVarMap.Add(name, this);
      }
    }

    public ClVariable(string name) : base(name)
    {
      _value = 0.0;
      if (_ourVarMap != null)
      {
        _ourVarMap.Add(name, this);
      }
    }
  
    public ClVariable(double value)
    {
      _value = value;
    }
  
    public ClVariable()
    {
      _value = 0.0;
    }

    public ClVariable(long number, string prefix, double value) : base(number, prefix)
    {
      _value = value;
    }
  
    public ClVariable(long number, string prefix) : base(number, prefix)
    {
      _value = 0.0;
    }
  
    public override bool IsDummy
    {
      get { return false; }
    }

    public override bool IsExternal
    {
      get { return true; }
    }
  
    public override bool IsPivotable
    {
      get { return false; }
    }

    public override bool IsRestricted
    {
      get { return false; }
    }

    public override string ToString()
    {
      return "[" + Name + ":" + _value + "]";
    }

    /// <remarks>
    /// Change the value held -- should *not* use this if the variable is 
    /// in a solver -- instead use AddEditVar() and SuggestValue() interface
    /// </remarks>
    public /*sealed*/ double Value
    {
      get { return _value; }
      set { _value = value; }
    }

    /// <remarks>
    /// Permit overriding in subclasses in case something needs to be
    /// done when the value is changed by the solver
    /// may be called when the value hasn't actually changed -- just
    /// means the solver is setting the external variable
    /// </remarks>
    public void ChangeValue(double value)
    {
      _value = value;
    }

    public object AttachedObject
    {
      get { return _attachedObject; }
      set { _attachedObject = value; }
    }
  
    public static Hashtable VarMap
    {
      get { return _ourVarMap; }
      set { _ourVarMap = value; }
    }
  
    private static Hashtable _ourVarMap;
    private double _value;
    private object _attachedObject;
  }
}
