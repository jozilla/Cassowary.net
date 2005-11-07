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
  /// <summary>
  /// ClEditInfo is privately-used class
  /// that just wraps a constraint, its positive and negative
  /// error variables, and its prior edit constant.
  /// It is used as values in _editVarMap, and replaces
  /// the parallel vectors of error variables and previous edit
  /// constants from the Smalltalk version of the code.
  /// </summary>
  class ClEditInfo
  {
    public ClEditInfo(ClConstraint cn, ClSlackVariable eplus, 
                      ClSlackVariable eminus, double prevEditConstant,
                      int i)
    {
      _cn = cn;
      _clvEditPlus = eplus;
      _clvEditMinus = eminus;
      _prevEditConstant = prevEditConstant;
      _i = i;
    }

    public int Index
    {
      get { return _i; }
    }

    public ClConstraint Constraint
    {
      get { return _cn; }
    }

    public ClSlackVariable ClvEditPlus
    {
      get { return _clvEditPlus; }
    }

    public ClSlackVariable ClvEditMinus
    {
      get { return _clvEditMinus; }
    }

    public double PrevEditConstant
    {
      get { return _prevEditConstant; }
      set { _prevEditConstant = value; }
    }

    private ClConstraint _cn;
    private ClSlackVariable _clvEditPlus;
    private ClSlackVariable _clvEditMinus;
    private double _prevEditConstant;
    private int _i;
  }
}
