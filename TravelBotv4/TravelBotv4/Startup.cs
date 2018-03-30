using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyMiddlewares = TravelBotv4.Middlewares;
using Underscore.Bot.MessageRouting;
using Underscore.Bot.MessageRouting.DataStore;
using Underscore.Bot.MessageRouting.DataStore.Azure;
using Underscore.Bot.MessageRouting.DataStore.Local;
using TravelBotv4.Settings;
using TravelBotv4.MessageRouting;
using TravelBotv4.CommandHandling;


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

                options.Middleware.Add(new ConversationState<BotConversationState>(new MemoryStorage()));
                options.Middleware.Add(new UserState<BotUserState>(new MemoryStorage()));
                options.Middleware.Add(new MyMiddlewares.ImageMiddleware(0.7f));
                options.Middleware.Add(new MyMiddlewares.HumanHandoff());
                options.EnableProactiveMessages = true;

                //var qnaOptions = new QnAMakerMiddlewareOptions()
                //{
                //    KnowledgeBaseId = "<Your ID>",
                //    SubscriptionKey = "<Your Subscription Key>",
                //    ScoreThreshold = 0.5f,
                //    EndActivityRoutingOnAnswer = true
                //};
                //options.Middleware.Add(new MyMiddlewares.QnAMakerMiddleware(qnaOptions));

                //var luisModel = new LuisModel("<Your Model ID>", "<Your subscription Key>", new System.Uri("https://eastasia.api.cognitive.microsoft.com/luis/v2.0/apps/"));
                //options.Middleware.Add(new MyMiddlewares.LuisMiddleware(luisModel));
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
            InitializeMessageRouting();
        }

        public void InitializeMessageRouting()
        {
            string connectionString = Configuration.GetConnectionString("RoutingDataStorageConnectionString");
            Settings = new BotSettings();
            Settings.ConnectionString = connectionString;
            IRoutingDataManager routingDataManager = null;

            if (string.IsNullOrEmpty(connectionString))
            {
                System.Diagnostics.Debug.WriteLine($"WARNING!!! No connection string found - using {nameof(LocalRoutingDataManager)}");
                routingDataManager = new LocalRoutingDataManager();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Found a connection string - using {nameof(AzureTableStorageRoutingDataManager)}");
                routingDataManager = new AzureTableStorageRoutingDataManager(connectionString);
            }

            MessageRouterManager = new MessageRouterManager(routingDataManager);
            MessageRouterResultHandler = new MessageRouterResultHandler(MessageRouterManager);
            CommandMessageHandler = new CommandMessageHandler(MessageRouterManager, MessageRouterResultHandler);
            BackChannelMessageHandler = new BackChannelMessageHandler(MessageRouterManager.RoutingDataManager);
        }

        public static BotSettings Settings
        {
            get;
            private set;
        }

        public static MessageRouterManager MessageRouterManager
        {
            get;
            private set;
        }

        public static MessageRouterResultHandler MessageRouterResultHandler
        {
            get;
            private set;
        }

        public static CommandMessageHandler CommandMessageHandler
        {
            get;
            private set;
        }

        public static BackChannelMessageHandler BackChannelMessageHandler
        {
            get;
            private set;
        }
    }
}
