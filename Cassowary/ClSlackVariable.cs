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
  class ClSlackVariable : ClAbstractVariable
  {
    public ClSlackVariable(string name) : base(name)
    {}

    public ClSlackVariable()
    {}

    public ClSlackVariable(long number, string prefix) : base(number, prefix)
    {}

    public override string ToString()
    {
      return string.Format("[{0}:slack]", Name);
    }

    public override bool IsExternal
    {
      get { return false; }
    }

    public override bool IsPivotable
    {
      get { return true; }
    }

    public override bool IsRestricted
    {
      get { return true; }
    }
  }
}
