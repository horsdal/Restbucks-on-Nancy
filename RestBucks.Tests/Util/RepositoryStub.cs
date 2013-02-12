namespace RestBucks.Tests.Util
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Linq.Expressions;

  using RestBucks.Data;
  using RestBucks.Infrastructure.Domain.BaseClass;

  public class RepositoryStub<T> : IRepository<T> where T : EntityBase
  {
    private readonly ISet<T> entities;

    public RepositoryStub(params T[] entities)
    {
      this.entities = new HashSet<T>(entities);
    }

    public T GetById(long id)
    {
      return entities.FirstOrDefault(e => e.Id == id);
    }

    public void MakePersistent(params T[] newEntities)
    {
      foreach (var entity in newEntities)
      {
        entities.Add(entity);
      }
    }

    public void MakeTransient(T entity)
    {
      entities.Remove(entity);
    }

    public void Update(T entity)
    {
      //no op
    }

    public IQueryable<T> Retrieve(Expression<Func<T, bool>> expression)
    {
      return entities.AsQueryable().Where(expression);
    }

    public IQueryable<T> RetrieveAll()
    {
      return entities.AsQueryable();
    }
  }
}