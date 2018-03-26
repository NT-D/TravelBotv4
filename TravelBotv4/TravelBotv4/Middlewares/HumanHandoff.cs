using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Underscore.Bot.MessageRouting;
using Underscore.Bot.MessageRouting.DataStore.Azure;
using Underscore.Bot.MessageRouting.DataStore.Local;


namespace TravelBotv4.Middlewares
{
    public class HumanHandoff : IMiddleware
    {
        public Task OnProcessRequest(IBotContext context, MiddlewareSet.NextDelegate next)
        {
            MessageRouterManager messageRouterManager = Startup.MessageRouterManager;
            var activity = context.Request.AsMessageActivity();

            string message = context.Request.Text;
            if(hasKeyword(message))
            {
                // TODO Implement the logic to connect to agent
                return null;
            }
            else
            {
                return next();
            }
            throw new NotImplementedException();
        }

        private bool hasKeyword(string message)
        {
            return message.Contains("human");
        }
    }
}
