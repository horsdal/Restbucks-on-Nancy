using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NHibernate;

namespace RestBucks.Infrastructure.SessionManagement
{
    public class NHibernateMessageChannel : DelegatingChannel
    {
        private readonly ISessionFactoryProvider sessionFactoryProvider;

        public NHibernateMessageChannel(HttpMessageChannel innerChannel, ISessionFactoryProvider sessionFactoryProvider) 
            : base(innerChannel)
        {
            this.sessionFactoryProvider = sessionFactoryProvider;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            OpenSessions();
            var sendAsync = base.SendAsync(request, cancellationToken);
            sendAsync.ContinueWith(EndSessions,cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current );
            return sendAsync;
        }

        private void OpenSessions()
        {
            foreach (var sf in sessionFactoryProvider.GetSessionFactories())
            {
                var localFactory = sf;
                var sessionInitializer = new Lazy<ISession>(() => BeginSession(localFactory));
                LazySessionContext.Bind(sessionInitializer, sf);
            }
        }

        private static ISession BeginSession(ISessionFactory sf)
        {
            var session = sf.OpenSession();
            session.BeginTransaction();
            return session;
        }

        private void EndSessions(Task<HttpResponseMessage> obj)
        {
            var sessionsToClose = sessionFactoryProvider
                .GetSessionFactories()
                .Select(LazySessionContext.UnBind)
                .Where(session => session != null);

            foreach (var session in sessionsToClose)
            {
                if (session.Transaction != null && session.Transaction.IsActive)
                {
                    session.Transaction.Commit();
                }
                session.Dispose();
            }
        }
    }
}