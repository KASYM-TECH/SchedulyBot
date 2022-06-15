using TechExtensions.SchedyBot.DLL;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TechExtensions.SchedyBot.Api.Extentions;
using System;
using TechExtensions.SchedyBot.Api.HostedServices;
using TechExtensions.SchedyBot.Api.Settings;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.TelegramBotBasics;
using Telegram.Bot;

namespace TechExtensions.SchedyBot.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly BotConfiguration _botConfig;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            _botConfig = _configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            services.AddScopedRepositories();
            services.AddMemoryCache();
            
            services.AddV2Services();

            services.AddHostedService<ConfigureWebhook>();
            
            services.AddHttpClient("tgwebhook")
                .AddTypedClient<ITelegramBotClient>(httpClient
                    => new TelegramBotClient(_botConfig.BotToken!, httpClient));

            // Dummy business-logic service
            services.AddScoped<HandleUpdateService>();

            var connectionPostgreSQLForSchedulybot = _configuration["ConnectionStringToSchedulyBot"];
            var connectionPostgreSQLForHangFire = _configuration["ConnectionStringToPostgresHangFire"];
            services.AddDbContext<SchedulyBotContext>(options =>
            {
                options.UseNpgsql(connectionPostgreSQLForSchedulybot);
            });

           services.AddHangfire(x =>
              x.UsePostgreSqlStorage(connectionPostgreSQLForHangFire));
            services.AddHangfireServer();
            services.AddControllers().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseHangfireDashboard();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                var token = _botConfig.BotToken;
                endpoints.MapControllerRoute(name: "tgwebhook",
                    pattern: $"bot/{token}",
                    new { controller = "Telegram", action = "Post" });
            });
        }
    }
}
