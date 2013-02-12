namespace RestBucks.Data.Impl
{
  using System;
  using System.Linq;
  using System.Linq.Expressions;

  using NHibernate;
  using NHibernate.Linq;

  public class Repository<T> : IRepository<T>
  {
    private readonly ISession session;
    private readonly ISessionFactory sessionFactory;

    public Repository(ISession session)
    {
      this.session = session;
    }

    public Repository(ISessionFactory sessionFactory)
    {
      this.sessionFactory = sessionFactory;
    }

    private ISession CurrentSession
    {
      get { return session ?? sessionFactory.GetCurrentSession(); }
    }

    public void MakePersistent(params T[] entities)
    {
      foreach (var entity in entities)
      {
        CurrentSession.Save(entity);
      }
    }

    public T GetById(long id)
    {
      return CurrentSession.Get<T>(id);
    }

    public IQueryable<T> Retrieve(Expression<Func<T, bool>> criteria)
    {
      return CurrentSession.Query<T>().Where(criteria);
    }

    public IQueryable<T> RetrieveAll()
    {
      return CurrentSession.Query<T>();
    }
  }
}