using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace TravelBotv4.Middlewares
{
    public class LogMiddleware : IMiddleware
    {
        public Task OnProcessRequest(IBotContext context, MiddlewareSet.NextDelegate next)
        {
            throw new NotImplementedException();
        }
    }
}
