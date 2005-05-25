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
	
			return(okResult);
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
			
	    return(okResult);
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

			return(okResult);
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
		}
	}
}
