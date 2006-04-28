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

using System.IO;
using System.Text;
using System.Collections;

using Cassowary;
using Cassowary.Parsing;

namespace Cassowary.Parsing
{
  public class ClParser
  {
    private string _rule;
    private ClConstraint _result = null;
    private Hashtable _context;

    public ClParser()
    {
      _context = new Hashtable();
    }

    public void Parse(string rule)
    {
      Rule = rule;
      Parse();
    }
    
    public void Parse()
    {
      UTF8Encoding ue = new UTF8Encoding();
      byte[] ruleBytes = ue.GetBytes(Rule);
      MemoryStream ms = new MemoryStream(ruleBytes);

      Scanner s = new Scanner(ms);
      Parser p = new Parser(s);
      p.Context = Context;

      p.Parse();
      
      _result = p.Value;
      
      if (p.errors.count > 0)
        throw new ExClParseError(Rule);
    }

    public void AddContext(params ClVariable[] vars)
    {
      foreach (ClVariable v in vars)
        _context.Add(v.Name, v);
    }

    public void AddContext(Hashtable context)
    {
      _context = new Hashtable(context);
    }
    
    public string Rule
    {
      get { return _rule; }
      set { _rule = value; }
    }
    
    public ClConstraint Result
    {
      get { return _result; }
    }

    public Hashtable Context
    {
      get { return _context; }
    }
  }
}
