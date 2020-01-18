using DDApp.AppStructure.Providers;
using DDApp.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DDApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<IQueryExpressionProvider, QueryExpressionProvider>();

            #region App Storage
            string redisAppStoreConnectionString = Configuration.GetConnectionString("RedisAppStore");
            if (!string.IsNullOrWhiteSpace(redisAppStoreConnectionString))
            {
                services.AddSingleton<IAppStorage, RedisAppStorage>();
                services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisAppStoreConnectionString));
                services.AddSingleton<IRedisCache, RedisCache>();
            }
            else
            {
                services.AddSingleton<IAppStorage, FileAppStorage>();
            }
            #endregion App Storage

            services.AddSingleton<IAppProvider, AppProvider>();
            services.AddSingleton<IServerState, ServerState>();

            services.AddMvc(options => options.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                var appProvider = app.ApplicationServices
                    .GetRequiredService<IAppProvider>();
                appProvider.PreCacheAppsAsync();
            }
            app.UseStaticFiles();
            
            app.UseMvc();
        }
    }
}
