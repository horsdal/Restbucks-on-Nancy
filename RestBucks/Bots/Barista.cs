using System;
using System.Linq;
using System.Reactive.Linq;
using NHibernate;
using NHibernate.Linq;
using RestBucks.Domain;

namespace RestBucks.Bots
{
    public class Barista
    {
        private readonly ISessionFactory sessionFactory;
        private readonly IDisposable subscription;

        public Barista(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
            subscription = Observable.Interval(TimeSpan.FromSeconds(45))
                .Subscribe(PrepareNextOrder);
        }

        public void PrepareNextOrder(long i)
        {
            using(var session = sessionFactory.OpenSession())
            using(var tx = session.BeginTransaction())
            {
                var order = session.Query<Order>()
                                .OrderBy(o => o.Id)
                                .Where(o => o.Status == OrderStatus.Paid)
                                .FirstOrDefault();
                if (order == null) return;
                
                order.Finish();
                
                tx.Commit();
            }
        }

        public void Dispose()
        {
            subscription.Dispose();
        }
    }
}