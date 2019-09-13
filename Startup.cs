namespace Throneteki.LobbyNode
{
    using CrimsonDev.Gameteki.LobbyNode.Helpers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Throneteki.LobbyNode.Services;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLobbyBase(Configuration);
            services.AddTransient<IHttpClient, HttpClientWrapper>();
            services.AddSingleton<IThronetekiLobbyService, ThronetekiLobbyService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseLobby();

            app.UseSignalR(routes =>
            {
                routes.MapHub<ThronetekiHub>(PathString.Empty);
            });

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var lobbyService = app.ApplicationServices.GetService<IThronetekiLobbyService>();
            lobbyService.Init();
        }
    }
}
