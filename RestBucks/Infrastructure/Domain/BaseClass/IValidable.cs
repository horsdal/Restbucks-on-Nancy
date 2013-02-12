namespace RestBucks.Infrastructure.Domain.BaseClass
{
  using System.Collections.Generic;
  using NHibernate.Util;

  public interface IValidable
  {
    IEnumerable<string> GetErrorMessages();
  }

  public static class ValidatableExtensions
  {
    public static bool IsValid(this IValidable validable)
    {
      return !validable.GetErrorMessages().Any();
    }
  }
}