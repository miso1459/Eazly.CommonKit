using Microsoft.AspNetCore.Builder; 
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using Radzen;

using Eazly.CommonKit.Module.Template00.Repository;
using Eazly.CommonKit.Module.Template00.Services;

namespace Eazly.CommonKit.Module.Template00.Startup
{
    public class ServerStartup : IServerStartup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // not implemented
        }

        public void ConfigureMvc(IMvcBuilder mvcBuilder)
        {
            // not implemented
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ITemplate00Service, ServerTemplate00Service>();
            services.AddDbContextFactory<Template00Context>(opt => { }, ServiceLifetime.Transient);

			services.AddScoped<DialogService>();
			services.AddScoped<NotificationService>();
			services.AddScoped<TooltipService>();
			services.AddScoped<ContextMenuService>();
		}
    }
}
