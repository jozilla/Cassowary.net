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
			} catch (ExClRequiredFailure rf)
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
			try
			{
				foreach (ClVariable v in _editVarMap.Keys)
				{
					ClEditInfo cei = (ClEditInfo) _editVarMap[v];
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
		/// resolve(Vector...)] interface), and the previous value.
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
