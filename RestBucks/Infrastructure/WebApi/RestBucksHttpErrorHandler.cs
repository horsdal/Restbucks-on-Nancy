using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.ApplicationServer.Http.Dispatcher;

namespace RestBucks.Infrastructure.WebApi
{ 
    //This is a nice idea... however, it doesn't work.
    //http://wcf.codeplex.com/discussions/255171
    //public class RestBucksHttpErrorHandler : HttpErrorHandler
    //{
    //    protected override bool OnHandleError(Exception error)
    //    {
    //        return true;
    //    }

    //    protected override HttpResponseMessage OnProvideResponse(Exception error)
    //    {
    //        var response = Responses.BadRequest(error.Message);
    //        return response;
    //    }
    //}
    public class RestBucksHttpErrorHandler : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();    
            }catch(InvalidOrderOperationException ex)
            {
                invocation.ReturnValue = Responses.BadRequest(content: ex.Message);
            }
        }
    }

}