using Business.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Filters
{
    public class MarketValedationExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var action = context.ActionDescriptor.DisplayName;
            var callStack = context.Exception.StackTrace;
            var exceptionMessage = context.Exception.Message;

            if (context.Exception is MarketException)
            {
                context.Result = new ContentResult
                {
                    Content = $"Calling {action} failed, because: {exceptionMessage}.",
                    StatusCode = 400
                };
            }
            else
            {
                if (context.Exception is AggregateException aggregateException)
                {
                    exceptionMessage = "Several exceptions might happen" +
                        string.Join(';', aggregateException.InnerExceptions.Select(e => e.Message));
                }
                context.Result = new ContentResult
                {
                    Content = $"Calling {action} failed, because: {exceptionMessage}. Callstack: {callStack}.",
                    StatusCode = 500
                };
            }
            context.ExceptionHandled = true;
        }
    }
}
