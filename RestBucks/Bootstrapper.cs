using System.Collections.Generic;
using Iesi.Collections.Generic;
using Nancy.Bootstrapper;

namespace RestBucks
{
  using System;
  using System.Yaml.Serialization;
  using Bots;

  using Castle.Facilities.TypedFactory;
  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.Resolvers.SpecializedResolvers;
  using Castle.Windsor;
  using Castle.Windsor.Installer;

  using Infrastructure;

  using NHibernate;
  using NHibernate.Context;

  using Nancy;
  using Nancy.Bootstrappers.Windsor;
  using Nancy.Conventions;
  using Nancy.Responses;
  using Nancy.Diagnostics;

  public class Bootstrapper : WindsorNancyBootstrapper
  {
    protected override DiagnosticsConfiguration DiagnosticsConfiguration
    {
      get
      {
        return new DiagnosticsConfiguration { Password = "RestBucksOnNancy" };
      }
    }

    protected override void ConfigureApplicationContainer(IWindsorContainer existingContainer)
    {
      base.ConfigureApplicationContainer(existingContainer);

      existingContainer.Register(
        Component.For<YamlSerializer>()
        .Instance(new YamlSerializer())
        .LifestyleSingleton());

      existingContainer.Register(
        Component.For<YamlWrapper>()
        .UsingFactoryMethod(
          () => new YamlWrapper(existingContainer.Resolve<YamlSerializer>())));
      
      existingContainer.Kernel.Resolver.AddSubResolver(new CollectionResolver(existingContainer.Kernel, true));
      existingContainer.Install(FromAssembly.This());
    }

    protected override void ApplicationStartup(IWindsorContainer container, IPipelines pipelines)
    {
      base.ApplicationStartup(container, pipelines);

      container.Resolve<Barista>();

      ConfigureNHibernateSessionPerRequest(container, pipelines);
    }

    private void ConfigureNHibernateSessionPerRequest(IWindsorContainer container, IPipelines pipelines)
    {
      pipelines.BeforeRequest += ctx => CreateSession(container);
      pipelines.BeforeRequest += ServeIndexPage;
      pipelines.AfterRequest += ctx => CommitSession(container);
      pipelines.OnError += (ctx, ex) => RollbackSession(container);
      pipelines.OnError += InvalidOrderOperationHandler;
    }

    private Response ServeIndexPage(NancyContext context)
    {
      if (context.Request.Url.Path == "/index.htm" || context.Request.Path == "/" )
        return new GenericFileResponse("index.htm");
      else
        return null;
    }

    protected override void ConfigureConventions(NancyConventions nancyConventions)
    {
      base.ConfigureConventions(nancyConventions);

      nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("docs", "docs"));
      nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("styles", "styles"));
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
      if (CurrentSessionContext.HasBind(sessionFactory))
      {
        var requestSession = sessionFactory.GetCurrentSession();
        requestSession.Transaction.Rollback();
        CurrentSessionContext.Unbind(sessionFactory);
        requestSession.Dispose();
      }
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
      if (CurrentSessionContext.HasBind(sessionFactory))
      {
        var requestSession = sessionFactory.GetCurrentSession();
        requestSession.Transaction.Commit();
        CurrentSessionContext.Unbind(sessionFactory);
        requestSession.Dispose();
      }
      return null;
    }    
  }
}