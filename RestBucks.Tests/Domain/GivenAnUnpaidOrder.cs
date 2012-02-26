using System.Linq;
using NUnit.Framework;
using RestBucks.Domain;
using RestBucks.Resources.Orders.Representations;
using SharpTestsEx;

namespace RestBucks.Tests.Domain
{
    [TestFixture]
    public class GivenAnUnpaidOrder
    {
        private Order order;
        private OrderRepresentation representation;
        [SetUp]
        public void SetUp()
        {
            order = new Order();
            representation = OrderRepresentationMapper.Map(order);
        }
        [Test]
        public void ThenNextStepsIncludeCancel()
        {
            representation.Links
                .Satisfy(links => links.Any(l => l.Uri == "http://restbuckson.net/order/0" && l.Relation.EndsWith("docs/order-cancel.htm")));
        }

        [Test]
        public void ThenNextStepsIncludeGet()
        {
            representation.Links
                .Satisfy(links => links.Any(l => l.Uri == "http://restbuckson.net/order/0" && l.Relation.EndsWith("docs/order-get.htm")));
        }

        [Test]
        public void TheNextStepsIncludeUpdate()
        {
            representation.Links
                .Satisfy(links => links.Any(l => l.Uri == "http://restbuckson.net/order/0" && l.Relation.EndsWith("docs/order-update.htm")));
        }

        [Test]
        public void TheNextStepsIncludePay()
        {
            representation.Links
                .Satisfy(links => links.Any(l => l.Uri == "http://restbuckson.net/order/0/payment" && l.Relation.EndsWith("docs/order-pay.htm")));
        }

        [Test]
        public void NextStepShouldNotIncludeReceipt()
        {
            representation.Links
                .Satisfy(links => !links.Any(l => l.Uri == "http://restbuckson.net/order/ready/0" && l.Relation.EndsWith("docs/receipt-coffee.htm")));
        }

        [Test]
        public void CancelShouldWork()
        {
            order.Cancel("error");
            order.Status.Should().Be.EqualTo(OrderStatus.Canceled);
        }

        [Test]
        public void PayShouldWork()
        {
            order.Pay("Jose", "123123123");
            order.Status.Should().Be.EqualTo(OrderStatus.Paid);
        }
    }
}