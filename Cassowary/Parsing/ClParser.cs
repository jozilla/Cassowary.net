using System.IO;
using System.Text;
using System.Collections;

using Cassowary;
using Cassowary.Parsing;

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
