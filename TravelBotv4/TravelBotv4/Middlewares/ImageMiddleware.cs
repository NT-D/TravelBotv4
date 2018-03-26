using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;


namespace TravelBotv4.Middlewares
{
    public class ImageMiddleware : IMiddleware
    {
        public async Task OnProcessRequest(IBotContext context, MiddlewareSet.NextDelegate next)
        {
            //Need to implement Bing Image Search Service and get best search term.
            await context.SendActivity("I'm CustomVisionMiddleware");
            await next();
        }
    }
}
