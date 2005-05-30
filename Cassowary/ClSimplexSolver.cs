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

using Cassowary.Utils;

namespace Cassowary
{
	public class ClSimplexSolver : ClTableau
	{
		/// <remarks>
		/// Constructor initializes the fields, and creaties the objective row.
		/// </remarks>
		public ClSimplexSolver()
		{	
			_stayMinusErrorVars = new ArrayList();
			_stayPlusErrorVars = new ArrayList();
			_errorVars = new Hashtable();
			_markerVars = new Hashtable();

			_resolve_pair = new ArrayList(2);
			_resolve_pair.Add(new ClDouble(0));
			_resolve_pair.Add(new ClDouble(0));

			_objective = new ClObjectiveVariable("Z");

			_editVarMap = new Hashtable();

			_slackCounter = 0;
			_artificialCounter = 0;
			_dummyCounter = 0;
			_epsilon = 1e-8;

			_cOptimizeAutomatically = true;
			_cNeedsSolving = false;

			ClLinearExpression e = new ClLinearExpression();
			_rows.Add(_objective, e);
			_stkCedcns = new Stack();
			_stkCedcns.Push(0);

			if (cTraceOn)
				TracePrint("objective expr == " + RowExpression(_objective));
		}

		/// <summary>
		/// Convenience function for creating a linear inequality constraint.
		/// </summary>
		public ClSimplexSolver AddLowerBound(ClAbstractVariable v, double lower)
			/* throws ExClRequiredFailure, ExClInternalError */
		{
			ClLinearInequality cn = new ClLinearInequality(v, Cl.GEQ, new ClLinearExpression(lower));
			return AddConstraint(cn);
		}

		/// <summary>
		/// Convenience function for creating a linear inequality constraint.
		/// </summary>
		public ClSimplexSolver AddUpperBound(ClAbstractVariable v, double upper)
			/* throws ExClRequiredFailure, ExClInternalError */
		{
			ClLinearInequality cn = new ClLinearInequality(v, Cl.LEQ, new ClLinearExpression(upper));
			return AddConstraint(cn);
		}

		/// <summary>
		/// Convenience function for creating a pair of linear inequality constraints.
		/// </summary>
		public ClSimplexSolver AddBounds(ClAbstractVariable v, double lower, double upper)
			/* throws ExClRequiredFailure, ExClInternalError */
		{
			AddLowerBound(v, lower);
			AddUpperBound(v, upper);
			return this;
		}

		/// <summary>
		/// Add a constraint to the solver.
		/// <param name="cn">
		/// The constraint to be added.
		/// </param>
		/// </summary>
		public ClSimplexSolver AddConstraint(ClConstraint cn)
			/* throws ExClRequiredFailure, ExClInternalError */
		{
			if (cTraceOn)
				FnEnterPrint("AddConstraint: " + cn);

			ArrayList eplus_eminus = new ArrayList(2);
			ClDouble prevEConstant = new ClDouble();
			ClLinearExpression expr = NewExpression(cn, /* output to: */
																							eplus_eminus,
																							prevEConstant);

			bool cAddedOkDirectly = false;

			try
			{
				cAddedOkDirectly = TryAddingDirectly(expr);
				if (!cAddedOkDirectly)
				{
					// could not add directly
					AddWithArtificialVariable(expr);
				}
			} 
			catch (ExClRequiredFailure rf)
			{
				throw rf;
			}

			_cNeedsSolving = true;

			if (cn.IsEditConstraint)
			{
				int i = _editVarMap.Count;
				ClEditConstraint cnEdit = (ClEditConstraint) cn;
				ClSlackVariable clvEplus = (ClSlackVariable) eplus_eminus[0];
				ClSlackVariable clvEminus = (ClSlackVariable) eplus_eminus[1];
				_editVarMap.Add(cnEdit.Variable,
												new ClEditInfo(cnEdit, clvEplus, clvEminus,
																			 prevEConstant.Value,
																			 i));
			}

			if (_cOptimizeAutomatically)
			{
				Optimize(_objective);
				SetExternalVariables();
			}

			return this;
		}

		/// <summary>
		/// Same as AddConstraint, throws no exceptions.
		/// <returns>
		/// False if the constraint resulted in an unsolvable system, otherwise true.
		/// </returns>
		/// </summary>
		public bool AddConstraintNoException(ClConstraint cn)
		{
			if (cTraceOn)
				FnEnterPrint("AddConstraintNoException: " + cn);

			try
			{
				AddConstraint(cn);
				return true;
			} catch (ExClRequiredFailure rf)
			{
				return false;
			}
		}
		
		/// <summary>
		/// Add an edit constraint for a variable with a given strength.
		/// <param name="v">Variable to add an edit constraint to.</param>
		/// <param name="strength">Strength of the edit constraint.</param>
		/// </summary>
		public ClSimplexSolver AddEditVar(ClVariable v, ClStrength strength)
			/* throws ExClInternalError */
		{
			try 
			{
				ClEditConstraint cnEdit = new ClEditConstraint(v, strength);
				return AddConstraint(cnEdit);
			}
			catch (ExClRequiredFailure rf)
			{
				// should not get this
				throw new ExClInternalError("Required failure when adding an edit variable");
			}
		}

		/// <remarks>
		/// Add an edit constraint with strength ClStrength#Strong.
		/// </remarks>
		public ClSimplexSolver AddEditVar(ClVariable v)
		{
			/* throws ExClInternalError */
			return AddEditVar(v, ClStrength.Strong);
		}

		/// <summary>
		/// Remove the edit constraint previously added.
		/// <param name="v">Variable to which the edit constraint was added before.</param>
		/// </summary>
		public ClSimplexSolver RemoveEditVar(ClVariable v)
			/* throws ExClInternalError, ExClConstraintNotFound */
		{
			ClEditInfo cei = (ClEditInfo) _editVarMap[v];
			ClConstraint cn = cei.Constraint;
			RemoveConstraint(cn);
		
			return this;
		}

		/// <summary>
		/// Marks the start of an edit session.
		/// </summary>
		/// <remarks>
		/// BeginEdit should be called before sending Resolve()
		/// messages, after adding the appropriate edit variables.
		/// </remarks>
		public ClSimplexSolver BeginEdit()
			/* throws ExClInternalError */
		{
			Assert(_editVarMap.Count > 0, "_editVarMap.Count > 0");
			// may later want to do more in here
			_infeasibleRows.Clear();
			ResetStayConstants();
			_stkCedcns.Push(_editVarMap.Count);

			return this;
		}

		/// <summary>
		/// Marks the end of an edit session.
		/// </summary>
		/// <remarks>
		/// EndEdit should be called after editing has finished for now, it
		/// just removes all edit variables.
		/// </remarks>
		public ClSimplexSolver EndEdit()
			/* throws ExClInternalError */
		{
			Assert(_editVarMap.Count > 0, "_editVarMap.Count > 0");
			Resolve();
			_stkCedcns.Pop();
			int n = (int) _stkCedcns.Peek();
			RemoveEditVarsTo(n);
			// may later want to do more in hore
			
			return this;
		}

		/// <summary>
		/// Eliminates all the edit constraints that were added.
		/// </summary>
		public ClSimplexSolver RemoveAllEditVars(int n)
			/* throws ExClInternalError */
		{
			return RemoveEditVarsTo(0);
		}

		/// <summary>
		/// Remove the last added edit vars to leave only
		/// a specific number left.
		/// <param name="n">
		/// Number of edit variables to keep.
		/// </param>
		/// </summary>
		public ClSimplexSolver RemoveEditVarsTo(int n)
			/* throws ExClInternalError */
		{
			// HACK: to be able to remove elements from _editVarMap,
			// which will be done by RemoveEditVar(...).
			//
			// C# enumerators do not allow this, because they
			// only take a snapshot of the collection. If the collection
			// changes (by removing an element for example), the
			// enumerator gets out of sync, and an 
			// InvalidOperationException will be thrown. 
			// 
			// We thus create a copy of the _editVarMap to iterate
			// through.
			Hashtable editVarMapCopy = (Hashtable) _editVarMap.Clone();
			
			try
			{
				foreach (ClVariable v in editVarMapCopy.Keys)
				{
					ClEditInfo cei = (ClEditInfo) editVarMapCopy[v];
					if (cei.Index >= n)
					{
						RemoveEditVar(v);
					}
				}
				Assert(_editVarMap.Count == n, "_editVarMap.Count == n");

				return this;
			} 
			catch (ExClConstraintNotFound cnf)
			{
				// should not get this
				throw new ExClInternalError("Constraint not found in RemoveEditVarsTo");
			}
		}

		/// <summary>
		/// Add weak stays to the x and y parts of each point. These
		/// have increasing weights so that the solver will try to satisfy
		/// the x and y stays on the same point, rather than the x stay on
		/// one and the y stay on another.
		/// <param name="listOfPoints">
		/// List of points to add weak stay constraints for.
		/// </param>
		/// </summary>
		public ClSimplexSolver AddPointStays(ArrayList listOfPoints)
			/* throws ExClRequiredFailure, ExClInternalError */
		{
			if (cTraceOn)
				FnEnterPrint("AddPointStays: " + listOfPoints);
			
			double weight = 1.0;
			const double multiplier = 2.0;

			foreach (ClPoint p in listOfPoints)
			{
				AddPointStay(p, weight);
				weight *= multiplier;
			}

			return this;
		}

		public ClSimplexSolver AddPointStay(ClVariable vx,
																				ClVariable vy,
																				double weight)
			/* throws ExClRequiredFailure, ExClInternalError */
		{
			AddStay(vx, ClStrength.Weak, weight);
			AddStay(vy, ClStrength.Weak, weight);
			
			return this;
		}

		public ClSimplexSolver AddPointStay(ClVariable vx,
																				ClVariable vy)
			/* throws ExClRequiredFailure, ExClInternalError */
		{
			AddPointStay(vx, vy, 1.0);

			return this;
		}

		public ClSimplexSolver AddPointStay(ClPoint clp, double weight)
			/* throws ExClRequiredFailure, ExClInternalError */
		{
			AddStay(clp.X, ClStrength.Weak, weight);
			AddStay(clp.Y, ClStrength.Weak, weight);

			return this;
		}

		public ClSimplexSolver AddPointStay(ClPoint clp)
			/* throws ExClRequiredFailure, ExClInternalError */
		{
			AddPointStay(clp, 1.0);

			return this;
		}

		/// <summary>
		/// Add a stay of the given strength (default to ClStrength#Weak)
		/// of a variable to the tableau..
		/// <param name="v">
		/// Variable to add the stay constraint to.
		/// </param>
		/// </summary>
		public ClSimplexSolver AddStay(ClVariable v,
																	 ClStrength strength,
																	 double weight)
			/* throws ExClRequiredFailure, ExClInternalError */
		{
			ClStayConstraint cn = new ClStayConstraint(v, strength, weight);
			
			return AddConstraint(cn);
		}

		/// <remarks>
		/// Default to weight 1.0.
		/// </remarks>
		public ClSimplexSolver AddStay(ClVariable v,
																	 ClStrength strength)
			/* throws ExClRequiredFailure, ExClInternalError */
		{
			AddStay(v, strength, 1.0);

			return this;
		}
		
		/// <remarks>
		/// Default to strength ClStrength#Weak.
		/// </remarks>
		public ClSimplexSolver AddStay(ClVariable v)
			/* throws ExClRequiredFailure, ExClInternalError */
		{
			AddStay(v, ClStrength.Weak, 1.0);

			return this;
		}

		/// <summary>
		/// Remove a constraint from the tableau.
		/// Also remove any error variable associated with it.
		/// </summary>
		public ClSimplexSolver RemoveConstraint(ClConstraint cn)
			/* throws ExClRequiredFailure, ExClInternalError */
		{
			if (cTraceOn)
			{
				FnEnterPrint("RemoveConstraint: " + cn);
				TracePrint(this.ToString());
			}

			_cNeedsSolving = true;

			ResetStayConstants();

			ClLinearExpression zRow = RowExpression(_objective);

			Set eVars = (Set) _errorVars[cn];
			if (cTraceOn)
				TracePrint("eVars == " + eVars);

			if (eVars != null)
			{
				foreach (ClAbstractVariable clv in eVars)
				{
					ClLinearExpression expr = RowExpression(clv);
					if (expr == null)
					{
						zRow.AddVariable(clv, -cn.Weight *
														 cn.Strength.SymbolicWeight.AsDouble(),
														 _objective, this);
					}
					else // the error variable was in the basis
					{
						zRow.AddExpression(expr, -cn.Weight *
															 cn.Strength.SymbolicWeight.AsDouble(),
															 _objective, this);
					}
				}
			}

			int markerVarsCount = _markerVars.Count;
			ClAbstractVariable marker = (ClAbstractVariable) _markerVars[cn];
			_markerVars.Remove(cn);
			
			if (markerVarsCount == _markerVars.Count) // key was not found
			{
				throw new ExClConstraintNotFound();
			}

			if (cTraceOn)
				TracePrint("Looking to remove var " + marker);

			if (RowExpression(marker) == null)
			{
				// not in the basis, so need to do some more work
				Set col = (Set) _columns[marker];

				if (cTraceOn)
					TracePrint("Must pivot -- columns are " + col);

				ClAbstractVariable exitVar = null;
				double minRatio = 0.0;
				foreach (ClAbstractVariable v in col)
				{
					if (v.IsRestricted)
					{
						ClLinearExpression expr = RowExpression(v);
						double coeff = expr.CoefficientFor(marker);
						
						if (cTraceOn)
							TracePrint("Marker " + marker + "'s coefficient in " + expr + " is " + coeff);

						if (coeff < 0.0)
						{
							double r = -expr.Constant / coeff;
							if (exitVar == null || r < minRatio) 
							{
								minRatio = r;
								exitVar = v;
							}
						}
					}
				}

				if (exitVar == null)
				{
					if (cTraceOn)
						TracePrint("exitVar is still null");
					
					foreach (ClAbstractVariable v in col)
					{
						if (v.IsRestricted)
						{
							ClLinearExpression expr = RowExpression(v);
							double coeff = expr.CoefficientFor(marker);
							double r = expr.Constant / coeff;
							if (exitVar == null || r < minRatio)
							{
								minRatio = r;
								exitVar = v;
							}
						}
					}
				}

				if (exitVar == null)
				{
					// exitVar is still null
					if (col.Count == 0)
					{
						RemoveColumn(marker);
					}
					else
					{
						// put first element in exitVar
						IEnumerator colEnum = col.GetEnumerator();
						colEnum.MoveNext();
						exitVar = (ClAbstractVariable) colEnum.Current; 
					}
				}

				if (exitVar != null)
				{
					Pivot(marker, exitVar);
				}
			}

			if (RowExpression(marker) != null)
			{
				RemoveRow(marker);
			}

			if (eVars != null)
			{
				foreach (ClAbstractVariable v in eVars)
				{
					// FIXME: decide wether to use equals or !=
					if (v != marker)
					{
						RemoveColumn(v);
						// v = null; // is read-only, cannot be set to null
					}
				}
			}

			if (cn.IsStayConstraint)
			{
				if (eVars != null)
				{
					for (int i = 0; i < _stayPlusErrorVars.Count; i++)
					{
						eVars.Remove(_stayPlusErrorVars[i]);
						eVars.Remove(_stayMinusErrorVars[i]);
					}
				}
			}
			else if (cn.IsEditConstraint)
			{
				Assert(eVars != null, "eVars != null");
				ClEditConstraint cnEdit = (ClEditConstraint) cn;
				ClVariable clv = cnEdit.Variable;
				ClEditInfo cei = (ClEditInfo) _editVarMap[clv];
				ClSlackVariable clvEditMinus = cei.ClvEditMinus;
				RemoveColumn(clvEditMinus);
				_editVarMap.Remove(clv);
			}

			// FIXME: do the remove at top
			if (eVars != null)
			{
				_errorVars.Remove(eVars);
			}
			marker = null;

			if (_cOptimizeAutomatically)
			{
				Optimize(_objective);
				SetExternalVariables();
			}

			return this;
		}

		/// <summary>
		/// Re-initialize this solver from the original constraints, thus
		/// getting rid of any accumulated numerical problems 
		/// </summary>
		/// <remarks>
		/// Actually, we haven't definitely observed any such problems yet.
		/// </remarks>
		public void Reset()
			/* throws ExClInternalError */
		{
			if (cTraceOn)
				FnEnterPrint("Reset");
			throw new ExClInternalError("Reset not implemented");
		}

		/// <summary>
		/// Re-solve the current collection of constraints for new values
		/// for the constants of the edit variables.
		/// </summary>
		/// <remarks>
		/// Deprecated. Use SuggestValue(...) then Resolve(). If you must
		/// use this, be sure to not use it if you
		/// remove an edit variable (or edit constraints) from the middle
		/// of a list of edits, and then try to resolve with this function
		/// (you'll get the wrong answer, because the indices will be wrong
		/// in the ClEditInfo objects).
		/// </remarks>
		public void Resolve(ArrayList newEditConstants)
			/* throws ExClInternalError */
		{
			if (cTraceOn)
				FnEnterPrint("Resolve " + newEditConstants);
			
			foreach (ClVariable v in _editVarMap.Keys)
			{
				ClEditInfo cei = (ClEditInfo) _editVarMap[v];
				int i = cei.Index;
				try
				{
					if (i < newEditConstants.Count)
					{
						SuggestValue(v, ((ClDouble) newEditConstants[i]).Value);
					}
				} catch (ExClError e)
				{
					throw new ExClInternalError("Error during resolve");
				}
			}
			Resolve();
		}

		/// <summary>
		/// Convenience function for resolve-s of two variables.
		/// </summary>
		public void Resolve(double x, double y)
			/* throws ExClInternalError */
		{
			((ClDouble) _resolve_pair[0]).Value = x;
			((ClDouble) _resolve_pair[1]).Value = y;

			Resolve(_resolve_pair);
		}

		/// <summary>
		/// Re-solve the current collection of constraints, given the new
		/// values for the edit variables that have already been
		/// suggested (see <see cref="SuggestValue"/> method).
		/// </summary>
		public void Resolve()
			/* throws ExClInternalError */
		{
			if (cTraceOn)
				FnEnterPrint("Resolve()");

			DualOptimize();
			SetExternalVariables();
			_infeasibleRows.Clear();
			ResetStayConstants();
		}

		/// <summary>
		/// Suggest a new value for an edit variable. 
		/// </summary>
		/// <remarks>
		/// The variable needs to be added as an edit variable and 
		/// BeginEdit() needs to be called before this is called.
		/// The tableau will not be solved completely until after Resolve()
		/// has been called.
		/// </remarks>
		public ClSimplexSolver SuggestValue(ClVariable v, double x)
			/* throws ExClError */
		{
			if (cTraceOn)
				FnEnterPrint("SuggestValue(" + v + ", " + x + ")");

			ClEditInfo cei = (ClEditInfo) _editVarMap[v];
			if (cei == null)
			{
				Console.Error.WriteLine("SuggestValue for variable " + v + ", but var is not an edit variable\n");
				throw new ExClError();
			}
			int i = cei.Index;
			ClSlackVariable clvEditPlus = cei.ClvEditPlus;
			ClSlackVariable clvEditMinus = cei.ClvEditMinus;
			double delta = x - cei.PrevEditConstant;
			cei.PrevEditConstant = x;
			DeltaEditConstant(delta, clvEditPlus, clvEditMinus);
			
			return this;
		}

		/// <summary>
		/// Controls wether optimization and setting of external variables is done
		/// automatically or not.
		/// </summary>
		/// <remarks>
		/// By default it is done automatically and <see cref="Solve"/> never needs
		/// to be explicitly called by client code. If <see cref="AutoSolve"/> is
		/// put to false, then <see cref="Solve"/> needs to be invoked explicitly
		/// before using variables' values. 
		/// (Turning off <see cref="AutoSolve"/> while addings lots and lots
		/// of constraints [ala the AddDel test in ClTests] saved about 20 % in
		/// runtime, from 60sec to 54sec for 900 constraints, with 126 failed adds).
		/// </remarks>
		public bool AutoSolve
		{
			get { return _cOptimizeAutomatically; }
			set { _cOptimizeAutomatically = value; }
		}

		public ClSimplexSolver Solve()
			/* throws ExClInternalError */
		{
			if (_cNeedsSolving)
			{
				Optimize(_objective);
				SetExternalVariables();
			}
			
			return this;
		}

		public ClSimplexSolver SetEditedValue(ClVariable v, double n)
			/* throws ExClInternalError */
		{
			if (!ContainsVariable(v))
			{
				v.ChangeValue(n);
				return this;
			}

			if (!Cl.Approx(n, v.Value))
			{
				AddEditVar(v);
				BeginEdit();
				try
				{
					SuggestValue(v, n);
				}
				catch(ExClError e)
				{
					// just added it above, so we shouldn't get an error
					throw new ExClInternalError("Error in SetEditedValue");
				}
				EndEdit();
			}
			
			return this;
		}

		public bool ContainsVariable(ClVariable v)
			/* throws ExClInternalError */
		{
			return ColumnsHasKey(v) || (RowExpression(v) != null);
		}

		public ClSimplexSolver AddVar(ClVariable v)
			/* throws ExClInternalError */
		{
			if (!ContainsVariable(v))
			{
				try 
				{
					AddStay(v);
				}
				catch(ExClRequiredFailure rf)
				{
					// cannot have a required failure, since we add w/ weak
					throw new ExClInternalError("Error in AddVar -- required failure is impossible");
				}
				
				if (cTraceOn)
					TracePrint("added initial stay on " + v);
			}

			return this;
		}

		/// <summary>
		/// Returns information about the solver's internals.
		/// </summary>
		/// <remarks>
		/// Originally from Michael Noth <noth@cs.washington.edu>
		/// </remarks>
		/// <returns>
		/// String containing the information.
		/// </returns>
		public override string GetInternalInfo()
		{
			string result = base.GetInternalInfo();

			result += "\nSolver info:\n";
			result += "Stay Error Variables: ";
			result += _stayPlusErrorVars.Count + _stayMinusErrorVars.Count;
			result += " (" +_stayPlusErrorVars.Count + " +, ";
			result += _stayMinusErrorVars.Count + " -)\n";
			result += "Edit Variables: " + _editVarMap.Count;
			result += "\n";
			
			return result;
		}

		public string GetDebugInfo()
		{
			string result = ToString();
			result += GetInternalInfo();
			result += "\n";

			return result;
		}

		public override string ToString()
		{
			string result = base.ToString();

			result += "\n_stayPlusErrorVars: ";
			result += _stayPlusErrorVars;
			result += "\n_stayMinusErrorVars: ";
			result += _stayMinusErrorVars;
			result += "\n";

			return result;
		}

		public Hashtable ConstraintMap
		{
			get { return _markerVars; }
		}

		//// END PUBLIC INTERFACE ////
		
		/// <summary>
		/// Add the constraint expr=0 to the inequality tableau using an
		/// artificial variable.
		/// </summary>
		/// <remarks>
		/// To do this, create an artificial variable av and add av=expr
		/// to the inequality tableau, then make av be 0 (raise an exception
		/// if we can't attain av=0).
		/// </remarks>
		protected void AddWithArtificialVariable(ClLinearExpression expr)
			/* throws ExClRequiredFailure, ExClInternalError */
		{
			if (cTraceOn)
				FnEnterPrint("AddWithArtificialVariable: " + expr);

			ClSlackVariable av = new ClSlackVariable(++_artificialCounter, "a");
			ClObjectiveVariable az = new ClObjectiveVariable("az");
			ClLinearExpression azRow = (ClLinearExpression) expr.Clone();
			
			if (cTraceOn)
				TracePrint("before AddRows:\n" + this);

			AddRow(az, azRow);
			AddRow(av, expr);

			if (cTraceOn)
				TracePrint("after AddRows:\n" + this);
			
			Optimize(az);

			ClLinearExpression azTableauRow = RowExpression(az);

			if (cTraceOn)
				TracePrint("azTableauRow.Constant == " + azTableauRow.Constant);

			if (!Cl.Approx(azTableauRow.Constant, 0.0))
			{
				RemoveRow(az);
				RemoveColumn(av);
				throw new ExClRequiredFailure();
			}

			// see if av is a basic variable
			ClLinearExpression e = RowExpression(av);

			if (e != null)
			{
				// find another variable in this row and pivot,
				// so that av becomes parametric
				if (e.IsConstant)
				{
					// if there isn't another variable in the row
					// then the tableau contains the equation av=0 --
					// just delete av's row
					RemoveRow(av);
					RemoveRow(az);
					return;
				}
				ClAbstractVariable entryVar = e.AnyPivotableVariable();
				Pivot(entryVar, av);
			}
			Assert(RowExpression(av) == null, "RowExpression(av) == null)");
			RemoveColumn(av);
			RemoveRow(az);
		}

		/// <summary>
		/// Try to add expr directly to the tableau without creating an
		/// artificial variable.
		/// </summary>
		/// <remarks>
		/// We are trying to add the constraint expr=0 to the appropriate
		/// tableau.
		/// </remarks>
		/// <returns>
		/// True if successful and false if not.
		/// </returns>
		protected bool TryAddingDirectly(ClLinearExpression expr)
			/* throws ExClRequiredFailure */
		{
			if (cTraceOn)
				FnEnterPrint("TryAddingDirectly: " + expr);

			ClAbstractVariable subject = ChooseSubject(expr);
			if (subject == null)
			{
				if (cTraceOn)
					FnExitPrint("returning false");
				return false;
			}
			expr.NewSubject(subject);
			if (ColumnsHasKey(subject))
			{
				SubstituteOut(subject, expr);
			}
			AddRow(subject, expr);
			if (cTraceOn)
				FnExitPrint("returning true");
			return true; // succesfully added directly
		}

		/// <summary>
		///	Try to choose a subject (a variable to become basic) from
		///	among the current variables in expr.
		/// </summary>
		/// <remarks>
		/// We are trying to add the constraint expr=0 to the tableaux.
		/// If expr constains any unrestricted variables, then we must choose
		/// an unrestricted variable as the subject. Also if the subject is
		/// new to the solver, we won't have to do any substitutions, so we
		/// prefer new variables to ones that are currently noted as parametric.
		/// If expr contains only restricted variables, if there is a restricted
		/// variable with a negative coefficient that is new to the solver we can
		/// make that the subject. Otherwise we can't find a subject, so return nil.
		/// (In this last case we have to add an artificial variable and use that
		/// variable as the subject -- this is done outside this method though.)
		/// </remarks>
		protected ClAbstractVariable ChooseSubject(ClLinearExpression expr)
			/* ExClRequiredFailure */
		{
			if (cTraceOn)
				FnEnterPrint("ChooseSubject: " + expr);

			ClAbstractVariable subject = null; // the current best subject, if any

			bool foundUnrestricted = false;
			bool foundNewRestricted = false;

			Hashtable terms = expr.Terms;

			foreach (ClAbstractVariable v in terms.Keys)
			{
				double c = ((ClDouble) terms[v]).Value;

				if (foundUnrestricted)
				{
					if (!v.IsRestricted)
					{
						if (!ColumnsHasKey(v))
							return v;
					}
				}
				else
				{
					// we haven't found an restricted variable yet
					if (v.IsRestricted)
					{
						if (!foundNewRestricted && !v.IsDummy && c < 0.0)
						{
							Set col = (Set) _columns[v];

							if ( col == null ||
									 (col.Count == 1 && ColumnsHasKey(_objective)) ) 
							{
								subject = v;
								foundNewRestricted = true;
							}
						}
					}
					else
					{
						subject = v;
						foundUnrestricted = true;
					}
				}
			}

			if (subject != null)
				return subject;

			double coeff = 0.0;

			foreach (ClAbstractVariable v in terms.Keys)
			{
				double c = ((ClDouble) terms[v]).Value;
				
				if (!v.IsDummy)
					return null; // nope, no luck
				if (!ColumnsHasKey(v))
				{
					subject = v;
					coeff = c;
				}
			}

			if (!Cl.Approx(expr.Constant, 0.0))
			{
				throw new ExClRequiredFailure();
			}
			if (coeff > 0.0)
			{
				expr.MultiplyMe(-1);
			}
			
			return subject;
		}

		protected ClLinearExpression NewExpression(ClConstraint cn,
																							 ArrayList eplus_eminus,
																							 ClDouble prevEConstant)
		{
			if (cTraceOn)
			{
				FnEnterPrint("NewExpression: " + cn);
				TracePrint("cn.IsInequality == " + cn.IsInequality);
				TracePrint("cn.IsRequired == " + cn.IsRequired);
			}

			ClLinearExpression cnExpr = cn.Expression;
			ClLinearExpression expr = new ClLinearExpression(cnExpr.Constant);
			ClSlackVariable slackVar = new ClSlackVariable();
			ClDummyVariable dummyVar = new ClDummyVariable();
			ClSlackVariable eminus = new ClSlackVariable();
			ClSlackVariable eplus = new ClSlackVariable();
			Hashtable cnTerms = cnExpr.Terms;
			foreach(ClAbstractVariable v in cnTerms.Keys)
			{
				double c = ((ClDouble) cnTerms[v]).Value;
				ClLinearExpression e = RowExpression(v);
				if (e == null)
					expr.AddVariable(v, c);
				else
					expr.AddExpression(e, c);
			}

			if (cn.IsInequality)
			{
				++_slackCounter;
				slackVar = new ClSlackVariable (_slackCounter, "s");
				expr.SetVariable(slackVar, -1);
				_markerVars.Add(cn, slackVar);
				if (!cn.IsRequired)
				{
					++_slackCounter;
					eminus = new ClSlackVariable(_slackCounter, "em");
					expr.SetVariable(eminus, 1.0);
					ClLinearExpression zRow = RowExpression(_objective);
					ClSymbolicWeight sw = cn.Strength.SymbolicWeight.Times(cn.Weight);
					zRow.SetVariable(eminus, sw.AsDouble());
					InsertErrorVar(cn, eminus);
					NoteAddedVariable(eminus, _objective);
				}
			}
			else
			{
				// cn is an equality
				if (cn.IsRequired)
				{
					++_dummyCounter;
					dummyVar = new ClDummyVariable(_dummyCounter, "d");
					expr.SetVariable(dummyVar, 1.0);
					_markerVars.Add(cn, dummyVar);
					if (cTraceOn)
						TracePrint("Adding dummyVar == d" + _dummyCounter);
				}
				else
				{
					++_slackCounter;
					eplus = new ClSlackVariable(_slackCounter, "ep");
					eminus = new ClSlackVariable(_slackCounter, "em");

					expr.SetVariable(eplus, -1.0);
					expr.SetVariable(eminus, 1.0);
					_markerVars.Add(cn, eplus);
					ClLinearExpression zRow = RowExpression(_objective);
					ClSymbolicWeight sw = cn.Strength.SymbolicWeight.Times(cn.Weight);
					double swCoeff = sw.AsDouble();
					if (swCoeff == 0)
					{
						if (cTraceOn)
						{
							TracePrint("sw == " + sw);
							TracePrint("cn == " + cn);
							TracePrint("adding " + eplus + " and " + eminus + " with swCoeff == " + swCoeff);
						}
					}
					zRow.SetVariable(eplus, swCoeff);
					NoteAddedVariable(eplus, _objective);
					zRow.SetVariable(eminus, swCoeff);
					NoteAddedVariable(eminus, _objective);
					InsertErrorVar(cn, eminus);
					InsertErrorVar(cn, eplus);
					if (cn.IsStayConstraint)
					{
						_stayPlusErrorVars.Add(eplus);
						_stayMinusErrorVars.Add(eminus);
					}
					else if (cn.IsEditConstraint)
					{
						eplus_eminus.Add(eplus);
						eplus_eminus.Add(eminus);
						prevEConstant.Value = cnExpr.Constant;
					}
				}
			}

			if (expr.Constant < 0)
				expr.MultiplyMe(-1);

			if (cTraceOn)
				FnExitPrint("returning " + expr);

			return expr;
		}
		
		/// <summary>
		/// Minimize the value of the objective.
		/// </summary>
		/// <remarks>
		/// The tableau should already be feasible.
		/// </remarks>
		protected void Optimize(ClObjectiveVariable zVar)
			/* throws ExClInternalError */
		{
			if (cTraceOn) 
			{
				FnEnterPrint("Optimize: " + zVar);
				TracePrint(this.ToString());
			}

			ClLinearExpression zRow = RowExpression(zVar);
			Assert(zRow != null, "zRow != null");
			ClAbstractVariable entryVar = null;
			ClAbstractVariable exitVar = null;
			while (true)
			{
				double objectiveCoeff = 0;
				Hashtable terms = zRow.Terms;
				foreach (ClAbstractVariable v in terms.Keys)
				{
					double c = ((ClDouble) terms[v]).Value;
					if (v.IsPivotable && c < objectiveCoeff)
					{
						objectiveCoeff = c;
						entryVar = v;
					}
				}
				if (objectiveCoeff >= -_epsilon || entryVar == null)
					return;
				if (cTraceOn)
					TracePrint("entryVar == " + entryVar + ", objectiveCoeff == " + objectiveCoeff);

				double minRatio = Double.MaxValue;
				Set columnVars = (Set) _columns[entryVar];
				double r = 0.0;
				foreach (ClAbstractVariable v in columnVars)
				{
					if (cTraceOn)
						TracePrint("Checking " + v);
					if (v.IsPivotable)
					{
						ClLinearExpression expr = RowExpression(v);
						double coeff = expr.CoefficientFor(entryVar);
						if (cTraceOn)
							TracePrint("pivotable, coeff == " + coeff);
						if (coeff < 0.0)
						{
							r = - expr.Constant / coeff;
							if (r < minRatio)
							{
								if (cTraceOn)
									TracePrint("New minRatio == " + r);
								minRatio = r;
								exitVar = v;
							}
						}
					}
				}
				if (minRatio == Double.MaxValue)
				{
					throw new ExClInternalError("Objective function is unbounded in Optimize");
				}
				Pivot(entryVar, exitVar);
				if (cTraceOn)
					TracePrint(this.ToString());
			}
		}

		/// <summary>
		/// Fix the constants in the equations representing the edit constraints.
		/// </summary>
		/// <remarks>
		/// Each of the non-required edits will be represented by an equation
		/// of the form:
		/// 	v = c + eplus - eminus
		/// where v is the variable with the edit, c is the previous edit value,
		/// and eplus and eminus are slack variables that hold the error in 
		/// satisfying the edit constraint. We are about to change something,
		/// and we want to fix the constants in the equations representing
		/// the edit constraints. If one of eplus and eminus is basic, the other
		/// must occur only in the expression for that basic error variable. 
		/// (They can't both be basic.) Fix the constant in this expression.
		/// Otherwise they are both non-basic. Find all of the expressions
		/// in which they occur, and fix the constants in those. See the
		/// UIST paper for details.
		/// (This comment was for ResetEditConstants(), but that is now
		/// gone since it was part of the screwey vector-based interface
		/// to resolveing. --02/16/99 gjb)
		/// </remarks>
		protected void DeltaEditConstant(double delta,
																		 ClAbstractVariable plusErrorVar,
																		 ClAbstractVariable minusErrorVar)
		{
			if (cTraceOn)
				FnEnterPrint("DeltaEditConstant :" + delta + ", " 
						+ plusErrorVar + ", " + minusErrorVar);

			ClLinearExpression exprPlus = RowExpression(plusErrorVar);
			if (exprPlus != null)
			{
				exprPlus.IncrementConstant(delta);

				if (exprPlus.Constant < 0.0)
				{
					_infeasibleRows.Add(plusErrorVar);
				}
				return;
			}

			ClLinearExpression exprMinus = RowExpression(minusErrorVar);
			if (exprMinus != null)
			{
				exprMinus.IncrementConstant(-delta);
				if (exprMinus.Constant < 0.0)
				{
					_infeasibleRows.Add(minusErrorVar);
				}
				return;
			}

			Set columnVars = (Set) _columns[minusErrorVar];

			foreach (ClAbstractVariable basicVar in columnVars)
			{
				ClLinearExpression expr = RowExpression(basicVar);
				//Assert(expr != null, "expr != null");
				double c = expr.CoefficientFor(minusErrorVar);
				expr.IncrementConstant(c * delta);
				if (basicVar.IsRestricted && expr.Constant < 0.0)
				{
					_infeasibleRows.Add(basicVar);
				}
			}
		}

		/// <summary>
		/// Re-optimize using the dual simplex algorithm.
		/// </summary>
		/// <remarks>
		/// We have set new values for the constants in the edit constraints.
		/// </remarks>
		protected void DualOptimize()
			/* throws ExClInternalError */
		{
			if (cTraceOn)
				FnEnterPrint("DualOptimize: ");

			ClLinearExpression zRow = RowExpression(_objective);
			while (!_infeasibleRows.Empty)
			{
				IEnumerator enumIfRows = _infeasibleRows.GetEnumerator();
				enumIfRows.MoveNext();
				ClAbstractVariable exitVar = (ClAbstractVariable) enumIfRows.Current;

				_infeasibleRows.Remove(exitVar);
				ClAbstractVariable entryVar = null;
				ClLinearExpression expr = RowExpression(exitVar);
				if (expr != null)
				{
					if (expr.Constant < 0.0)
					{
						double ratio = Double.MaxValue;
						double r;
						Hashtable terms = expr.Terms;
						foreach (ClAbstractVariable v in terms.Keys)
						{
							double c = ((ClDouble) terms[v]).Value;
							if (c > 0.0 && v.IsPivotable)
							{
								double zc = zRow.CoefficientFor(v);
								r = zc / c; // FIXME: zc / c or zero, as ClSymbolicWeigth-s
								if (r < ratio)
								{
									entryVar = v;
									ratio = r;
								}
							}
						}
						if (ratio == Double.MaxValue)
						{
							throw new ExClInternalError("ratio == nil (Double.MaxValue) in DualOptimize");
						}
						Pivot(entryVar, exitVar);
					}
				}
			}
		}
		
		/// <summary>
		/// Do a pivot. Move entryVar into the basis and move exitVar 
		/// out of the basis.
		/// </summary>
		/// <remarks>
		/// We could for example make entryVar a basic variable and
		/// make exitVar a parametric variable.
		/// </remarks>
		protected void Pivot(ClAbstractVariable entryVar,
												 ClAbstractVariable exitVar)
			/* throws ExClInternalError */
		{
			if (cTraceOn)
				FnEnterPrint("Pivot: " + entryVar + ", " + exitVar);

			// the entryVar might be non-pivotable if we're doing a 
			// RemoveConstraint -- otherwise it should be a pivotable
			// variable -- enforced at call sites, hopefully

			ClLinearExpression pexpr = RemoveRow(exitVar);

			pexpr.ChangeSubject(exitVar, entryVar);
			SubstituteOut(entryVar, pexpr);
			AddRow(entryVar, pexpr);
		}
		
		/// <summary>
		/// Fix the constants in the equations representing the stays.
		/// </summary>
		/// <remarks>
		/// Each of the non-required stays will be represented by an equation
		/// of the form
		/// 	v = c + eplus - eminus
		/// where v is the variable with the stay, c is the previous value
		/// of v, and eplus and eminus are slack variables that hold the error
		/// in satisfying the stay constraint. We are about to change something,
		/// and we want to fix the constants in the equations representing the
		/// stays. If both eplus and eminus are nonbasic they have value 0
		/// in the current solution, meaning the previous stay was exactly
		/// satisfied. In this case nothing needs to be changed. Otherwise one
		/// of them is basic, and the other must occur only in the expression
		/// for that basic error variable. Reset the constant of this
		/// expression to 0.
		/// </remarks>
		protected void ResetStayConstants()
		{
			if (cTraceOn)
				FnEnterPrint("ResetStayConstants");

			for (int i = 0; i < _stayPlusErrorVars.Count; i++)
			{
				ClLinearExpression expr = 
					RowExpression((ClAbstractVariable) _stayPlusErrorVars[i]);
				if (expr == null)
					expr = RowExpression((ClAbstractVariable) _stayMinusErrorVars[i]);
				if (expr != null)
					expr.Constant = 0.0;
			}
		}
		
		/// <summary>
		/// Set the external variables known to this solver to their appropriate values.
		/// </summary>
		/// <remarks>
		/// Set each external basic variable to its value, and set each external parametric
		/// variable to 0. (It isn't clear that we will ever have external parametric
		/// variables -- every external variable should either have a stay on it, or have an
		/// equation that defines it in terms of other external variables that do have stays.
		/// For the moment I'll put this in though.) Variables that are internal to the solver
		/// don't actually store values -- their values are just implicit in the tableau -- so
		/// we don't need to set them.
		/// </remarks>
		protected void SetExternalVariables()
		{
			if (cTraceOn)
				FnEnterPrint("SetExternalVariables:");
			if (cTraceOn)
				TracePrint(this.ToString());

			foreach (ClVariable v in _externalParametricVars)
			{
				if (RowExpression(v) != null)
				{
					Console.Error.WriteLine("Error: variable " + v +
																	"in _externalParametricVars is basic");
					continue;
				}
				v.ChangeValue(0.0);
			}

			foreach (ClVariable v in _externalRows)
			{
				ClLinearExpression expr = RowExpression(v);
				if (cTraceOn)
					DebugPrint("v == " + v);
				if (cTraceOn)
					DebugPrint("expr == " + expr);
				v.ChangeValue(expr.Constant);
			}

			_cNeedsSolving = false;
		}
		
		/// <summary>
		/// Protected convenience function to insert an error variable
		/// into the _errorVars set, creating the mapping with Add as necessary.
		/// </summary>
		protected void InsertErrorVar(ClConstraint cn, ClAbstractVariable var)
		{
			if (cTraceOn)
				FnEnterPrint("InsertErrorVar: " + cn + ", " + var);

			Set cnset = (Set) _errorVars[cn];
			if (cnset == null)
				_errorVars.Add(cn, cnset = new Set());
			cnset.Add(var);
		}
		
		//// BEGIN PRIVATE INSTANCE FIELDS ////
		
		/// <summary>
		/// The array of negative error vars for the stay constraints
		/// (need both positive and negative since they have only non-negative
		/// values).
		/// </summary>
		private ArrayList _stayMinusErrorVars;

		/// <summary>
		/// The array of positive error vars for the stay constraints
		/// (need both positive and negative since they have only non-negative
		/// values).
		/// </summary>
		private ArrayList _stayPlusErrorVars;
		
		/// <summary>
		/// Give error variables for a non-required constraints,
		/// maps to ClSlackVariable-s.
		/// </summary>
		/// <remarks>
		/// Map ClConstraint to Set (of ClVariable).
		/// </remarks>
		private Hashtable _errorVars;
		
		/// <summary>
		/// Return a lookup table giving the marker variable for
		/// each constraints (used when deleting a constraint).
		/// </summary>
		/// <remarks>
		/// Map ClConstraint to ClVariable.
		/// </remarks>
		private Hashtable _markerVars;
		
		private ClObjectiveVariable _objective;
		
		/// <summary>
		/// Map edit variables to ClEditInfo-s.
		/// </summary>
		/// <remarks>
		/// ClEditInfo instances contain all the information for an
		/// edit constraints (the edit plus/minus vars, the index [for old-style
		/// resolve(ArrayList...)] interface), and the previous value.
		/// (ClEditInfo replaces the parallel vectors from the Smalltalk impl.)
		/// </remarks>
		private Hashtable _editVarMap;
		
		private long _slackCounter;
		private long _artificialCounter;
		private long _dummyCounter;
		
		private ArrayList _resolve_pair;
		
		private double _epsilon;
		
		private bool _cOptimizeAutomatically;
		private bool _cNeedsSolving;
			
		private Stack _stkCedcns;
	}
}
