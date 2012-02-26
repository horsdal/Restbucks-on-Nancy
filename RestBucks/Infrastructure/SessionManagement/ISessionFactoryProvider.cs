using System.Collections.Generic;
using NHibernate;

namespace RestBucks.Infrastructure.SessionManagement
{
    public interface ISessionFactoryProvider
    {
        IEnumerable<ISessionFactory> GetSessionFactories();
    }
}