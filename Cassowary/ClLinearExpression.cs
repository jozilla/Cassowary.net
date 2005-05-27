/*
	Cassowary.net: an incremental constraint solver for .NET
	(http://lumumba.uhasselt.be/jo/projects/cassowary.net/)
	
	Copyright (C) 2005  Jo Vermeulen (jo@lumumba.uhasselt.be)
	
	This program is free software; you can redistribute it and/or
	modify it under the terms of the GNU Lesser General Public License
	as published by the Free Software Foundation; either version 2.1
	of	the License, or (at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU Lesser General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using System;
using System.Collections;

namespace Cassowary
{
	public class ClLinearExpression : Cl, ICloneable
	{
		public ClLinearExpression(ClAbstractVariable clv, double value, double constant)
		{
			if (Cl.cGC)
				Console.Error.WriteLine("new ClLinearExpression");

			_constant = new ClDouble(constant);
			_terms = new Hashtable(1);
			
			if (clv != null)
				_terms.Add(clv, new ClDouble(value));
		}

		public ClLinearExpression(double num) : this(null, 0, num)
		{}

		public ClLinearExpression() : this(0)
		{}

		public ClLinearExpression(ClAbstractVariable clv, double value) : this(clv, value, 0.0)
		{}

		public ClLinearExpression(ClAbstractVariable clv) : this(clv, 1, 0)
		{}

		/// <summary>
		/// For use by the clone method.
		/// </summary>
		protected ClLinearExpression(ClDouble constant, Hashtable terms)
		{
			if (Cl.cGC)
				Console.Error.WriteLine("clone ClLinearExpression");
			
			_constant = (ClDouble) constant.Clone();
			_terms = new Hashtable();

			// need to unalias the ClDouble-s that we clone (do a deep clone)
			foreach (ClAbstractVariable clv in terms.Keys)
			{
				_terms.Add(clv, ((ClDouble) terms[clv]).Clone());
			}
		}

		public ClLinearExpression MultiplyMe(double x)
		{
			_constant.Value = _constant.Value * x;
      
			foreach (ClAbstractVariable clv in _terms.Keys)
			{
				ClDouble cld = (ClDouble) _terms[clv];
				cld.Value = cld.Value * x;
			}
 
			return this;
		}

		public virtual object Clone()
		{
			return new ClLinearExpression(_constant, _terms);
		}

		public /*sealed*/ ClLinearExpression Times(double x)
		{
			return ((ClLinearExpression) Clone()).MultiplyMe(x);
		}

		public /*sealed*/ ClLinearExpression Times(ClLinearExpression expr) /*throws ExCLNonlinearExpression*/
		{
			if (IsConstant)
			{
				return expr.Times(_constant.Value);
			}
			else if (!expr.IsConstant)
			{
				throw new ExClNonlinearExpression();
			}	
			
			return Times(expr._constant.Value);
		}

		public /*sealed*/ ClLinearExpression Plus(ClLinearExpression expr)
		{
			return ((ClLinearExpression) Clone()).AddExpression(expr, 1.0);
		}

		public /*sealed*/ ClLinearExpression Plus(ClVariable var) /*throws ExCLNonlinearExpression*/
		{ 
			return ((ClLinearExpression) Clone()).AddVariable(var, 1.0);
		}

		public /*sealed*/ ClLinearExpression Minus(ClLinearExpression expr)
		{
			return ((ClLinearExpression) Clone()).AddExpression(expr, -1.0);
		}

		public /*sealed*/ ClLinearExpression Minus(ClVariable var) /*throws ExCLNonlinearExpression*/
		{ 
			return ((ClLinearExpression) Clone()).AddVariable(var, -1.0);
		}

		public /*sealed*/ ClLinearExpression Divide(double x) /*throws ExCLNonlinearExpression*/
		{
			if (Cl.Approx(x, 0.0))
			{	
				throw new ExClNonlinearExpression();
			}
            
			return Times(1.0 / x);
		}

		public /*sealed*/ ClLinearExpression Divide(ClLinearExpression expr) /*throws ExCLNonlinearExpression*/
		{
			if (!expr.IsConstant)
			{
				throw new ExClNonlinearExpression();
			}

			return Divide(expr._constant.Value);
		}

		public /*sealed*/ ClLinearExpression DivFrom(ClLinearExpression expr) /*throws ExCLNonlinearExpression*/
		{
			if (!IsConstant || Cl.Approx(_constant.Value, 0.0))
			{
				throw new ExClNonlinearExpression();
			}
			
			return expr.Divide(_constant.Value);
		}

		public /*sealed*/ ClLinearExpression SubtractFrom(ClLinearExpression expr)
		{ 
			return expr.Minus(this);
		}

		/// <summary>
		/// Add n*expr to this expression from another expression expr.
		/// Notify the solver if a variable is added or deleted from this
		/// expression.
		/// </summary>
		public /*sealed*/ ClLinearExpression AddExpression(ClLinearExpression expr, double n,
			ClAbstractVariable subject, ClTableau solver)
		{
			IncrementConstant(n * expr.Constant);
      
			foreach (ClAbstractVariable clv in expr.Terms.Keys)
			{
				double coeff = ((ClDouble) expr.Terms[clv]).Value;
				AddVariable(clv, coeff * n, subject, solver);
			}

			return this;
		}

		/// <summary>
		/// Add n*expr to this expression from another expression expr.
		/// </summary>
		public /*sealed*/ ClLinearExpression AddExpression(ClLinearExpression expr, double n)
		{
			IncrementConstant(n * expr.Constant);
      
			foreach (ClAbstractVariable clv in expr.Terms.Keys)
			{
				double coeff = ((ClDouble) expr.Terms[clv]).Value;
				AddVariable(clv, coeff * n);
			}

			return this;
		}

		public /*sealed*/ ClLinearExpression AddExpression(ClLinearExpression expr)
		{
			return AddExpression(expr, 1.0);
		}

		/// <summary>
		/// Add a term c*v to this expression.  If the expression already
		/// contains a term involving v, add c to the existing coefficient.
		/// If the new coefficient is approximately 0, delete v.
		/// </summary>
		public /*sealed*/ ClLinearExpression AddVariable(ClAbstractVariable v, double c)
		{ 
			// body largely duplicated below
			if (cTraceOn) 
				FnEnterPrint(string.Format("AddVariable: {0}, {1}", v, c));

			ClDouble coeff = (ClDouble) _terms[v];
			
			if (coeff != null) 
			{
				double new_coefficient = coeff.Value + c;
				
				if (Cl.Approx(new_coefficient, 0.0)) 
				{
					_terms.Remove(v);
				}
				else 
				{
					coeff.Value = new_coefficient;
				}
			} 
			else 
			{
				if (!Cl.Approx(c, 0.0)) 
				{
					_terms.Add(v, new ClDouble(c));
				}
			}
			
			return this;
		}

		public /*sealed*/ ClLinearExpression AddVariable(ClAbstractVariable v)
		{ 
			return AddVariable(v, 1.0); 
		}


		public /*sealed*/ ClLinearExpression SetVariable(ClAbstractVariable v, double c)
		{ 
			// Assert(c != 0.0);
			ClDouble coeff = (ClDouble) _terms[v];
			
			if (coeff != null) 
				coeff.Value = c;
			else
				_terms.Add(v, new ClDouble(c)); 
			
			return this;
		}

		/// <summary>
		/// Add a term c*v to this expression.  If the expression already
		/// contains a term involving v, add c to the existing coefficient.
		/// If the new coefficient is approximately 0, delete v.  Notify the
		/// solver if v appears or disappears from this expression.
		/// </summary>
		public /*sealed*/ ClLinearExpression AddVariable(ClAbstractVariable v, double c,
			ClAbstractVariable subject, ClTableau solver)
		{ 
			// body largely duplicated above
			if (cTraceOn) 
				FnEnterPrint(string.Format("AddVariable: {0}, {1}, {2}, ...", v, c, subject));

			ClDouble coeff = (ClDouble) _terms[v];
			
			if (coeff != null) 
			{
				double new_coefficient = coeff.Value + c;
				
				if (Cl.Approx(new_coefficient, 0.0)) 
				{
					solver.NoteRemovedVariable(v, subject);
					_terms.Remove(v);
				} 
				else 
				{ 
					coeff.Value = new_coefficient;
				}
			} 
			else 
			{
				if (!Cl.Approx(c, 0.0)) 
				{
					_terms.Add(v, new ClDouble(c));
					solver.NoteAddedVariable(v, subject);
				}
			}

			return this;
		}

		/// <summary>
		/// Return a pivotable variable in this expression.  (It is an error
		/// if this expression is constant -- signal ExCLInternalError in
		/// that case).  Return null if no pivotable variables
		/// </summary>
		public /*sealed*/ ClAbstractVariable AnyPivotableVariable() /*throws ExCLInternalError*/
		{
			if (IsConstant)
			{
				throw new ExClInternalError("anyPivotableVariable called on a constant");
			}

			foreach (ClAbstractVariable clv in _terms.Keys)
			{
				if (clv.IsPivotable)
					return clv;
			} 

			// No pivotable variables, so just return null, and let the caller
			// error if needed
			return null;
		}

		/// <summary>
		/// Replace var with a symbolic expression expr that is equal to it.
		/// If a variable has been added to this expression that wasn't there
		/// before, or if a variable has been dropped from this expression
		/// because it now has a coefficient of 0, inform the solver.
		/// PRECONDITIONS:
		///   var occurs with a non-zero coefficient in this expression.
		/// </summary>
		public /*sealed*/ void SubstituteOut(ClAbstractVariable var, ClLinearExpression expr, 
			ClAbstractVariable subject, ClTableau solver)
		{
			if (cTraceOn) 
				FnEnterPrint(string.Format("CLE:SubstituteOut: {0}, {1}, {2}, ...", var, expr, subject));
			if (cTraceOn) 
				TracePrint("this = " + this);

			double multiplier = ((ClDouble) _terms[var]).Value;
			 _terms.Remove(var);
			IncrementConstant(multiplier * expr.Constant);
		    
			foreach (ClAbstractVariable clv in expr.Terms.Keys)
			{
				double coeff = ((ClDouble) expr.Terms[clv]).Value;
				ClDouble d_old_coeff = (ClDouble) _terms[clv];
				
				if (d_old_coeff != null) 
				{
					double old_coeff = d_old_coeff.Value;
					double newCoeff = old_coeff + multiplier * coeff;
					
					if (Cl.Approx(newCoeff, 0.0)) 
					{
						solver.NoteRemovedVariable(clv, subject);
						_terms.Remove(clv);
					} 
					else 
					{
						d_old_coeff.Value = newCoeff;
					}
				} 
				else 
				{
					// did not have that variable already
					_terms.Add(clv, new ClDouble(multiplier * coeff));
					solver.NoteAddedVariable(clv, subject);
				}
			}

			if (cTraceOn) 
				TracePrint("Now this is " + this);
		}

		/// <summary>
		/// This linear expression currently represents the equation
		/// oldSubject=self.  Destructively modify it so that it represents
		/// the equation newSubject=self.
		///
		/// Precondition: newSubject currently has a nonzero coefficient in
		/// this expression.
		///
		/// NOTES
		///   Suppose this expression is c + a*newSubject + a1*v1 + ... + an*vn.
		///
		///   Then the current equation is 
		///       oldSubject = c + a*newSubject + a1*v1 + ... + an*vn.
		///   The new equation will be
		///        newSubject = -c/a + oldSubject/a - (a1/a)*v1 - ... - (an/a)*vn.
		///   Note that the term involving newSubject has been dropped.
		/// </summary>
		public /*sealed*/ void ChangeSubject(ClAbstractVariable old_subject, ClAbstractVariable new_subject)
		{
			ClDouble cld = (ClDouble) _terms[old_subject];
			
			if (cld != null)
				cld.Value  = NewSubject(new_subject);
			else
				_terms.Add(old_subject, new ClDouble(NewSubject(new_subject)));
		}
  
		/// <summary>
		/// This linear expression currently represents the equation self=0.  Destructively modify it so 
		/// that subject=self represents an equivalent equation.  
		///
		/// Precondition: subject must be one of the variables in this expression.
		/// NOTES
		///   Suppose this expression is
		///     c + a*subject + a1*v1 + ... + an*vn
		///   representing 
		///     c + a*subject + a1*v1 + ... + an*vn = 0
		/// The modified expression will be
		///    subject = -c/a - (a1/a)*v1 - ... - (an/a)*vn
		///   representing
		///    subject = -c/a - (a1/a)*v1 - ... - (an/a)*vn
		///
		/// Note that the term involving subject has been dropped.
		/// Returns the reciprocal, so changeSubject can use it, too
		/// </summary>
		public /*sealed*/ double NewSubject(ClAbstractVariable subject)
		{
			if (cTraceOn) 
				FnEnterPrint(string.Format("newSubject: {0}", subject));
			
			ClDouble coeff = (ClDouble) _terms[subject];
			_terms.Remove(subject);
			
			double reciprocal = 1.0 / coeff.Value;
			MultiplyMe(-reciprocal);
			
			return reciprocal;
		}

		/// <summary>
		/// Return the coefficient corresponding to variable var, i.e.,
		/// the 'ci' corresponding to the 'vi' that var is:
		///      v1*c1 + v2*c2 + .. + vn*cn + c
		/// </summary>
		public /*sealed*/ double CoefficientFor(ClAbstractVariable var)
		{ 
			ClDouble coeff = (ClDouble) _terms[var];
			
			if (coeff != null)
				return coeff.Value;
			else
				return 0.0;
		}

		public double Constant
		{
			get {
				return _constant.Value; 
			}
			set {
				_constant.Value = value;
			}
		}

		public Hashtable Terms
		{
			get {
				return _terms; 
			}
		}

		public /*sealed*/ void IncrementConstant(double c)
		{ 
			_constant.Value = _constant.Value + c;
		}

		public bool IsConstant
		{
			get {
				return _terms.Count == 0;
			}
		}

		public override string ToString()
		{
			String s = "";
			
			IDictionaryEnumerator e = _terms.GetEnumerator();

			if (!Cl.Approx(_constant.Value, 0.0) || _terms.Count == 0) 
			{
				s += _constant.ToString();
			}
			else
			{
				if (_terms.Count == 0)
				{
					return s;
				}
				e.MoveNext(); // go to first element
				ClAbstractVariable clv = (ClAbstractVariable) e.Key;
				ClDouble coeff = (ClDouble) _terms[clv];
				s += string.Format("{0}*{1}", coeff.ToString(), clv.ToString());
			}
			while (e.MoveNext())
			{
				ClAbstractVariable clv = (ClAbstractVariable) e.Key;
				ClDouble coeff = (ClDouble) _terms[clv];
				s += string.Format(" + {0}*{1}", coeff.ToString(), clv.ToString());
			}
			
			return s;
		}

		public /*sealed*/ new static ClLinearExpression Plus(ClLinearExpression e1, ClLinearExpression e2)
		{ 
			return e1.Plus(e2); 
		}

		public /*sealed*/ new static ClLinearExpression Minus(ClLinearExpression e1, ClLinearExpression e2)
		{ 
			return e1.Minus(e2); 
		}

		public /*sealed*/ new static ClLinearExpression Times(ClLinearExpression e1, ClLinearExpression e2) /* throws ExCLNonlinearExpression */
		{ 
			return e1.Times(e2); 
		}

		public /*sealed*/ new static ClLinearExpression Divide(ClLinearExpression e1, ClLinearExpression e2) /* throws ExCLNonlinearExpression */
		{ 
			return e1.Divide(e2);
		}

		public /*sealed*/ static bool FEquals(ClLinearExpression e1, ClLinearExpression e2)
		{ 
			return e1 == e2;
		}

		private ClDouble _constant;
		private Hashtable _terms; // from ClVariable to ClDouble
	}
}
