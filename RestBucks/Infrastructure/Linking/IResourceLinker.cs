using System;
using System.Linq.Expressions;

namespace RestBucks.Infrastructure.Linking
{
    public interface IResourceLinker
    {
        string GetUri<T>(Expression<Action<T>> method, object uriArgs = null);
        //Link GetLink<T>(Expression<Action<T>> restMethod, object argsObject);
    }
}