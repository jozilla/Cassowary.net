using System.Collections;
using System.Globalization;
using Cassowary;

using System;



public class Parser {
	const int _EOF = 0;
	const int _leq = 1;
	const int _geq = 2;
	const int _eq = 3;
	const int _altleq = 4;
	const int _altgeq = 5;
	const int _plus = 6;
	const int _minus = 7;
	const int _times = 8;
	const int _divide = 9;
	const int _lparen = 10;
	const int _rparen = 11;
	const int _variable = 12;
	const int _number = 13;
	const int maxT = 14;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

ClConstraint _constraint;
Hashtable _context = new Hashtable();

public Hashtable Context
{
  get { return _context; }
  set { _context = value; }
}

public ClConstraint Value
{
  get { return _constraint; }
  set { _constraint = value; }
}



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void Constraint() {
		ClLinearExpression e1, e2; bool eq = false, geq = false, leq = false; 
		Expression(out e1);
		if (la.kind == 3) {
			Get();
			eq = true; 
		} else if (la.kind == 2) {
			Get();
			geq = true; 
		} else if (la.kind == 5) {
			Get();
			geq = true; 
		} else if (la.kind == 1) {
			Get();
			leq = true; 
		} else if (la.kind == 4) {
			Get();
			leq = true; 
		} else SynErr(15);
		Expression(out e2);
		if (eq)
		 Value = new ClLinearEquation(e1, e2);
		else if (geq)
		  Value = new ClLinearInequality(e1, Cl.GEQ, e2);
		else if (leq)
		  Value = new ClLinearInequality(e1, Cl.LEQ, e2);
		
	}

	void Expression(out ClLinearExpression e) {
		e = null; ClLinearExpression e1; 
		Term(out e);
		while (la.kind == 6 || la.kind == 7) {
			if (la.kind == 6) {
				Get();
				Term(out e1);
				e = Cl.Plus(e, e1); 
			} else {
				Get();
				Term(out e1);
				e = Cl.Minus(e, e1); 
			}
		}
	}

	void Term(out ClLinearExpression e) {
		e = null; ClLinearExpression e1; 
		Factor(out e);
		while (la.kind == 8 || la.kind == 9) {
			if (la.kind == 8) {
				Get();
				Factor(out e1);
				e = Cl.Times(e, e1); 
			} else {
				Get();
				Factor(out e1);
				e = Cl.Divide(e, e1); 
			}
		}
	}

	void Factor(out ClLinearExpression e) {
		e = null; ClDouble d; ClVariable v; bool negate = false; 
		if (la.kind == 7) {
			Get();
			negate = true; 
		}
		if (la.kind == 13) {
			Number(out d);
			e = new ClLinearExpression(d.Value); 
		} else if (la.kind == 12) {
			Variable(out v);
			e = new ClLinearExpression(v); 
		} else if (la.kind == 10) {
			Get();
			Expression(out e);
			Expect(11);
		} else SynErr(16);
		if (negate)
		 e = Cl.Minus(0, e);
		
	}

	void Number(out ClDouble d) {
		Expect(13);
		double tmpVal = double.Parse(t.val, new CultureInfo("en-US").NumberFormat);
		d = new ClDouble(tmpVal);
		
	}

	void Variable(out ClVariable v) {
		Expect(12);
		if (Context.ContainsKey(t.val))
		{
		  v = (ClVariable) Context[t.val];
		}
		else
		{
		SemErr("Undefined variable: " + t.val);
		v = null;
		                      }
		                    
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Constraint();

    Expect(0);
	}
	
	bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
  public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text
  
	public void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "leq expected"; break;
			case 2: s = "geq expected"; break;
			case 3: s = "eq expected"; break;
			case 4: s = "altleq expected"; break;
			case 5: s = "altgeq expected"; break;
			case 6: s = "plus expected"; break;
			case 7: s = "minus expected"; break;
			case 8: s = "times expected"; break;
			case 9: s = "divide expected"; break;
			case 10: s = "lparen expected"; break;
			case 11: s = "rparen expected"; break;
			case 12: s = "variable expected"; break;
			case 13: s = "number expected"; break;
			case 14: s = "??? expected"; break;
			case 15: s = "invalid Constraint"; break;
			case 16: s = "invalid Factor"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}

