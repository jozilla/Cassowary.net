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
  GNU General Public License for more details.

  You should have received a copy of the GNU Lesser General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

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
