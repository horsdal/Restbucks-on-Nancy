namespace RestBucks.Domain.BaseClass
{
  using System.Collections.Generic;

  public interface IValidable
  {
    IEnumerable<string> GetErrorMessages();
  }

}

namespace RestBucks.Domain
{
  using NHibernate.Util;

  using BaseClass;

  public static class ValidatableExtensions
  {
    public static bool IsValid(this IValidable validable)
    {
      return !validable.GetErrorMessages().Any();
    }
  }
}