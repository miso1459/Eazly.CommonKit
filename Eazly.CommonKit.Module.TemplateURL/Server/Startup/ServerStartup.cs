using Microsoft.AspNetCore.Builder; 
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using Eazly.CommonKit.Module.TemplateURL.Repository;
using Eazly.CommonKit.Module.TemplateURL.Services;

namespace Eazly.CommonKit.Module.TemplateURL.Startup
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
            services.AddTransient<ITemplateURLService, ServerTemplateURLService>();
            services.AddDbContextFactory<TemplateURLContext>(opt => { }, ServiceLifetime.Transient);
        }
    }
}
