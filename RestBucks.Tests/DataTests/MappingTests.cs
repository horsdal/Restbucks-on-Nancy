using System;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using RestBucks.Data;

namespace RestBucks.Tests.DataTests
{
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
