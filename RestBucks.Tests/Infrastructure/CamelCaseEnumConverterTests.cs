using System;
using System.Linq;
using NUnit.Framework;
using Nancy;
using Nancy.Json;
using RestBucks.Infrastructure;

namespace RestBucks.Tests.Infrastructure
{
    public class CamelCaseEnumConverterTests
    {
        private CamelCaseEnumConverter converter;

        [SetUp]
        public void SetUp()
        {
            converter = new CamelCaseEnumConverter();
        }

        [Test]
        public void SupportedTypes()
        {
            var supportedTypes = converter.SupportedTypes.ToList();

            Assert.That(supportedTypes.Count(), Is.EqualTo(1));
            Assert.That(supportedTypes.ElementAt(0), Is.EqualTo(typeof(Enum)));
        }

        [Test]
        public void Enum_is_serialized_to_camel_case_string()
        {
            var result = converter.Serialize(TestEnum.OneOne, new JavaScriptSerializer()) as DynamicDictionaryValue;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("oneOne"));
        }

        private enum TestEnum
        {
            OneOne,
        }
    }
}
