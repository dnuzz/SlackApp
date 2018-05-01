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

namespace SlackApp
{
    public class Startup
    {
        public SlackSocketClient slackSocketClient { get; private set; }
        public SlackClient slackNormalClient { get; private set; }
        public BotRegexResponder bot { get; private set; }

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
            services.AddMvc();
            if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SLACKAUTHTOKEN")))
            {
                Environment.SetEnvironmentVariable("SLACKAUTHTOKEN", Configuration["SLACKAUTHTOKEN"]);
            }
            if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SLACKAUTHTOKEN2")))
            {
                Environment.SetEnvironmentVariable("SLACKAUTHTOKEN2", Configuration["SLACKAUTHTOKEN2"]);
            }

            slackSocketClient = BuildSlackSocketClient(Environment.GetEnvironmentVariable("SLACKAUTHTOKEN"));
            slackNormalClient = BuildSlackClient(Environment.GetEnvironmentVariable("SLACKAUTHTOKEN2"));
            bot = new BotRegexResponder(slackSocketClient, slackNormalClient);
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

        public SlackSocketClient BuildSlackSocketClient(string auth_token)
        {
            ManualResetEventSlim clientReady = new ManualResetEventSlim(false);
            SlackSocketClient client = new SlackSocketClient(auth_token);
            client.Connect((connected) => {
                // This is called once the client has emitted the RTM start command
                clientReady.Set();
            }, () => {
                Console.WriteLine("Connected to Slack");
            });
            clientReady.Wait();

            return client;
        }

        public SlackClient BuildSlackClient(string auth_token)
        {
            ManualResetEventSlim clientReady = new ManualResetEventSlim(false);
            SlackClient client = new SlackClient(auth_token);
            client.Connect((connected) => {
                // This is called once the client has emitted the RTM start command
                clientReady.Set();
            }, () => {
                Console.WriteLine("Connected to Slack");
            });
            clientReady.Wait();

            return client;
        }
    }
}
