public class ClPoint
{
	public ClPoint(double x, double y)
	{
		_clv_x = new ClVariable(x);
		_clv_y = new ClVariable(y);
	}

	public ClPoint(double x, double y, int a)
	{
		_clv_x = new ClVariable("x"+a,x);
		_clv_y = new ClVariable("y"+a,y);
	}

	public ClPoint(ClVariable clv_x, ClVariable clv_y)
	{
		_clv_x = clv_x;
		_clv_y = clv_y;
	}

	public ClVariable X
	{
		get { return _clv_x; }
		set { _clv_x = value; }
	}

	public ClVariable Y
	{
		get { return _clv_y; }
		set { _clv_y = value; }
	}

	/// <remarks>
	/// Use only before adding into the solver
	/// </remarks>
	public void SetXY(double x, double y)
	{
		_clv_x.Value = x;
		_clv_y.Value = y;
	}

	public void SetXY(ClVariable clv_x, ClVariable clv_y)
	{
		_clv_x = clv_x;
		_clv_y = clv_y;
	}

	public double XValue
	{
		get { return X.Value;  }
		set { X.Value = value; }
	}

	public double YValue
	{
		get { return Y.Value; }
		set { Y.Value = value; }
	}	

	public override string ToString()
	{
		return "(" + _clv_x.ToString() + ", " + _clv_y.ToString() + ")";
	}

	private ClVariable _clv_x;
	private ClVariable _clv_y;
}
