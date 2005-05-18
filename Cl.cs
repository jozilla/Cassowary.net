using System;

namespace Cassowary
{
	/// <summary>
	/// The enumerations from ClLinearInequality,
	/// and `global' functions that we want easy to access
	/// </summary>
	public class Cl
	{
		protected const bool cDebugOn = false;
		protected const bool cTraceOn = false;
		protected const bool cGC = false;

		protected static void DebugPrint(string s)
		{
			Console.Error.WriteLine(s);
		}

		protected static void TracePrint(string s)
		{
			Console.Error.WriteLine(s);
		}

		protected static void FnEnterPrint(string s)
		{
			Console.Error.WriteLine("* {0}", s);
		}

		protected static void FnExitPrint(string s)
		{
			Console.Error.WriteLine("- {0}", s);
		}

		protected void Assert(bool f, string description)
		{
			if (!f) 
			{
				throw new ExClInternalError(string.Format("Assertion failed: {0}", description));
			}
		}

		protected void Assert(bool f)
		{
			if (!f) 
			{
				throw new ExClInternalError("Assertion failed");
			}
		}

		public const byte GEQ = 1;
		public const byte LEQ = 2;

		public static ClLinearExpression Plus(ClLinearExpression e1, ClLinearExpression e2)
		{
			return e1.Plus(e2);
		}

		public static ClLinearExpression Plus(double e1, ClLinearExpression e2)
		{
			return (new ClLinearExpression(e1)).Plus(e2);
		}

		public static ClLinearExpression Plus(ClVariable e1, ClLinearExpression e2)
		{
			return (new ClLinearExpression(e1)).Plus(e2);
		}

		public static ClLinearExpression Plus(ClLinearExpression e1, ClVariable e2)
		{
			return e1.Plus(new ClLinearExpression(e2));
		}

		public static ClLinearExpression Plus(ClVariable e1, double e2)
		{
			return (new ClLinearExpression(e1)).Plus(new ClLinearExpression(e2));
		}

		public static ClLinearExpression Plus(double e1, ClVariable e2)
		{
			return (new ClLinearExpression(e1)).Plus(new ClLinearExpression(e2));
		}

		public static ClLinearExpression Minus(ClLinearExpression e1, ClLinearExpression e2)
		{ 
			return e1.Minus(e2); 
		}

		public static ClLinearExpression Minus(double e1, ClLinearExpression e2)
		{ 
			return (new ClLinearExpression(e1)).Minus(e2); 
		}

		public static ClLinearExpression Minus(ClLinearExpression e1, double e2)
		{ 
			return e1.Minus(new ClLinearExpression(e2)); 
		}

		public static ClLinearExpression Times(ClLinearExpression e1, ClLinearExpression e2) /*throws ExCLNonlinearExpression*/
		{ 
			return e1.Times(e2); 
		}

		public static ClLinearExpression Times(ClLinearExpression e1, ClVariable e2) /*throws ExCLNonlinearExpression*/
		{ 
			return e1.Times(new ClLinearExpression(e2)); 
		}

		public static ClLinearExpression Times(ClVariable e1, ClLinearExpression e2) /*throws ExCLNonlinearExpression*/
		{ 
			return (new ClLinearExpression(e1)).Times(e2); 
		}
	
		public static ClLinearExpression Times(ClLinearExpression e1, double e2) /*throws ExCLNonlinearExpression*/
		{	
			return e1.Times(new ClLinearExpression(e2)); 
		}

		public static ClLinearExpression Times(double e1, ClLinearExpression e2) /*throws ExCLNonlinearExpression*/
		{ 
			return (new ClLinearExpression(e1)).Times(e2); 
		}

		public static ClLinearExpression Times(double n, ClVariable clv) /*throws ExCLNonlinearExpression*/
		{ 
			return (new ClLinearExpression(clv,n)); 
		}

		public static ClLinearExpression Times( ClVariable clv, double n) /*throws ExCLNonlinearExpression*/
		{ 
			return (new ClLinearExpression(clv,n)); 
		}

		public static ClLinearExpression Divide(ClLinearExpression e1, ClLinearExpression e2) /*throws ExCLNonlinearExpression*/
		{
			return e1.Divide(e2); 
		}

		public static bool Approx(double a, double b)
		{
			double epsilon = 1.0e-8;

			if (a == 0.0) 
			{
				return (Math.Abs(b) < epsilon);
			} 
			else if (b == 0.0) 
			{
				return (Math.Abs(a) < epsilon);
			} 
			else 
			{
				return (Math.Abs(a-b) < Math.Abs(a) * epsilon);
			}
		}
  
		public static bool Approx(ClVariable clv, double b)
		{
			return Approx(clv.Value, b);
		}
  
		static bool Approx(double a, ClVariable clv)
		{
			return Approx(a, clv.Value);
		}
	}
}