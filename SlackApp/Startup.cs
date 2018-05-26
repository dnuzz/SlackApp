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
            //This will load env variables from your configuration if you are storing them there for development.
            var env_variables = new List<string>() { "BOT_AUTH_TOKEN", "DB_CONNECTION_STRING" };
            foreach(var env in env_variables)
            {
                if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable(env)))
                {
                    Environment.SetEnvironmentVariable(env, Configuration[env]);
                }
            }

            var db_conn_string = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            var auth_token = Environment.GetEnvironmentVariable("BOT_AUTH_TOKEN");

            // ASP.NET CORE Services configuration
            services.AddDbContext<AppResponseContext>(options =>
                options.UseSqlServer(db_conn_string));
            services.AddMvc();
            services.AddAutofac();

            //Autofac configuration
            var builder = new ContainerBuilder();
            var slackservice = new SlackClientService(auth_token, auth_token);

            builder.RegisterInstance(slackservice).As<ISlackClient>();
            builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(AbstractSocketResponse)))
               .Where(t => t.BaseType == typeof(AbstractSocketResponse))
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();
            builder.RegisterBuildCallback(x => { this.ContainerBuildCallback(x); });
            builder.Populate(services);

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
            container.BeginLifetimeScope().Resolve<IEnumerable<IMessageReceiver>>().ToList();
            DbInitializer.Initialize(container.BeginLifetimeScope().Resolve<AppResponseContext>());
        }
    }
}
