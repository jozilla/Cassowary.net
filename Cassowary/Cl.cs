/*
	Cassowary.net: an incremental constraint solver for .NET
	(http://lumumba.uhasselt.be/jo/projects/cassowary.net/)
	
	Copyright (C) 2005	Jo Vermeulen (jo.vermeulen@uhasselt.be)
		
	This program is free software; you can redistribute it and/or
	modify it under the terms of the GNU Lesser General Public License
	as published by the Free Software Foundation; either version 2.1
	of	the License, or (at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.	See the
	GNU General Public License for more details.

	You should have received a copy of the GNU Lesser General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA	 02111-1307, USA.
*/

using System;
using System.Collections;

namespace Cassowary
{
	/// <summary>
	/// The enumerations from ClLinearInequality,
	/// and `global' functions that we want easy to access
	/// </summary>
	public class Cl
	{
		protected static bool _debug = false;
		protected static bool _trace = false;
		protected static bool _gc = false;

		protected static void DebugPrint(string s)
		{
			#if !COMPACT
			  Console.Error.WriteLine(s);
			#else
			  Console.WriteLine(s);
			#endif
		}

		protected static void TracePrint(string s)
		{
			#if !COMPACT
			  Console.Error.WriteLine(s);
			#else
			  Console.WriteLine(s);
			#endif
		}

		protected static void FnEnterPrint(string s)
		{
			#if !COMPACT
			  Console.Error.WriteLine("* {0}", s);
			#else
			  Console.WriteLine("* {0}", s);
			#endif
		}
		
		protected static void FnExitPrint(string s)
		{
			#if !COMPACT
			  Console.Error.WriteLine("- {0}", s);
			#else
			  Console.WriteLine("- {0}", s);
			#endif
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

		public static ClLinearExpression Times(ClLinearExpression e1, ClLinearExpression e2) 
		  /*throws ExCLNonlinearExpression*/
		{ 
			return e1.Times(e2); 
		}

		public static ClLinearExpression Times(ClLinearExpression e1, ClVariable e2) 
		  /*throws ExCLNonlinearExpression*/
		{ 
			return e1.Times(new ClLinearExpression(e2)); 
		}

		public static ClLinearExpression Times(ClVariable e1, ClLinearExpression e2) 
		  /*throws ExCLNonlinearExpression*/
		{ 
			return (new ClLinearExpression(e1)).Times(e2); 
		}

		public static ClLinearExpression Times(ClLinearExpression e1, double e2) 
		  /*throws ExCLNonlinearExpression*/
		{		
			return e1.Times(new ClLinearExpression(e2)); 
		}

		public static ClLinearExpression Times(double e1, ClLinearExpression e2) 
		  /*throws ExCLNonlinearExpression*/
		{ 
			return (new ClLinearExpression(e1)).Times(e2); 
		}

		public static ClLinearExpression Times(double n, ClVariable clv) 
		  /*throws ExCLNonlinearExpression*/
		{ 
			return new ClLinearExpression(clv,n); 
		}

		public static ClLinearExpression Times( ClVariable clv, double n) 
		  /*throws ExCLNonlinearExpression*/
		{ 
			return new ClLinearExpression(clv,n); 
		}

		public static ClLinearExpression Divide(ClLinearExpression e1, ClLinearExpression e2) 
		  /*throws ExCLNonlinearExpression*/
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

		public static string HashtableToString(Hashtable h)
		{
		  string result = "{";
		  
		  bool first = true;
		  foreach (object k in h.Keys)
		  {
		    if (first)
		      first = false;
		    else
		    {
		      result += ", ";
		    }
		    
		    result += k.ToString() + " => " + h[k].ToString();
		  }
		  
		  result += "}";
		  return result;
		}

		public static string ArrayListToString(ArrayList a)
		{
		  string result = "{";
		  
		  bool first = true;
		  foreach (object o in a)
		  {
		    if (first)
		      first = false;
		    else
		    {
		      result += ", ";
	      }
		      
		    result += o.ToString();
		  }
		  
		  result += "}";
		  return result;
		}

    public static bool Debug 
    {
      get { return _debug; }
      set { _debug = value; }
    }

    public static bool Trace 
    {
      get { return _trace; }
      set { _trace = value; }
    }

    public static bool GC 
    {
      get { return _gc; }
      set { _gc = value; }
    }
	}
}
