/*
  Cassowary.net: an incremental constraint solver for .NET
  (http://lumumba.uhasselt.be/jo/projects/cassowary.net/)
  
  Copyright (C) 2005  Jo Vermeulen (jo@lumumba.uhasselt.be)
  
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
  public abstract class ClAbstractVariable
  {
    public ClAbstractVariable(string name)
    {
      _name = name;
      iVariableNumber++;
    }

    public ClAbstractVariable()
    {
      _name = "v" + iVariableNumber;
      iVariableNumber++;
    }

    public ClAbstractVariable(long varnumber, string prefix)
    {
      _name = prefix + varnumber;
      iVariableNumber++;
    }

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public virtual bool IsDummy
    {
      get { return false; }
    }

    public abstract bool IsExternal
    {
      get;
    }

    public abstract bool IsPivotable
    {
      get;
    }

    public abstract bool IsRestricted
    {
      get;
    }

    public override abstract string ToString();
    
    public static int NumCreated
    {
      get { return iVariableNumber; }
    }

    private string _name;
    private static int iVariableNumber;
  }
}
