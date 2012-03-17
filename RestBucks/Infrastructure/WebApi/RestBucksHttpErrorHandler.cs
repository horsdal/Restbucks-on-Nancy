namespace RestBucks.Infrastructure.WebApi
{
  using Castle.DynamicProxy;

  // TODO Move to Nancy error handling pipeline
  public class RestBucksHttpErrorHandler : IInterceptor
  {
    public void Intercept(IInvocation invocation)
    {
      try
      {
        invocation.Proceed();
      }
      catch (InvalidOrderOperationException ex)
      {
        invocation.ReturnValue = ResponseHelpers.BadRequest(null, content: ex.Message);
      }
    }
  }
}