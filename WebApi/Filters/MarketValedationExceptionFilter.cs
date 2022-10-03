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
            var exceptionMessage = context.Exception.Message;

            if (context.Exception is MarketException aggregateException)
            {
                context.Result = new ContentResult
                {
                    Content = $"Calling {action} failed, because: {exceptionMessage}.",
                    StatusCode = 400
                };
            }
            context.ExceptionHandled = true;
        }
    }
}
