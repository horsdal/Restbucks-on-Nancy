namespace RestBucks.Data
{
  using System;
  using System.Linq;
  using System.Linq.Expressions;

  public interface IRepository<T>
  {
    void MakePersistent(params T[] entities);
    T GetById(long id);
    IQueryable<T> Retrieve(Expression<Func<T, bool>> criteria);
    IQueryable<T> RetrieveAll();
  }
}