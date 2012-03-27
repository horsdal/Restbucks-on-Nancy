namespace RestBucks.Tests.DataTests
{
  using System;

  using NHibernate.Mapping.ByCode;

  using NUnit.Framework;

  using RestBucks.Data;

  public class MappingTests
  {
    [Test]
    public void GenerateMappings()
    {
      var mappings = Mapper.Generate();
      Console.WriteLine(mappings.AsString());
    }
  }
}
