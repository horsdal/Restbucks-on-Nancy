namespace RestBucks.Domain
{
  using System.Collections.Generic;

  using BaseClass;

  public class Customization : EntityBase
  {
    private readonly ISet<string> possibleValues;

    public Customization()
    {
      possibleValues = new HashSet<string>();
    }

    public virtual string Name { get; set; }

    public virtual ISet<string> PossibleValues
    {
      get { return possibleValues; }
    }
  }
}