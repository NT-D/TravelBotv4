using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder.Ai;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.LUIS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyMiddlewares = TravelBotv4.Middlewares;

namespace TravelBotv4
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_ => Configuration);
            services.AddBot<TravelBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);

                options.Middleware.Add(new MyMiddlewares.ImageMiddleware());
                options.Middleware.Add(new MyMiddlewares.HumanHandoff());

                var qnaOptions = new QnAMakerMiddlewareOptions()
                {
                    KnowledgeBaseId = "<Your ID>",
                    SubscriptionKey = "<Your Subscription Key>",
                    ScoreThreshold = 0.5f,
                    EndActivityRoutingOnAnswer = true
                };
                options.Middleware.Add(new MyMiddlewares.QnAMakerMiddleware(qnaOptions));

                var luisModel = new LuisModel("<Your Model ID>", "<Your subscription Key>", new System.Uri("https://eastasia.api.cognitive.microsoft.com/luis/v2.0/apps/"));
                options.Middleware.Add(new MyMiddlewares.LuisMiddleware(luisModel));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseBotFramework();
        }
    }
}
