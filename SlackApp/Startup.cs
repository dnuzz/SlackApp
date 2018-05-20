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
using Microsoft.EntityFrameworkCore;
using SlackApp.Data;

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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SLACKAUTHTOKEN")))
            {
                Environment.SetEnvironmentVariable("SLACKAUTHTOKEN", Configuration["SLACKAUTHTOKEN"]);
            }
            if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SLACKSOCKETAUTHTOKEN")))
            {
                Environment.SetEnvironmentVariable("SLACKSOCKETAUTHTOKEN", Configuration["SLACKSOCKETAUTHTOKEN"]);
            }
            if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("slackapidbconnection")))
            {
                Environment.SetEnvironmentVariable("slackapidbconnection", Configuration["slackapidbconnection"]);
            }

            services.AddDbContext<AppResponseContext>(options =>
                options.UseSqlServer(Environment.GetEnvironmentVariable("slackapidbconnection")));
            services.AddMvc();
            services.AddAutofac();

            var slackservice = new SlackClientService(Environment.GetEnvironmentVariable("SLACKAUTHTOKEN"), Environment.GetEnvironmentVariable("SLACKSOCKETAUTHTOKEN"));
            var builder = new ContainerBuilder();

            builder.RegisterInstance(slackservice).As<ISlackClient>();
            builder.Populate(services);
            builder.RegisterBuildCallback(x => { this.ContainerBuildCallback(x); });

            var container = builder.Build();
            //Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(container);
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
            DbInitializer.Initialize(container.BeginLifetimeScope().Resolve<AppResponseContext>());
        }
    }
}
