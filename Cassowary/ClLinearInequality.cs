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
  public class ClLinearInequality : ClLinearConstraint
  {
    public ClLinearInequality(ClLinearExpression cle,
                              ClStrength strength,
                              double weight) : base(cle, strength, weight)
    {}

    public ClLinearInequality(ClLinearExpression cle,
                              ClStrength strength) : base(cle, strength)
    {}

    public ClLinearInequality(ClLinearExpression cle) : base(cle)
    {}

    public ClLinearInequality(ClVariable clv1,
                              byte op_enum,
                              ClVariable clv2,
                              ClStrength strength,
                              double weight) : base(new ClLinearExpression(clv2), strength, weight)
                              /* throws ExClInternalError */
    {
      switch (op_enum)
      {
        case Cl.GEQ:
          _expression.MultiplyMe(-1.0);
          _expression.AddVariable(clv1);
          break;
        case Cl.LEQ:
          _expression.AddVariable(clv1, -1.0);
          break;
        default:
          // invalid operator
          throw new ExClInternalError("Invalid operator in ClLinearInequality constructor");
      }
    }

    public ClLinearInequality(ClVariable clv1,
                              byte op_enum,
                              ClVariable clv2,
                              ClStrength strength) : this(clv1, op_enum, clv2, strength, 1.0)
                              /* throws ExClInternalError */
    {}

    public ClLinearInequality(ClVariable clv1,
                              byte op_enum,
                              ClVariable clv2) : this(clv1, op_enum, clv2, ClStrength.Required, 1.0)
                              /* throws ExClInternalError */
    {}

    public ClLinearInequality(ClVariable clv,
                              byte op_enum,
                              double val,
                              ClStrength strength,
                              double weight) : base(new ClLinearExpression(val), strength, weight)
                              /* throws ExClInternalError */
    {
      switch (op_enum)
      {
        case Cl.GEQ:
          _expression.MultiplyMe(-1.0);
          _expression.AddVariable(clv);
          break;
        case Cl.LEQ:
          _expression.AddVariable(clv, -1.0);
          break;
        default:
          // invalid operator
          throw new ExClInternalError("Invalid operator in ClLinearInequality constructor");
      }
    }

    public ClLinearInequality(ClVariable clv,
                              byte op_enum,
                              double val,
                              ClStrength strength) : this(clv, op_enum, val, strength, 1.0)
                              /* throws ExClInternalError */
    {}

    public ClLinearInequality(ClVariable clv,
                              byte op_enum,
                              double val) : this(clv, op_enum, val, ClStrength.Required, 1.0)
                              /* throws ExClInternalError */
    {}

    public ClLinearInequality(ClLinearExpression cle1,
                              byte op_enum,
                              ClLinearExpression cle2,
                              ClStrength strength,
                              double weight) : base((ClLinearExpression) cle2.Clone(), strength, weight)
                              /* throws ExClInternalError */
    {
      switch (op_enum)
      {
        case Cl.GEQ:
          _expression.MultiplyMe(-1.0);
          _expression.AddExpression(cle1);
          break;
        case Cl.LEQ:
          _expression.AddExpression(cle1, -1.0);
          break;
        default:
          // invalid operator
          throw new ExClInternalError("Invalid operator in ClLinearInequality constructor");
      }
    }

    public ClLinearInequality(ClLinearExpression cle1,
                              byte op_enum,
                              ClLinearExpression cle2,
                              ClStrength strength) : this(cle1, op_enum, cle2, strength, 1.0)
                              /* throws ExClInternalError */
    {}

    public ClLinearInequality(ClLinearExpression cle1,
                              byte op_enum,
                              ClLinearExpression cle2) : this(cle1, op_enum, cle2, ClStrength.Required, 1.0)
                              /* throws ExClInternalError */
    {}

    public ClLinearInequality(ClAbstractVariable clv,
                              byte op_enum,
                              ClLinearExpression cle,
                              ClStrength strength,
                              double weight) : base((ClLinearExpression) cle.Clone(), strength, weight)
                              /* throws ExClInternalError */
    {
      switch (op_enum)
      {
        case Cl.GEQ:
          _expression.MultiplyMe(-1.0);
          _expression.AddVariable(clv);
          break;
        case Cl.LEQ:
          _expression.AddVariable(clv, -1.0);
          break;
        default:
          // invalid operator
          throw new ExClInternalError("Invalid operator in ClLinearInequality constructor");
      }
    }

    public ClLinearInequality(ClAbstractVariable clv,
                              byte op_enum,
                              ClLinearExpression cle,
                              ClStrength strength) : this(clv, op_enum, cle, strength, 1.0)
                              /* throws ExClInternalError */
    {}

    public ClLinearInequality(ClAbstractVariable clv,
                              byte op_enum,
                              ClLinearExpression cle) : this(clv, op_enum, cle, ClStrength.Required, 1.0)
                              /* throws ExClInternalError */
    {}

    public ClLinearInequality(ClLinearExpression cle,
                              byte op_enum,
                              ClAbstractVariable clv,
                              ClStrength strength,
                              double weight) : base((ClLinearExpression) cle.Clone(), strength, weight)
                              /* throws ExClInternalError */
    {
      switch (op_enum)
      {
        case Cl.LEQ:
          _expression.MultiplyMe(-1.0);
          _expression.AddVariable(clv);
          break;
        case Cl.GEQ:
          _expression.AddVariable(clv, -1.0);
          break;
        default:
          // invalid operator
          throw new ExClInternalError("Invalid operator in ClLinearInequality constructor");
      }
    }

    public ClLinearInequality(ClLinearExpression cle,
                              byte op_enum,
                              ClAbstractVariable clv,
                              ClStrength strength) : this(cle, op_enum, clv, strength, 1.0)
                              /* throws ExClInternalError */
    {}

    public ClLinearInequality(ClLinearExpression cle,
                              byte op_enum,
                              ClAbstractVariable clv) : this(cle, op_enum, clv, ClStrength.Required, 1.0)
                              /* throws ExClInternalError */
    {}

    public sealed override bool IsInequality
    {
      get { return true; }
    }

    public sealed override string ToString()
    {
      return base.ToString() + " >= 0)";
    }
  }
}
