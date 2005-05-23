using System;
using System.Collections;

namespace Cassowary
{
	public class ClSimplexSolver : ClTableau
	{
		/// <remarks>
		/// Constructor initializes the fields, and creaties the objective row
		/// </remarks>
		public ClSimplexSolver()
		{	
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
		
		private bool _fOptimizeAutomatically;
		private bool _fNeedsSolving;
			
		private Stack _stkCedcns;
	}
}
