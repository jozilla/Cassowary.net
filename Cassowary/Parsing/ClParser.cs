using System.IO;
using System.Text;
using System.Collections;

using Cassowary;
using Cassowary.Parsing;

public class ClParser
{
  private string _rule;
  private ClConstraint _result = null;
  private Hashtable _context = null;

  public ClParser()
  {}

  public void Parse(string rule)
  {
    Rule = rule;
    
    UTF8Encoding ue = new UTF8Encoding();
    byte[] ruleBytes = ue.GetBytes(Rule);
    MemoryStream ms = new MemoryStream(ruleBytes);

    Scanner s = new Scanner(ms);
    Parser p = new Parser(s);
    p.Parse();
    
    Context = p.Context;
    Result = p.Value;
  }

  public string Rule
  {
    get { return _rule; }
    set { _rule = value; }
  }
  
  public ClConstraint Result
  {
    get { return _result; }
    set { _result = value; }
  }

  public Hashtable Context
  {
    get { return _context; }
    set { _context = value; }
  }
}
