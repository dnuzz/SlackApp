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

namespace SlackApp
{
    public class Startup
    {
        public SlackClientService SlackClientProvider { get; private set; }
        public IAmazonService AmazonClient { get; private set; }
        public List<AbstractSocketResponse> BotResponders { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

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
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonDynamoDB>();

            SlackClientProvider = new SlackClientService(Environment.GetEnvironmentVariable("SLACKAUTHTOKEN"), Environment.GetEnvironmentVariable("SLACKSOCKETAUTHTOKEN"));
            BotResponders = new List<AbstractSocketResponse>();

            BotResponders.Add(new BotLimiterResponse(SlackClientProvider));
            BotResponders.Add(new ConversionResponse(SlackClientProvider));
            BotResponders.Add(new RegexResponse(SlackClientProvider));
            BotResponders.Add(new DeleteResponse(SlackClientProvider));
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
    }
}
