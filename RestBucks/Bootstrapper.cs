namespace RestBucks
{
  using System;
  using System.ComponentModel;

  using Bots;

  using Castle.Facilities.TypedFactory;
  using Castle.MicroKernel.Resolvers.SpecializedResolvers;
  using Castle.Windsor;
  using Castle.Windsor.Installer;

  using Infrastructure;

  using NHibernate;
  using NHibernate.Context;

  using Nancy;
  using Nancy.Bootstrappers.Windsor;

  public class Bootstrapper : WindsorNancyBootstrapper
  {
    protected override void ConfigureApplicationContainer(Castle.Windsor.IWindsorContainer existingContainer)
    {
      base.ConfigureApplicationContainer(existingContainer);

      existingContainer.AddFacility<TypedFactoryFacility>();
      existingContainer.Kernel.Resolver.AddSubResolver(new CollectionResolver(existingContainer.Kernel, true));
      existingContainer.Install(FromAssembly.This());
    }

    protected override void ApplicationStartup(Castle.Windsor.IWindsorContainer container, Nancy.Bootstrapper.IPipelines pipelines)
    {
      base.ApplicationStartup(container, pipelines);

      container.Resolve<Barista>();

      pipelines.BeforeRequest += ctx => CreateSession(container);
      pipelines.AfterRequest += ctx => CommitSession(container);
      pipelines.OnError += (ctx, ex) => RollbackSession(container);
      pipelines.OnError += InvalidOrderOperationHandler;
    }

    private Response InvalidOrderOperationHandler(NancyContext ctx, Exception ex)
    {
      if (ex is InvalidOrderOperationException)
        return ResponseHelpers.BadRequest(null, content: ex.Message);
      else
        return null;
    }

    private Response RollbackSession(IWindsorContainer container)
    {
      var sessionFactory = container.Resolve<ISessionFactory>();
      var requestSession = sessionFactory.GetCurrentSession();
      requestSession.Transaction.Rollback();
      CurrentSessionContext.Unbind(sessionFactory);
      requestSession.Dispose();

      return null;
    }

    private Response CreateSession(IWindsorContainer container)
    {
      var sessionFactory = container.Resolve<ISessionFactory>();
      var requestSession = sessionFactory.OpenSession();
      CurrentSessionContext.Bind(requestSession);
      requestSession.BeginTransaction();

      return null;
    }

    private AfterPipeline CommitSession(IWindsorContainer container)
    {
      var sessionFactory = container.Resolve<ISessionFactory>();
      var requestSession = sessionFactory.GetCurrentSession();
      requestSession.Transaction.Commit();
      CurrentSessionContext.Unbind(sessionFactory);
      requestSession.Dispose();

      return null;
    }    
  }
}