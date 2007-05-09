/*
  Cassowary.net: an incremental constraint solver for .NET
  (http://lumumba.uhasselt.be/jo/projects/cassowary.net/)
  
  Copyright (C) 2005-2006  Jo Vermeulen (jo.vermeulen@uhasselt.be)
  
  This program is free software; you can redistribute it and/or
  modify it under the terms of the GNU Lesser General Public License
  as published by the Free Software Foundation; either version 2.1
  of  the License, or (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU Lesser General Public License for more details.

  You should have received a copy of the GNU Lesser General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using System;

namespace Cassowary.Tests
{
  /// <summary>
  /// Cassowary test class.
  /// </summary>
  public class ClTests : Cl
  {
    public ClTests()
    {
      _rnd = new Random(123456789);
    }
    
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
  
    public static bool Casso1()
    {
      bool okResult = true; 
      ClVariable x = new ClVariable("x");
      ClVariable y = new ClVariable("y");
      ClSimplexSolver solver = new ClSimplexSolver();

      solver
        .AddConstraint( new ClLinearInequality(x, Cl.LEQ, y) )
        .AddConstraint( new ClLinearEquation(y, Cl.Plus(x, 3.0)) )
        .AddConstraint( new ClLinearEquation(x, 10.0, ClStrength.Weak) )
        .AddConstraint( new ClLinearEquation(y, 10.0, ClStrength.Weak) );
     
      okResult = okResult && 
        ( Cl.Approx(x,10.0) && Cl.Approx(y,13.0) ||
          Cl.Approx(x,7.0) && Cl.Approx(y,10.0) );
        
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
      catch (ExClRequiredFailure)
      {
        // we want this exception to get thrown
        Console.WriteLine("-- got the exception");
        return true;
      }
    }
    
    public static bool Inconsistent2()
    {
      try 
      {
        ClVariable x = new ClVariable("x");
        ClSimplexSolver solver = new ClSimplexSolver();
        
        solver
          .AddConstraint( new ClLinearInequality(x, Cl.GEQ, 10.0) )
          .AddConstraint( new ClLinearInequality(x, Cl.LEQ, 5.0) );

        // no exception, we failed!
        return false;
      } 
      catch (ExClRequiredFailure)
      {
        // we want this exception to get thrown
        Console.WriteLine("-- got the exception");
        return true;
      }
    }

    public static bool Multiedit()
    {
      try 
      {
        bool okResult = true;

        ClVariable x = new ClVariable("x");
        ClVariable y = new ClVariable("y");
        ClVariable w = new ClVariable("w");
        ClVariable h = new ClVariable("h");
        ClSimplexSolver solver = new ClSimplexSolver();
        
        solver
          .AddStay(x)
          .AddStay(y)
          .AddStay(w)
          .AddStay(h);

        solver
          .AddEditVar(x)
          .AddEditVar(y)
          .BeginEdit();

        solver
          .SuggestValue(x, 10)
          .SuggestValue(y, 20)
          .Resolve();

        Console.WriteLine("x = " + x.Value + "; y = " + y.Value);
        Console.WriteLine("w = " + w.Value + "; h = " + h.Value);

        okResult = okResult &&
          Cl.Approx(x, 10) && Cl.Approx(y, 20) &&
          Cl.Approx(w, 0) && Cl.Approx(h, 0);

        solver
          .AddEditVar(w)
          .AddEditVar(h)
          .BeginEdit();

        solver
          .SuggestValue(w, 30)
          .SuggestValue(h, 40)
          .EndEdit();

        Console.WriteLine("x = " + x.Value + "; y = " + y.Value);
        Console.WriteLine("w = " + w.Value + "; h = " + h.Value);

        okResult = okResult &&
          Cl.Approx(x, 10) && Cl.Approx(y, 20) && 
          Cl.Approx(w, 30) && Cl.Approx(h, 40);

        solver
          .SuggestValue(x, 50)
          .SuggestValue(y, 60)
          .EndEdit();

        Console.WriteLine("x = " + x.Value + "; y = " + y.Value);
        Console.WriteLine("w = " + w.Value + "; h = " + h.Value);

        okResult = okResult &&
          Cl.Approx(x, 50) && Cl.Approx(y, 60) &&
          Cl.Approx(w, 30) && Cl.Approx(h, 40);

        return okResult;
      } 
      catch (ExClRequiredFailure)
      {
        // we want this exception to get thrown
        Console.WriteLine("-- got the exception");
        return true;
      }
    }

    public static bool Inconsistent3()
    {
      try 
      {
        ClVariable w = new ClVariable("w");
        ClVariable x = new ClVariable("x");
        ClVariable y = new ClVariable("y");
        ClVariable z = new ClVariable("z");
        ClSimplexSolver solver = new ClSimplexSolver();
        
        solver
          .AddConstraint( new ClLinearInequality(w, Cl.GEQ, 10.0) )
          .AddConstraint( new ClLinearInequality(x, Cl.GEQ, w) )
          .AddConstraint( new ClLinearInequality(y, Cl.GEQ, x) )
          .AddConstraint( new ClLinearInequality(z, Cl.GEQ, y) )
          .AddConstraint( new ClLinearInequality(z, Cl.GEQ, 8.0) )
          .AddConstraint( new ClLinearInequality(z, Cl.LEQ, 4.0) );

        // no exception, we failed!
        return false;
      } 
      catch (ExClRequiredFailure)
      {
        // we want this exception to get thrown
        Console.WriteLine("-- got the exception");
        return true;
      }
    }

    public static bool AddDel(int nCns, int nVars, int nResolves)
    {
      Timer timer = new Timer();
      double ineqProb = 0.12;
      int maxVars = 3;

      Console.WriteLine("starting timing test. nCns = " + nCns +
          ", nVars = " + nVars + ", nResolves = " + nResolves);
      
      timer.Start();
      ClSimplexSolver solver = new ClSimplexSolver();

      ClVariable[] rgpclv = new ClVariable[nVars];
      for (int i = 0; i < nVars; i++) {
        rgpclv[i] = new ClVariable(i, "x");
        solver.AddStay(rgpclv[i]);
      }

      ClConstraint[] rgpcns = new ClConstraint[nCns];
      int nvs = 0;
      int k;
      int j;
      double coeff;
      for (j = 0; j < nCns; j++) {
        // number of variables in this constraint
        nvs = RandomInRange(1, maxVars);
        ClLinearExpression expr = new ClLinearExpression(UniformRandomDiscretized() * 20.0 - 10.0);
        for (k = 0; k < nvs; k++) {
          coeff = UniformRandomDiscretized()*10 - 5;
          int iclv = (int) (UniformRandomDiscretized()*nVars);
          expr.AddExpression(Cl.Times(rgpclv[iclv], coeff));
        }
        if (UniformRandomDiscretized() < ineqProb) {
          rgpcns[j] = new ClLinearInequality(expr);
        } else {  
          rgpcns[j] = new ClLinearEquation(expr);
        }
        if (Trace) 
          TracePrint("Constraint " + j + " is " + rgpcns[j]);
      }

      Console.WriteLine("done building data structures");
      Console.WriteLine("time = " + timer.ElapsedTime);
      timer.Start();
      int cExceptions = 0;
      for (j = 0; j < nCns; j++) {
        // add the constraint -- if it's incompatible, just ignore it
        try
        {
          solver.AddConstraint(rgpcns[j]);
        }
        catch (ExClRequiredFailure)
        {
          cExceptions++;
          if (Trace) 
            TracePrint("got exception adding " + rgpcns[j]);
          
          rgpcns[j] = null;
        }
      }
      Console.WriteLine("done adding constraints [" + cExceptions + " exceptions]");
      Console.WriteLine("time = " + timer.ElapsedTime + "\n");
      timer.Start();

      int e1Index = (int) (UniformRandomDiscretized()*nVars);
      int e2Index = (int) (UniformRandomDiscretized()*nVars);

      Console.WriteLine("indices " + e1Index + ", " + e2Index);
      
      ClEditConstraint edit1 = new ClEditConstraint(rgpclv[e1Index],ClStrength.Strong);
      ClEditConstraint edit2 = new ClEditConstraint(rgpclv[e2Index],ClStrength.Strong);

      solver
        .AddConstraint(edit1)
        .AddConstraint(edit2);

      Console.WriteLine("done creating edit constraints -- about to start resolves");
      Console.WriteLine("time = " + timer.ElapsedTime + "\n");
      timer.Start();

      for (int m = 0; m < nResolves; m++)
      {
              solver.Resolve(rgpclv[e1Index].Value * 1.001,
                             rgpclv[e2Index].Value * 1.001);
      }

      Console.WriteLine("done resolves -- now removing constraints");
      Console.WriteLine("time = " + timer.ElapsedTime + "\n");
      
      solver.RemoveConstraint(edit1);
      solver.RemoveConstraint(edit2);

      timer.Start();

      for (j = 0; j < nCns; j++)
      {
        if (rgpcns[j] != null)
        {
                solver.RemoveConstraint(rgpcns[j]);
        }
      }
      
      Console.WriteLine("done removing constraints and AddDel timing test");
      Console.WriteLine("time = " + timer.ElapsedTime + "\n");

      timer.Start();
     
      return true;
    }
    
    public static double UniformRandomDiscretized()
    {
      double n = Math.Abs(_rnd.Next());
      return n / int.MaxValue;
    }
    
    public static int RandomInRange(int low, int high)
    {
      return (int) UniformRandomDiscretized()*(high-low)+low;
    }
    
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
      if (GC) 
        Console.WriteLine("Num vars = " + ClAbstractVariable.NumCreated );

      ////////////////////////// JustStay1 ////////////////////////// 
      Console.WriteLine("\nJustStay1:");
      
      result = JustStay1(); 
      allOkResult &= result;
      
      if (!result) 
        Console.WriteLine("--> Failed!");
      else
        Console.WriteLine("--> Succeeded!");
      if (GC) 
        Console.WriteLine("Num vars = " + ClAbstractVariable.NumCreated );

      ////////////////////////// AddDelete1 ////////////////////////// 
      Console.WriteLine("\nAddDelete1:");
      result = AddDelete1(); 
      allOkResult &= result;
      
      if (!result) 
        Console.WriteLine("--> Failed!");
      else
        Console.WriteLine("--> Succeeded!");
      if (GC) 
        Console.WriteLine("Num vars = " + ClAbstractVariable.NumCreated );

      ////////////////////////// AddDelete2 ////////////////////////// 
      Console.WriteLine("\nAddDelete2:");
      result = AddDelete2(); 
      allOkResult &= result;
      
      if (!result) 
        Console.WriteLine("--> Failed!");
      else
        Console.WriteLine("--> Succeeded!");
      if (GC) 
        Console.WriteLine("Num vars = " + ClAbstractVariable.NumCreated );

      ////////////////////////// Casso1 ////////////////////////// 
      Console.WriteLine("\nCasso1:");
      result = Casso1(); 
      allOkResult &= result;
      
      if (!result) 
        Console.WriteLine("--> Failed!");
      else
        Console.WriteLine("--> Succeeded!");
      if (GC) 
        Console.WriteLine("Num vars = " + ClAbstractVariable.NumCreated );

      ////////////////////////// Inconsistent1 ////////////////////////// 
      Console.WriteLine("\nInconsistent1:");
      result = Inconsistent1(); 
      allOkResult &= result;
      
      if (!result) 
        Console.WriteLine("--> Failed!");
      else
        Console.WriteLine("--> Succeeded!");
      if (GC) 
        Console.WriteLine("Num vars = " + ClAbstractVariable.NumCreated );
      
      ////////////////////////// Inconsistent2 ////////////////////////// 
      Console.WriteLine("\nInconsistent2:");
      result = Inconsistent2(); 
      allOkResult &= result;
      
      if (!result) 
        Console.WriteLine("--> Failed!");
      else
        Console.WriteLine("--> Succeeded!");
      if (GC) 
        Console.WriteLine("Num vars = " + ClAbstractVariable.NumCreated );

      ////////////////////////// Inconsistent3 ////////////////////////// 
      Console.WriteLine("\nInconsistent3:");
      result = Inconsistent3(); 
      allOkResult &= result;
      
      if (!result) 
        Console.WriteLine("--> Failed!");
      else
        Console.WriteLine("--> Succeeded!");
      if (GC) 
        Console.WriteLine("Num vars = " + ClAbstractVariable.NumCreated );

      ////////////////////////// Multiedit ////////////////////////// 
      Console.WriteLine("\nMultiedit:");
      result = Multiedit(); 
      allOkResult &= result;
      
      if (!result) 
        Console.WriteLine("--> Failed!");
      else
        Console.WriteLine("--> Succeeded!");
      if (GC) 
        Console.WriteLine("Num vars = " + ClAbstractVariable.NumCreated );

      ////////////////////////// AddDel ////////////////////////// 
      Console.WriteLine("\nAddDel:");

      int cns = 900, vars = 900, resolves = 10000;

      if (args.Length > 0)
        cns = int.Parse(args[0]);

      if (args.Length > 1)
        vars = int.Parse(args[1]);

      if (args.Length > 2)
        resolves = int.Parse(args[2]);

      result = AddDel(cns, vars, resolves);
      allOkResult &= result;
      
      if (!result) 
        Console.WriteLine("--> Failed!");
      else
        Console.WriteLine("--> Succeeded!");
      if (GC) 
        Console.WriteLine("Num vars = " + ClAbstractVariable.NumCreated );
    }

    private static Random _rnd;
  }
}
