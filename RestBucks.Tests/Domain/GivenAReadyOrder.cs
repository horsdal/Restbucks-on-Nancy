using System.Linq;
using NUnit.Framework;
using RestBucks.Domain;
using RestBucks.Infrastructure;
using RestBucks.Resources.Orders.Representations;
using SharpTestsEx;

namespace RestBucks.Tests.Domain
{
    public class GivenAReadyOrder
    {
        private Order order;
        private OrderRepresentation representation;

        [SetUp]
        public void SetUp()
        {
            order = new Order();
            order.Pay("123", "jose");
            order.Finish();
            
            representation = OrderRepresentationMapper.Map(order);
        }

        [Test]
        public void ThenNextStepsIncludeGet()
        {
            representation.Links
                .Satisfy(links => links.Any(l => l.Uri == "http://restbuckson.net/order/0/receipt" && l.Relation.EndsWith("docs/receipt-coffee.htm")));
        }

        [Test]
        public void CancelShouldThrow()
        {
            order.Executing(o => o.Cancel("error"))
                .Throws<InvalidOrderOperationException>()
                .And
                .Exception.Message.Should().Be.EqualTo("The order can not be canceled because it is ready.");
        }

        [Test]
        public void PayShouldThrow()
        {
            order.Executing(o => o.Pay("a", "b"))
                .Throws<InvalidOrderOperationException>()
                .And
                .Exception.Message.Should().Be.EqualTo("The order can not be paid because it is ready.");
        }
    }
}