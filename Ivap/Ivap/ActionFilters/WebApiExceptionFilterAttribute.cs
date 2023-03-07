using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Ivap.ActionFilters
{
    
    public class WebApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError) {

                Content = new StringContent("Something went while executing your request."),
                ReasonPhrase = "Something went while executing your request."
            };

        }
    }
}