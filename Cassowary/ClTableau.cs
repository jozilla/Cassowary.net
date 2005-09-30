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
using System.Collections;
using Cassowary.Utils;

namespace Cassowary
{
  public class ClTableau : Cl
  {
    /// <summary>
    /// Constructor is protected, since this only supports an ADT for
    /// the ClSimplexSolver class.
    /// </summary>
    protected ClTableau()
    {
      _columns = new Hashtable();
      _rows = new Hashtable();
      _infeasibleRows = new Set();
      _externalRows = new Set();
      _externalParametricVars = new Set();
    }
    
    /// <summary>
    /// Variable v has been removed from an expression. If the
    /// expression is in a tableau the corresponding basic variable is
    /// subject (or if subject is nil then it's in the objective function).
    /// Update the column cross-indices.
    /// </summary>
    public /*sealed*/ void NoteRemovedVariable(ClAbstractVariable v, ClAbstractVariable subject)
    { 
      if (Trace) 
        FnEnterPrint(string.Format("NoteRemovedVariable: {0}, {1}", v, subject));

      if (subject != null) {
        ((Set) _columns[v]).Remove(subject);
      }
    }

    /// <summary>
    /// v has been added to the linear expression for subject
    /// update column cross indices.
    /// </summary>
    public /*sealed*/ void NoteAddedVariable(ClAbstractVariable v, ClAbstractVariable subject)
    { 
      if (Trace) 
        FnEnterPrint(string.Format("NoteAddedVariable: {0}, {1}", v, subject));
      if (subject != null) {
        InsertColVar(v, subject);
      }
    }
      
    /// <summary>
    /// Returns information about the tableau's internals.
    /// </summary>
    /// <remarks>
    /// Originally from Michael Noth <noth@cs.washington.edu>
    /// </remarks>
    /// <returns>
    /// String containing the information.
    /// </returns>
    public virtual string GetInternalInfo() {
      string s = "Tableau Information:\n";
      s += string.Format("Rows: {0} (= {1} constraints)", _rows.Count, _rows.Count - 1);
      s += string.Format("\nColumns: {0}", _columns.Count);
      s += string.Format("\nInfeasible Rows: {0}", _infeasibleRows.Count);
      s += string.Format("\nExternal basic variables: {0}", _externalRows.Count);
      s += string.Format("\nExternal parametric variables: {0}", _externalParametricVars.Count);
        
      return s;
    }

    public override string ToString()
    { 
      string s = "Tableau:\n";
      
      foreach (ClAbstractVariable clv in _rows.Keys) 
      {
        ClLinearExpression expr = (ClLinearExpression) _rows[clv];
        s += string.Format("{0} <==> {1}\n", clv.ToString(), expr.ToString());
      }

      s += string.Format("\nColumns:\n{0}", _columns.ToString());
      s += string.Format("\nInfeasible rows: {0}", _infeasibleRows.ToString());

      s += string.Format("\nExternal basic variables: {0}", _externalRows.ToString());
      s += string.Format("\nExternal parametric variables: {0}", _externalParametricVars.ToString());
          
      return s;
    }

    
    /// <summary>
    /// Convenience function to insert a variable into
    /// the set of rows stored at _columns[param_var],
    /// creating a new set if needed. 
    /// </summary>
    private /*sealed*/ void InsertColVar(ClAbstractVariable param_var, 
                    ClAbstractVariable rowvar)
    { 
      Set rowset = (Set) _columns[param_var];
      
      if (rowset == null)
        _columns.Add(param_var, rowset = new Set());
      
      rowset.Add(rowvar);
    }

    // Add v=expr to the tableau, update column cross indices
    // v becomes a basic variable
    // expr is now owned by ClTableau class, 
    // and ClTableau is responsible for deleting it
    // (also, expr better be allocated on the heap!).
    protected /*sealed*/ void AddRow(ClAbstractVariable var, ClLinearExpression expr)
    {
      if (Trace) 
        FnEnterPrint("AddRow: " + var + ", " + expr);

      // for each variable in expr, add var to the set of rows which
      // have that variable in their expression
      _rows.Add(var, expr);
        
      // FIXME: check correctness!
      foreach (ClAbstractVariable clv in expr.Terms.Keys)
      {
        InsertColVar(clv, var);
        
        if (clv.IsExternal)
        {
          _externalParametricVars.Add(clv);
        }
      }

      if (var.IsExternal) {
        _externalRows.Add(var);
      }

      if (Trace) 
        TracePrint(this.ToString());
    }

    /// <summary>
    /// Remove v from the tableau -- remove the column cross indices for v
    /// and remove v from every expression in rows in which v occurs
    /// </summary>
    protected /*sealed*/ void RemoveColumn(ClAbstractVariable var)
    {
      if (Trace) 
        FnEnterPrint(string.Format("RemoveColumn: {0}", var));
      // remove the rows with the variables in varset

      Set rows = (Set) _columns[var];
      _columns.Remove(var);

      if (rows != null) {
        foreach(ClAbstractVariable clv in rows)
        {
          ClLinearExpression expr = (ClLinearExpression) _rows[clv];
          expr.Terms.Remove(var);
        }
      } else {
        if (Trace) 
          DebugPrint(string.Format("Could not find var {0} in _columns", var));
      }
          
      if (var.IsExternal) {
        _externalRows.Remove(var);
        _externalParametricVars.Remove(var);
      }
    }

    /// <summary>
    /// Remove the basic variable v from the tableau row v=expr
    /// Then update column cross indices.
    /// </summary>
    protected /*sealed*/ ClLinearExpression RemoveRow(ClAbstractVariable var)
      /*throws ExCLInternalError*/
    {
      if (Trace) 
        FnEnterPrint(string.Format("RemoveRow: {0}", var));

      ClLinearExpression expr = (ClLinearExpression) _rows[var];
      Assert(expr != null);

      // For each variable in this expression, update
      // the column mapping and remove the variable from the list
      // of rows it is known to be in.
      foreach (ClAbstractVariable clv in expr.Terms.Keys)
      {
        Set varset = (Set) _columns[clv];
        
        if (varset != null) 
        {
          if (Trace) 
            DebugPrint(string.Format("removing from varset {0}", var));
          
          varset.Remove(var);
        }
      }
                
      _infeasibleRows.Remove(var);

      if (var.IsExternal) {
        _externalRows.Remove(var);
      }

      _rows.Remove(var);
      if (Trace) 
        FnExitPrint(string.Format("returning {0}", expr));
      
      return expr;
    }

    /// <summary> 
    /// Replace all occurrences of oldVar with expr, and update column cross indices
    /// oldVar should now be a basic variable.
    /// </summary> 
    protected /*sealed*/ void SubstituteOut(ClAbstractVariable oldVar, ClLinearExpression expr)
    {
      if (Trace)
        FnEnterPrint(string.Format("SubstituteOut: {0}", oldVar, expr));
      if (Trace) 
        TracePrint(this.ToString());
          
      Set varset = (Set) _columns[oldVar];
      
      foreach(ClAbstractVariable v in varset)
      {
        ClLinearExpression row = (ClLinearExpression) _rows[v];
        row.SubstituteOut(oldVar, expr, v, this);
        if (v.IsRestricted && row.Constant < 0.0) 
        {
          _infeasibleRows.Add(v);
        }
      }
      
      if (oldVar.IsExternal) {
        _externalRows.Add(oldVar);
        _externalParametricVars.Remove(oldVar);
      }
    
      _columns.Remove(oldVar);
    }

    protected Hashtable Columns
    {
      get { 
        return _columns; 
      }
    }

    protected Hashtable Rows
    {
      get 
      { 
        return _rows; 
      }
    }

    /// <summary>
    /// Return true if and only if the variable subject is in the columns keys 
    /// </summary>
    protected /*sealed*/ bool ColumnsHasKey(ClAbstractVariable subject)
    { 
      return _columns.ContainsKey(subject);
    }

    protected /*sealed*/ ClLinearExpression RowExpression(ClAbstractVariable v)
    {
      // if (Trace) FnEnterPrint(string.Format("rowExpression: {0}", v));
      return (ClLinearExpression) _rows[v];
    }

    /// <summary>
    /// _columns is a mapping from variables which occur in expressions to the
    /// set of basic variables whose expressions contain them
    /// i.e., it's a mapping from variables in expressions (a column) to the 
    /// set of rows that contain them.
    /// </summary>
    protected Hashtable _columns; // From ClAbstractVariable to Set of variables

    /// <summary>
    /// _rows maps basic variables to the expressions for that row in the tableau.
    /// </summary>
    protected Hashtable _rows;    // From ClAbstractVariable to ClLinearExpression

    /// <summary>
    /// Collection of basic variables that have infeasible rows
    /// (used when reoptimizing).
    /// </summary>
    protected Set _infeasibleRows; // Set of ClAbstractVariable-s

    /// <summary>
    /// Set of rows where the basic variable is external
    /// this was added to the Java/C++/C# versions to reduce time in SetExternalVariables().
    /// </summary>
    protected Set _externalRows; // Set of ClVariable-s

    /// <summary>
    /// Set of external variables which are parametric
    /// this was added to the Java/C++/C# versions to reduce time in SetExternalVariables().
    /// </summary>
    protected Set _externalParametricVars; // Set of ClVariable-s
  }
}
