using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Context;
using NHibernate.Engine;
using RestBucks.Data;

namespace RestBucks.Infrastructure.SessionManagement
{
    public class LazySessionContext : ICurrentSessionContext
    {
        private readonly ISessionFactoryImplementor factory;

        public LazySessionContext(ISessionFactoryImplementor factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Retrieve the current session for the session factory.
        /// </summary>
        /// <returns></returns>
        public ISession CurrentSession()
        {
            Lazy<ISession> initializer;
            var currentSessionFactoryMap = GetCurrentFactoryMap();
            if (currentSessionFactoryMap == null || !currentSessionFactoryMap.TryGetValue(factory, out initializer))
            {
                return null;
            }
            return initializer.Value;
        }

        /// <summary>
        /// Bind a new sessionInitializer to the context of the sessionFactory.
        /// </summary>
        /// <param name="sessionInitializer"></param>
        /// <param name="sessionFactory"></param>
        public static void Bind(Lazy<ISession> sessionInitializer, ISessionFactory sessionFactory)
        {
            var map = GetCurrentFactoryMap();
            map[sessionFactory] = sessionInitializer;
        }

        /// <summary>
        /// Unbind the current session of the session factory.
        /// </summary>
        /// <param name="sessionFactory"></param>
        /// <returns></returns>
        public static ISession UnBind(ISessionFactory sessionFactory)
        {
            var map = GetCurrentFactoryMap();
            var sessionInitializer = map[sessionFactory];
            map[sessionFactory] = null;
            if (sessionInitializer == null || !sessionInitializer.IsValueCreated) return null;
            return sessionInitializer.Value;
        }

        /// <summary>
        /// Provides the CurrentMap of SessionFactories.
        /// If there is no map create/store and return a new one.
        /// </summary>
        /// <returns></returns>
        private static IDictionary<ISessionFactory, Lazy<ISession>> GetCurrentFactoryMap()
        {
            //var currentFactoryMap = (IDictionary<ISessionFactory, Lazy<ISession>>)HttpContext.Current.Items[CurrentSessionContextKey];
            //if (currentFactoryMap == null)
            //{
            //    currentFactoryMap = new Dictionary<ISessionFactory, Lazy<ISession>>();
            //    HttpContext.Current.Items[CurrentSessionContextKey] = currentFactoryMap;
            //}

            if (currentFactoryMap == null)
            {
                currentFactoryMap = new Dictionary<ISessionFactory, Lazy<ISession>>();
            }
            return currentFactoryMap;
        }

        [ThreadStatic]
        private static IDictionary<ISessionFactory, Lazy<ISession>> currentFactoryMap;
    }


}