using System.Collections.Generic;
using NHibernate.Util;
using RestBucks.Domain.BaseClass;

namespace RestBucks.Domain.BaseClass
{
    public interface IValidable
    {
        IEnumerable<string> GetErrorMessages();
    }

}

namespace RestBucks.Domain
{
    public static class ValidatableExtensions
    {
        public static bool IsValid(this IValidable validable)
        {
            return !validable.GetErrorMessages().Any();
        }
    }
}