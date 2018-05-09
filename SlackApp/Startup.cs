using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SlackAPI;
using SlackApp.Controllers;
using Amazon.Runtime;
using SlackAPIService;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Amazon.DynamoDBv2;
using SlackApp.BotResponses;
using Autofac;
using System.Reflection;
using Amazon.RDS;
using Autofac.Extensions.DependencyInjection;

namespace SlackApp
{
    public class Startup
    {
        public SlackClientService SlackClientProvider { get; private set; }
        public IAmazonService AmazonClient { get; private set; }
        public List<AbstractSocketResponse> BotResponders { get; private set; }
        public IConfiguration Configuration { get; }
        public IContainer ServicesContainer { get; private set; }
        public IHostingEnvironment HostingEnvironment { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            HostingEnvironment = env;

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SLACKAUTHTOKEN")))
            {
                Environment.SetEnvironmentVariable("SLACKAUTHTOKEN", Configuration["SLACKAUTHTOKEN"]);
            }
            if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SLACKSOCKETAUTHTOKEN")))
            {
                Environment.SetEnvironmentVariable("SLACKSOCKETAUTHTOKEN", Configuration["SLACKSOCKETAUTHTOKEN"]);
            }

            services.AddMvc();
            services.AddAutofac();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var dynamoDBClient = new AmazonDynamoDBClient(Configuration.GetAWSOptions().Credentials, Configuration.GetAWSOptions().Region);
            var slackservice = new SlackClientService(Environment.GetEnvironmentVariable("SLACKAUTHTOKEN"), Environment.GetEnvironmentVariable("SLACKSOCKETAUTHTOKEN"));
            var amazonrds = new AmazonRDSClient(Configuration.GetAWSOptions().Credentials, Configuration.GetAWSOptions().Region);

            builder.RegisterInstance(slackservice).As<ISlackClient>();
            builder.RegisterInstance(dynamoDBClient).As<IAmazonDynamoDB>();
            builder.RegisterInstance(amazonrds).As<IAmazonRDS>();

            if (HostingEnvironment.IsDevelopment()) { slackservice.SubscribeToMessage((x) => { Console.WriteLine(x.text); }); }

            var dataAccess = Assembly.GetExecutingAssembly();

            //builder.RegisterType<BotLimiterResponse>();
            builder.RegisterAssemblyTypes(dataAccess)
                   .Where(t => (t.BaseType == typeof(AbstractSocketResponse)))
                   .As<IMessageReceiver>();

            builder.RegisterBuildCallback(x => { this.ContainerBuildCallback(x); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        private void ContainerBuildCallback(IContainer container)
        {
            container.BeginLifetimeScope().Resolve<IEnumerable<IMessageReceiver>>();
        }
    }
}
