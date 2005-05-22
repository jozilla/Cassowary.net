using System;

namespace Cassowary
{
	/// <summary>
	/// ClEditInfo is privately-used class
	/// that just wraps a constraint, its positive and negative
	/// error variables, and it prior edit constant.
	/// It is used as values in _editVarMap, and replaces
	/// the parallel vectors of error variables and previous edit
	/// constants from the Smalltalk version of the code.
	/// </summary>
	public class ClEditInfo
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

		public ClSlackVariable ClvEditMin
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
