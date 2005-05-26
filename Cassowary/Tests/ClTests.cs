using System;

namespace Cassowary.Tests
{
	/// <summary>
	/// Cassowary test class.
	/// </summary>
	public class ClTests
	{
		public static bool Simple1()
		{
			bool okResult = true;
			ClVariable x = new ClVariable(167);
			ClVariable y = new ClVariable(2);
			ClSimplexSolver solver = new ClSimplexSolver();
					
			ClLinearEquation eq = new ClLinearEquation(x,new ClLinearExpression(y));
			solver.AddConstraint(eq);
			okResult = (x.Value == y.Value);

			Console.WriteLine("x == " + x.Value);
			Console.WriteLine("y == " + y.Value);
	
			return okResult;
		}
		
		public static bool JustStay1()
  	{
	    bool okResult = true;
	    ClVariable x = new ClVariable(5);
	    ClVariable y = new ClVariable(10);
	    ClSimplexSolver solver = new ClSimplexSolver();
	      
	    solver.AddStay(x);
	    solver.AddStay(y);
	    okResult = okResult && Cl.Approx(x,5);
	    okResult = okResult && Cl.Approx(y,10);
	    
			Console.WriteLine("x == " + x.Value);
	    Console.WriteLine("y == " + y.Value);
			
	    return okResult;
		}
	
		public static bool AddDelete1()
  	{
			bool okResult = true; 
			ClVariable x = new ClVariable("x");
			ClSimplexSolver solver = new ClSimplexSolver();
				
			solver.AddConstraint( new ClLinearEquation(x, 100, ClStrength.Weak ) );
				
			ClLinearInequality c10 = new ClLinearInequality(x, Cl.LEQ, 10.0);
			ClLinearInequality c20 = new ClLinearInequality(x, Cl.LEQ, 20.0);
				
			solver
				.AddConstraint(c10)
				.AddConstraint(c20);
				
			okResult = okResult && Cl.Approx(x, 10.0);
			Console.WriteLine("x == " + x.Value);
				
			solver.RemoveConstraint(c10);
			okResult = okResult && Cl.Approx(x, 20.0);
			Console.WriteLine("x == " + x.Value);

			solver.RemoveConstraint(c20);
			okResult = okResult && Cl.Approx(x, 100.0);
			Console.WriteLine("x == " + x.Value);

			ClLinearInequality c10again = new ClLinearInequality(x, Cl.LEQ, 10.0);

			solver
				.AddConstraint(c10)
				.AddConstraint(c10again);
				
			okResult = okResult && Cl.Approx(x, 10.0);
			Console.WriteLine("x == " + x.Value);
			
			solver.RemoveConstraint(c10);
			okResult = okResult && Cl.Approx(x,10.0);
			Console.WriteLine("x == " + x.Value);

			solver.RemoveConstraint(c10again);
			okResult = okResult && Cl.Approx(x,100.0);
			Console.WriteLine("x == " + x.Value);

			return okResult;
		}	

		public static bool AddDelete2()
  	{
			bool okResult = true; 
			ClVariable x = new ClVariable("x");
			ClVariable y = new ClVariable("y");
			ClSimplexSolver solver = new ClSimplexSolver();

			solver
				.AddConstraint( new ClLinearEquation(x, 100.0, ClStrength.Weak))
				.AddConstraint( new ClLinearEquation(y, 120.0, ClStrength.Strong));
				
			ClLinearInequality c10 = new ClLinearInequality(x,Cl.LEQ, 10.0);
			ClLinearInequality c20 = new ClLinearInequality(x,Cl.LEQ, 20.0);
				
			solver
				.AddConstraint(c10)
				.AddConstraint(c20);
			okResult = okResult && Cl.Approx(x, 10.0) && Cl.Approx(y, 120.0);
			Console.WriteLine("x == " + x.Value + ", y == " + y.Value);

			solver.RemoveConstraint(c10);
			okResult = okResult && Cl.Approx(x, 20.0) && Cl.Approx(y, 120.0);
			Console.WriteLine("x == " + x.Value + ", y == " + y.Value);
		 
			ClLinearEquation cxy = new ClLinearEquation( Cl.Times(2.0, x), y);
			solver.AddConstraint(cxy);
			okResult = okResult && Cl.Approx(x, 20.0) && Cl.Approx(y, 40.0);
			Console.WriteLine("x == " + x.Value + ", y == " + y.Value);

			solver.RemoveConstraint(c20);
			okResult = okResult && Cl.Approx(x, 60.0) && Cl.Approx(y, 120.0);
			Console.WriteLine("x == " + x.Value + ", y == " + y.Value);

			solver.RemoveConstraint(cxy);
			okResult = okResult && Cl.Approx(x, 100.0) && Cl.Approx(y, 120.0);
			Console.WriteLine("x == " + x.Value + ", y == " + y.Value);
				
			return okResult;
		}

		public static bool Inconsistent1()
		{
			try 
			{
				ClVariable x = new ClVariable("x");
				ClSimplexSolver solver = new ClSimplexSolver();
				
				solver
					.AddConstraint( new ClLinearEquation(x, 10.0) )
					.AddConstraint( new ClLinearEquation(x, 5.0) );
				
				// no exception, we failed!
				return false;
			} 
			catch (ExClRequiredFailure rf)
			{
				// we want this exception to get thrown
				Console.WriteLine("-- got the exception");
				return true;
			}
		}
						
		[STAThread]
		static void Main(string[] args)
		{
			ClTests clt = new ClTests();

			bool allOkResult = true;
			bool result;
			
			////////////////////////// Simple1 ////////////////////////// 
			Console.WriteLine("Simple1:");
			
			result = Simple1(); 
			allOkResult &= result;
				
			if (!result) 
				Console.WriteLine("--> Failed!");
			else
				Console.WriteLine("--> Succeeded!");
			if (Cl.cGC) 
				Console.WriteLine("Num vars = " + ClAbstractVariable.NumCreated );

			////////////////////////// JustStay1 ////////////////////////// 
			Console.WriteLine("\nJustStay1:");
			
			result = JustStay1(); 
			allOkResult &= result;
			
			if (!result) 
				Console.WriteLine("--> Failed!");
			else
				Console.WriteLine("--> Succeeded!");
			if (Cl.cGC) 
				Console.WriteLine("Num vars = " + ClAbstractVariable.NumCreated );

			////////////////////////// AddDelete1 ////////////////////////// 
			Console.WriteLine("\nAddDelete1:");
      result = AddDelete1(); 
			allOkResult &= result;
			
			if (!result) 
				Console.WriteLine("--> Failed!");
			else
				Console.WriteLine("--> Succeeded!");
			if (Cl.cGC) 
				Console.WriteLine("Num vars = " + ClAbstractVariable.NumCreated );

			////////////////////////// AddDelete2 ////////////////////////// 
			Console.WriteLine("\nAddDelete2:");
      result = AddDelete2(); 
			allOkResult &= result;
			
			if (!result) 
				Console.WriteLine("--> Failed!");
			else
				Console.WriteLine("--> Succeeded!");
			if (Cl.cGC) 
				Console.WriteLine("Num vars = " + ClAbstractVariable.NumCreated );

			////////////////////////// Inconsistent1 ////////////////////////// 
			Console.WriteLine("\nInconsistent1:");
      result = Inconsistent1(); 
			allOkResult &= result;
			
			if (!result) 
				Console.WriteLine("--> Failed!");
			else
				Console.WriteLine("--> Succeeded!");
			if (Cl.cGC) 
				Console.WriteLine("Num vars = " + ClAbstractVariable.NumCreated );
		}
	}
}
