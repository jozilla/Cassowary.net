/*
  Cassowary.net: an incremental constraint solver for .NET
  (http://lumumba.uhasselt.be/jo/projects/cassowary.net/)
  
  Copyright (C) 2005  Jo Vermeulen (jo@lumumba.uhasselt.be)
  
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

namespace Cassowary.Tests
{
  public class LayoutTest 
  {
    public static void Main(string[] args) 
    {
      _solver = new ClSimplexSolver();
      
      // initialize the needed variables
      BuildVariables();
      
      // create the constraints
      try 
      {
        BuildConstraints();
      } 
      catch (ExClRequiredFailure rf) 
      {
        #if !COMPACT
          Console.Error.WriteLine(rf.StackTrace);
        #else
          Console.WriteLine(rf.Message);
        #endif
      } 
      catch (ExClInternalError ie) 
      {
        #if !COMPACT
          Console.Error.WriteLine(ie.StackTrace);
        #else
          Console.WriteLine(ie.Message);
        #endif
      }
      
      // solve it
      try 
      {
        _solver.Solve();
      } 
      catch (ExClInternalError ie) 
      {
        #if !COMPACT
          Console.Error.WriteLine(ie.StackTrace);
        #else
          Console.WriteLine(ie.Message);
        #endif
      }
      
      // print out the values
      Console.WriteLine("Variables: ");
      PrintVariables();
    }
    
    protected static void BuildVariables() 
    {
      ////////////////////////////////////////////////////////////////
      //                 Individual widgets                         // 
      ////////////////////////////////////////////////////////////////
      
      update_top = new ClVariable("update.top", 0);
      update_bottom = new ClVariable("update.bottom", 23);
      update_left = new ClVariable("update.left", 0);
      update_right = new ClVariable("update.right", 75);
      update_height = new ClVariable("update.height", 23);
      update_width = new ClVariable("update.width", 75);
      
      newpost_top = new ClVariable("newpost.top", 0);
      newpost_bottom = new ClVariable("newpost.bottom", 23);
      newpost_left = new ClVariable("newpost.left", 0);
      newpost_right = new ClVariable("newpost.right", 75);
      newpost_width = new ClVariable("newpost.width", 75);
      newpost_height = new ClVariable("newpost.height", 23);
      
      quit_bottom = new ClVariable("quit.bottom", 23);
      quit_right = new ClVariable("quit.right", 75);
      quit_height = new ClVariable("quit.height", 23);
      quit_width = new ClVariable("quit.width", 75);
      quit_left = new ClVariable("quit.left", 0);
      quit_top = new ClVariable("quit.top", 0);
      
      l_title_top = new ClVariable("l_title.top", 0);
      l_title_bottom = new ClVariable("l_title.bottom", 23);
      l_title_left = new ClVariable("l_title.left", 0);
      l_title_right = new ClVariable("l_title.right", 100);
      l_title_height = new ClVariable("l_title.height", 23);
      l_title_width = new ClVariable("l_title.width", 100);
      
      title_top = new ClVariable("title.top", 0);
      title_bottom = new ClVariable("title.bottom", 20);
      title_left = new ClVariable("title.left.", 0);
      title_right = new ClVariable("title.right", 100);
      title_height = new ClVariable("title.height", 20);
      title_width = new ClVariable("title.width", 100);
      
      l_body_top = new ClVariable("l_body.top", 0);
      l_body_bottom = new ClVariable("l_body.bottom", 23);
      l_body_left = new ClVariable("l_body.left", 0);
      l_body_right = new ClVariable("l_body.right", 100);
      l_body_height = new ClVariable("l_body.height.", 23);
      l_body_width = new ClVariable("l_body.width", 100);
      
      blogentry_top = new ClVariable("blogentry.top", 0);
      blogentry_bottom = new ClVariable("blogentry.bottom", 315);
      blogentry_left = new ClVariable("blogentry.left", 0);
      blogentry_right = new ClVariable("blogentry.right", 400);
      blogentry_height = new ClVariable("blogentry.height", 315);
      blogentry_width = new ClVariable("blogentry.width", 400);
      
      
      l_recent_top = new ClVariable("l_recent.top", 0);
      l_recent_bottom = new ClVariable("l_recent.bottom", 23);
      l_recent_left = new ClVariable("l_recent.left", 0);
      l_recent_right = new ClVariable("l_recent.right", 100);
      l_recent_height = new ClVariable("l_recent.height", 23);
      l_recent_width = new ClVariable("l_recent.width", 100);
      
      articles_top = new ClVariable("articles.top", 0);
      articles_bottom = new ClVariable("articles.bottom", 415);
      articles_left = new ClVariable("articles.left", 0);
      articles_right = new ClVariable("articles.right", 180);
      articles_height = new ClVariable("articles.height", 415);
      articles_width = new ClVariable("articles.width", 100);
          
      ////////////////////////////////////////////////////////////////
      //                  Container widgets                         // 
      ////////////////////////////////////////////////////////////////
      
      topRight_top = new ClVariable("topRight.top", 0);
      //topRight_top = new ClVariable("topRight.top", 0);
      topRight_bottom = new ClVariable("topRight.bottom", 100);
      //topRight_bottom = new ClVariable("topRight.bottom", 100);
      topRight_left = new ClVariable("topRight.left", 0);
      //topRight_left = new ClVariable("topRight.left", 0);
      topRight_right = new ClVariable("topRight.right", 200);
      //topRight_right = new ClVariable("topRight.right", 200);
      topRight_height = new ClVariable("topRight.height", 100);
      //topRight_height = new ClVariable("topRight.height", 100);
      topRight_width = new ClVariable("topRight.width", 200);
      //topRight_width = new ClVariable("topRight.width", 200);
      //topRight_width = new ClVariable("topRight.width", 200);

      bottomRight_top = new ClVariable("bottomRight.top", 0);
      //bottomRight_top = new ClVariable("bottomRight.top", 0);
      bottomRight_bottom = new ClVariable("bottomRight.bottom", 100);
      //bottomRight_bottom = new ClVariable("bottomRight.bottom", 100);
      bottomRight_left = new ClVariable("bottomRight.left", 0);
      //bottomRight_left = new ClVariable("bottomRight.left", 0);
      bottomRight_right = new ClVariable("bottomRight.right", 200);
      //bottomRight_right = new ClVariable("bottomRight.right", 200);
      bottomRight_height = new ClVariable("bottomRight.height", 100);
      //bottomRight_height = new ClVariable("bottomRight.height", 100);
      bottomRight_width = new ClVariable("bottomRight.width", 200);
      //bottomRight_width = new ClVariable("bottomRight.width", 200);
      
      right_top = new ClVariable("right.top", 0);
      //right_top = new ClVariable("right.top", 0);
      right_bottom = new ClVariable("right.bottom", 100);
      //right_bottom = new ClVariable("right.bottom", 100);
      //right_bottom = new ClVariable("right.bottom", 100);
      right_left = new ClVariable("right.left", 0);
      //right_left = new ClVariable("right.left", 0);
      right_right = new ClVariable("right.right", 200);
      //right_right = new ClVariable("right.right", 200);
      right_height = new ClVariable("right.height", 100);
      //right_height = new ClVariable("right.height", 100);
      right_width = new ClVariable("right.width", 200);
      //right_width = new ClVariable("right.width", 200);
      //right_width = new ClVariable("right.width", 200);
      
      left_top = new ClVariable("left.top", 0);
      //left_top = new ClVariable("left.top", 0);
      left_bottom = new ClVariable("left.bottom", 100);
      //left_bottom = new ClVariable("left.bottom", 100);
      left_left = new ClVariable("left.left", 0);
      //left_left = new ClVariable("left.left", 0);
      left_right = new ClVariable("left.right", 200);
      //left_right = new ClVariable("left.right", 200);
      left_height = new ClVariable("left.height", 100);
      //left_height = new ClVariable("left.height", 100);
      left_width = new ClVariable("left.width", 200);
      //left_width = new ClVariable("left.width", 200);
      
      fr_top = new ClVariable("fr.top", 0);
      fr_bottom = new ClVariable("fr.bottom", 100);
      fr_left = new ClVariable("fr.left", 0);
      fr_right = new ClVariable("fr.right", 200);
      fr_height = new ClVariable("fr.height", 100);
      fr_width = new ClVariable("fr.width", 200);
    }
    
    protected static void BuildConstraints() 
    {
      BuildStayConstraints();
      BuildRequiredConstraints();
      BuildStrongConstraints();
    }
    
    protected static void BuildStayConstraints() 
    {
      _solver.AddStay(update_height);
      _solver.AddStay(update_width);
      _solver.AddStay(newpost_height);
      _solver.AddStay(newpost_width);
      _solver.AddStay(quit_height);
      _solver.AddStay(quit_width);
      _solver.AddStay(l_title_height);
      _solver.AddStay(l_title_width);
      _solver.AddStay(title_height);
      _solver.AddStay(title_width);
      _solver.AddStay(l_body_height);
      _solver.AddStay(l_body_width);
      _solver.AddStay(blogentry_height);
      // let's keep blogentry.width in favor of other stay constraints!
      // remember we later specify title.width to be equal to blogentry.width
      _solver.AddStay(blogentry_width, ClStrength.Strong);
      _solver.AddStay(l_recent_height);
      _solver.AddStay(l_recent_width);
      _solver.AddStay(articles_height);
      _solver.AddStay(articles_width);
    }
    
    protected static void BuildRequiredConstraints() {
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(bottomRight_height), bottomRight_top), new ClLinearExpression(bottomRight_bottom), ClStrength.Required));
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(bottomRight_width), bottomRight_left), new ClLinearExpression(bottomRight_right), ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(bottomRight_top, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(bottomRight_bottom, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(bottomRight_left, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(bottomRight_right, Cl.GEQ, 0, ClStrength.Required));
      
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(update_height), update_top), new ClLinearExpression(update_bottom), ClStrength.Required));
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(update_width), update_left), new ClLinearExpression(update_right), ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(update_top, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(update_bottom, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(update_left, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(update_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(update_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(update_bottom, Cl.LEQ, bottomRight_height));
      _solver.AddConstraint(new ClLinearInequality(update_right, Cl.LEQ, bottomRight_width));
      
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(newpost_height), newpost_top), new ClLinearExpression(newpost_bottom), ClStrength.Required));
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(newpost_width), newpost_left), new ClLinearExpression(newpost_right), ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(newpost_top, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(newpost_bottom, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(newpost_left, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(newpost_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(newpost_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(newpost_bottom, Cl.LEQ, bottomRight_height));
      _solver.AddConstraint(new ClLinearInequality(newpost_right, Cl.LEQ, bottomRight_width));
      
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(quit_height), quit_top), new ClLinearExpression(quit_bottom), ClStrength.Required));
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(quit_width), quit_left), new ClLinearExpression(quit_right), ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(quit_top, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(quit_bottom, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(quit_left, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(quit_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(quit_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(quit_bottom, Cl.LEQ, bottomRight_height));
      _solver.AddConstraint(new ClLinearInequality(quit_right, Cl.LEQ, bottomRight_width));
      
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(topRight_height), topRight_top), new ClLinearExpression(topRight_bottom), ClStrength.Required));
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(topRight_width), topRight_left), new ClLinearExpression(topRight_right), ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(topRight_top, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(topRight_bottom, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(topRight_left, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(topRight_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(topRight_right, Cl.GEQ, 0, ClStrength.Required));
      
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(l_title_height), l_title_top), new ClLinearExpression(l_title_bottom), ClStrength.Required));
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(l_title_width), l_title_left), new ClLinearExpression(l_title_right), ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_title_top, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_title_bottom, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_title_left, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_title_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_title_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_title_bottom, Cl.LEQ, topRight_height));
      _solver.AddConstraint(new ClLinearInequality(l_title_right, Cl.LEQ, topRight_width));
      
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(title_height), title_top), new ClLinearExpression(title_bottom), ClStrength.Required));
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(title_width), title_left), new ClLinearExpression(title_right), ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(title_top, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(title_bottom, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(title_left, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(title_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(title_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(title_bottom, Cl.LEQ, topRight_height));
      _solver.AddConstraint(new ClLinearInequality(title_right, Cl.LEQ, topRight_width));
      
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(l_body_height), l_body_top), new ClLinearExpression(l_body_bottom), ClStrength.Required));
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(l_body_width), l_body_left), new ClLinearExpression(l_body_right), ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_body_top, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_body_bottom, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_body_left, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_body_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_body_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_body_bottom, Cl.LEQ, topRight_height));
      _solver.AddConstraint(new ClLinearInequality(l_body_right, Cl.LEQ, topRight_width));
      
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(blogentry_height), blogentry_top), new ClLinearExpression(blogentry_bottom), ClStrength.Required));
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(blogentry_width), blogentry_left), new ClLinearExpression(blogentry_right), ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(blogentry_top, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(blogentry_bottom, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(blogentry_left, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(blogentry_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(blogentry_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(blogentry_bottom, Cl.LEQ, topRight_height));
      _solver.AddConstraint(new ClLinearInequality(blogentry_right, Cl.LEQ, topRight_width));
      
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(left_height), left_top), new ClLinearExpression(left_bottom), ClStrength.Required));
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(left_width), left_left), new ClLinearExpression(left_right), ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(left_top, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(left_bottom, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(left_left, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(left_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(left_right, Cl.GEQ, 0, ClStrength.Required));
      
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(l_recent_height), l_recent_top), new ClLinearExpression(l_recent_bottom), ClStrength.Required));
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(l_recent_width), l_recent_left), new ClLinearExpression(l_recent_right), ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_recent_top, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_recent_bottom, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_recent_left, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_recent_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_recent_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(l_recent_bottom, Cl.LEQ, left_height));
      _solver.AddConstraint(new ClLinearInequality(l_recent_right, Cl.LEQ, left_width));
      
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(articles_height), articles_top), new ClLinearExpression(articles_bottom), ClStrength.Required));
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(articles_width), articles_left), new ClLinearExpression(articles_right), ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(articles_top, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(articles_bottom, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(articles_left, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(articles_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(articles_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(articles_bottom, Cl.LEQ, left_height));
      _solver.AddConstraint(new ClLinearInequality(articles_right, Cl.LEQ, left_width));
      
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(left_height), left_top), new ClLinearExpression(left_bottom), ClStrength.Required));
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(left_width), left_left), new ClLinearExpression(left_right), ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(left_top, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(left_bottom, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(left_left, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(left_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(left_right, Cl.GEQ, 0, ClStrength.Required));
      
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(right_height), right_top), new ClLinearExpression(right_bottom), ClStrength.Required));
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(right_width), right_left), new ClLinearExpression(right_right), ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(right_top, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(right_bottom, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(right_left, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(right_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(right_right, Cl.GEQ, 0, ClStrength.Required));
      
      _solver.AddConstraint(new ClLinearInequality(topRight_bottom, Cl.LEQ, right_height));
      _solver.AddConstraint(new ClLinearInequality(topRight_right, Cl.LEQ, right_width));
      
      _solver.AddConstraint(new ClLinearInequality(bottomRight_bottom, Cl.LEQ, right_height));
      _solver.AddConstraint(new ClLinearInequality(bottomRight_right, Cl.LEQ, right_width));
      
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(fr_height), fr_top), new ClLinearExpression(fr_bottom), ClStrength.Required));
      _solver.AddConstraint(new ClLinearEquation(Cl.Plus(new ClLinearExpression(fr_width), fr_left), new ClLinearExpression(fr_right), ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(fr_top, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(fr_bottom, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(fr_left, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(fr_right, Cl.GEQ, 0, ClStrength.Required));
      _solver.AddConstraint(new ClLinearInequality(fr_right, Cl.GEQ, 0, ClStrength.Required));
      
      _solver.AddConstraint(new ClLinearInequality(left_bottom, Cl.LEQ, fr_height));
      _solver.AddConstraint(new ClLinearInequality(left_right, Cl.LEQ, fr_width));
      _solver.AddConstraint(new ClLinearInequality(right_bottom, Cl.LEQ, fr_height));
      _solver.AddConstraint(new ClLinearInequality(right_right, Cl.LEQ, fr_width));
    }
    
    protected static void BuildStrongConstraints() 
    {
      _solver.AddConstraint(new ClLinearInequality(update_right, Cl.LEQ, newpost_left, ClStrength.Strong));
      _solver.AddConstraint(new ClLinearInequality(newpost_right, Cl.LEQ, quit_left, ClStrength.Strong));
      //_solver.AddConstraint(new ClLinearEquation(bottomRight_width, new ClLinearExpression(topRight_width), ClStrength.Strong));
      //_solver.AddConstraint(new ClLinearEquation(right_width, new ClLinearExpression(topRight_width), ClStrength.Strong));
      _solver.AddConstraint(new ClLinearEquation(bottomRight_bottom, new ClLinearExpression(right_bottom), ClStrength.Strong)); 
      _solver.AddConstraint(new ClLinearEquation(newpost_height, new ClLinearExpression(update_height), ClStrength.Strong));
      _solver.AddConstraint(new ClLinearEquation(newpost_width, new ClLinearExpression(update_width), ClStrength.Strong));
      _solver.AddConstraint(new ClLinearEquation(update_height, new ClLinearExpression(quit_height), ClStrength.Strong));
      _solver.AddConstraint(new ClLinearEquation(quit_width, new ClLinearExpression(update_width), ClStrength.Strong));
      
      _solver.AddConstraint(new ClLinearInequality(l_title_bottom, Cl.LEQ, title_top, ClStrength.Strong));
      _solver.AddConstraint(new ClLinearInequality(title_bottom, Cl.LEQ, l_body_top, ClStrength.Strong));
      _solver.AddConstraint(new ClLinearInequality(l_body_bottom, Cl.LEQ, blogentry_top, ClStrength.Strong));
      
      _solver.AddConstraint(new ClLinearEquation(title_width, new ClLinearExpression(blogentry_width), ClStrength.Strong));
      
      _solver.AddConstraint(new ClLinearInequality(l_recent_bottom, Cl.LEQ, articles_top, ClStrength.Strong));
      
      _solver.AddConstraint(new ClLinearInequality(topRight_bottom, Cl.LEQ, bottomRight_top, ClStrength.Strong));
      _solver.AddConstraint(new ClLinearInequality(left_right, Cl.LEQ, right_left, ClStrength.Strong));
      //_solver.AddConstraint(new ClLinearEquation(left_height, new ClLinearExpression(right_height), ClStrength.Strong));
      //_solver.AddConstraint(new ClLinearEquation(fr_height, new ClLinearExpression(right_height), ClStrength.Strong));
      
      // alignment
      _solver.AddConstraint(new ClLinearEquation(l_title_left, new ClLinearExpression(title_left), ClStrength.Strong));
      _solver.AddConstraint(new ClLinearEquation(title_left, new ClLinearExpression(blogentry_left), ClStrength.Strong));
      _solver.AddConstraint(new ClLinearEquation(l_body_left, new ClLinearExpression(blogentry_left), ClStrength.Strong));
      _solver.AddConstraint(new ClLinearEquation(l_recent_left, new ClLinearExpression(articles_left), ClStrength.Strong));
    }
    
    protected static void PrintVariables()
    {
      Console.WriteLine(update_top);
      Console.WriteLine(update_bottom);
      Console.WriteLine(update_left);
      Console.WriteLine(update_right);
      Console.WriteLine(update_height);
      Console.WriteLine(update_width);
      
      Console.WriteLine(newpost_top);
      Console.WriteLine(newpost_bottom);
      Console.WriteLine(newpost_left);
      Console.WriteLine(newpost_right);
      Console.WriteLine(newpost_height);
      Console.WriteLine(newpost_width);
      
      Console.WriteLine(quit_top);
      Console.WriteLine(quit_bottom);
      Console.WriteLine(quit_left);
      Console.WriteLine(quit_right);
      Console.WriteLine(quit_height);
      Console.WriteLine(quit_width);
      
      Console.WriteLine(l_title_top);
      Console.WriteLine(l_title_bottom);
      Console.WriteLine(l_title_left);
      Console.WriteLine(l_title_right);
      Console.WriteLine(l_title_height);
      Console.WriteLine(l_title_width);
      
      Console.WriteLine(title_top);
      Console.WriteLine(title_bottom);
      Console.WriteLine(title_left);
      Console.WriteLine(title_right);
      Console.WriteLine(title_height);
      Console.WriteLine(title_width);
      
      Console.WriteLine(l_body_top);
      Console.WriteLine(l_body_bottom);
      Console.WriteLine(l_body_left);
      Console.WriteLine(l_body_right);
      Console.WriteLine(l_body_height);
      Console.WriteLine(l_body_width);
      
      Console.WriteLine(blogentry_top);
      Console.WriteLine(blogentry_bottom);
      Console.WriteLine(blogentry_left);
      Console.WriteLine(blogentry_right);
      Console.WriteLine(blogentry_height);
      Console.WriteLine(blogentry_width);
      
      Console.WriteLine(l_recent_top);
      Console.WriteLine(l_recent_bottom);
      Console.WriteLine(l_recent_left);
      Console.WriteLine(l_recent_right);
      Console.WriteLine(l_recent_height);
      Console.WriteLine(l_recent_width);
      
      Console.WriteLine(articles_top);
      Console.WriteLine(articles_bottom);
      Console.WriteLine(articles_left);
      Console.WriteLine(articles_right);
      Console.WriteLine(articles_height);
      Console.WriteLine(articles_width);
      
      Console.WriteLine(topRight_top);
      Console.WriteLine(topRight_bottom);
      Console.WriteLine(topRight_left);
      Console.WriteLine(topRight_right);
      Console.WriteLine(topRight_height);
      Console.WriteLine(topRight_width);

      Console.WriteLine(bottomRight_top);
      Console.WriteLine(bottomRight_bottom);
      Console.WriteLine(bottomRight_left);
      Console.WriteLine(bottomRight_right);
      Console.WriteLine(bottomRight_height);
      Console.WriteLine(bottomRight_width);
      
      Console.WriteLine(right_top);
      Console.WriteLine(right_bottom);
      Console.WriteLine(right_left);
      Console.WriteLine(right_right);
      Console.WriteLine(right_height);
      Console.WriteLine(right_width);
      
      Console.WriteLine(left_top);
      Console.WriteLine(left_bottom);
      Console.WriteLine(left_left);
      Console.WriteLine(left_right);
      Console.WriteLine(left_height);
      Console.WriteLine(left_width);
      
      Console.WriteLine(fr_top);
      Console.WriteLine(fr_bottom);
      Console.WriteLine(fr_left);
      Console.WriteLine(fr_right);
      Console.WriteLine(fr_height);
      Console.WriteLine(fr_width);
    }
    
    private static ClSimplexSolver _solver;
    
    private static ClVariable update_top;
    private static ClVariable update_bottom;
    private static ClVariable update_left;
    private static ClVariable update_right;
    private static ClVariable update_height;
    private static ClVariable update_width;
    
    private static ClVariable newpost_top;
    private static ClVariable newpost_bottom;
    private static ClVariable newpost_left;
    private static ClVariable newpost_right;
    private static ClVariable newpost_height;
    private static ClVariable newpost_width;
    
    private static ClVariable quit_top;
    private static ClVariable quit_bottom;
    private static ClVariable quit_left;
    private static ClVariable quit_right; 
    private static ClVariable quit_height;
    private static ClVariable quit_width;
    
    private static ClVariable l_title_top;
    private static ClVariable l_title_bottom;
    private static ClVariable l_title_left;
    private static ClVariable l_title_right;
    private static ClVariable l_title_height;
    private static ClVariable l_title_width;
    
    private static ClVariable title_top;
    private static ClVariable title_bottom;
    private static ClVariable title_left;
    private static ClVariable title_right;
    private static ClVariable title_height;
    private static ClVariable title_width;
    
    private static ClVariable l_body_top;
    private static ClVariable l_body_bottom;
    private static ClVariable l_body_left;
    private static ClVariable l_body_right;
    private static ClVariable l_body_height;
    private static ClVariable l_body_width;
    
    private static ClVariable blogentry_top;
    private static ClVariable blogentry_bottom;
    private static ClVariable blogentry_left;
    private static ClVariable blogentry_right;
    private static ClVariable blogentry_height;
    private static ClVariable blogentry_width;
    
    private static ClVariable l_recent_top;
    private static ClVariable l_recent_bottom;
    private static ClVariable l_recent_left;
    private static ClVariable l_recent_right;
    private static ClVariable l_recent_height;
    private static ClVariable l_recent_width;
    
    private static ClVariable articles_top;
    private static ClVariable articles_bottom;
    private static ClVariable articles_left;
    private static ClVariable articles_right;
    private static ClVariable articles_height;
    private static ClVariable articles_width;
        
    ////////////////////////////////////////////////////////////////
    //                  Container widgets                         // 
    ////////////////////////////////////////////////////////////////
    
    private static ClVariable topRight_top;
    private static ClVariable topRight_bottom;
    private static ClVariable topRight_left;
    private static ClVariable topRight_right;
    private static ClVariable topRight_height;
    private static ClVariable topRight_width;
    
    private static ClVariable bottomRight_top;
    private static ClVariable bottomRight_bottom;
    private static ClVariable bottomRight_left;
    private static ClVariable bottomRight_right;
    private static ClVariable bottomRight_height;
    private static ClVariable bottomRight_width;
    
    private static ClVariable right_top; 
    private static ClVariable right_bottom;
    private static ClVariable right_left;
    private static ClVariable right_right;
    private static ClVariable right_height;
    private static ClVariable right_width;
    
    private static ClVariable left_top;
    private static ClVariable left_bottom;
    private static ClVariable left_left;
    private static ClVariable left_right;
    private static ClVariable left_height;
    private static ClVariable left_width;
    
    private static ClVariable fr_top;
    private static ClVariable fr_bottom;
    private static ClVariable fr_left;
    private static ClVariable fr_right;
    private static ClVariable fr_height;
    private static ClVariable fr_width;
  }
}
