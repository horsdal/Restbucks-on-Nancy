using NUnit.Framework;
using RestBucks.Infrastructure.Linking;
using RestBucks.Resources.Products;
using SharpTestsEx;

namespace RestBucks.Tests
{
    [TestFixture]
    public class LocationsSpecifications
    {
        
        protected IResourceLinker resourceLinker 
            = new ResourceLinker();
        
        [Test]
        public void TheUriOfTheMenuIsOk()
        {
            resourceLinker.GetUri<MenuResourceHandler>(pr => pr.Get())
                .Should().Be.EqualTo("http://restbuckson.net/menu");
        }

        //[Test]
        //public void TheUriToGetAProductIsOk()
        //{
        //    resourceLinker.GetUri<MenuResourceHandler>(pr => pr.GetProductById(0), 
        //                                                   new{productId = 123})
        //        .Should().Be.EqualTo("http://restbuckson.net/products/123");
        //}
    }
}