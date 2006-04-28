/*
  Cassowary.net: an incremental constraint solver for .NET
  (http://lumumba.uhasselt.be/jo/projects/cassowary.net/)
  
  Copyright (C) 2005-2006  Jo Vermeulen (jo.vermeulen@uhasselt.be)
  
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
using System.Collections;

namespace Cassowary.Utils
{
  /// <summary>
  /// A simple set class for use in ClTableau and ClSimplexSolver.
  /// </summary>
  /// <remarks>
  /// Uses a Hashtable internally for storage.
  /// </remarks>
  public class Set : ICloneable
  {
    public Set()
    {
      _hash = new Hashtable();
    }

    public Set(int i)
    {
      _hash = new Hashtable(i);
    }

    public Set(int i, float f)
    {
      _hash = new Hashtable(i, f);
    }

    public Set(Hashtable h)
    {
      _hash = h;
    }

    public bool ContainsKey(object o)
    {
      return _hash.ContainsKey(o);
    }

    public void Add(object o)
    {
      // BUG: overwrite?
      //_hash.Add(o, o);
      _hash[o] = o;
    }

    public void Remove(object o)
    {
      _hash.Remove(o);
    }

    public void Clear()
    {
      _hash.Clear();
    }

    public object Clone()
    {
      return new Set((Hashtable)_hash.Clone());
    }

    public virtual IEnumerator GetEnumerator()
    {
      return _hash.Keys.GetEnumerator();
    }

    public override string ToString()
    {
      // start with left brace
      string result = "{";
      string separator = ", ";
      
      // add each element

      foreach (object o in _hash.Keys)
      {
        result += o;
        result += separator;
      }

      if (result.Length > 1) // if we added something to result
      { 
        // remove last separator
        result = result.Substring(0, result.Length - separator.Length);
      }

      // add right brace
      result += "}";

      return result;
    }

    /// <summary>
    /// For casting between Set and Hashtable.
    /// </summary>
    public static explicit operator Set (Hashtable h)
    {
      Set res = new Set(h);

      return res;
    }

    public int Count
    {
      get { return _hash.Count; }
    }

    public bool Empty
    {
      get { return _hash.Count == 0; }
    }

    private Hashtable _hash;
  }
}
