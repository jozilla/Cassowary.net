using Cassowary;

namespace Cassowary.Parsing
{
  public class ExClParseError : ExClError
  {
    private string _rule;

    public ExClParseError(string rule)
    {
      Rule = rule;
    }

    public override string Description()
    {
      return string.Format("[ExClParseError] Parse error in \"{0}\"", Rule);
    }

    public string Rule
    {
      get { return _rule; }
      set { _rule = value; }
    }
  }
}
