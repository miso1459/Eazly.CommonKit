using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Oqtane.Services;
using Eazly.CommonKit.Module.Template00.Services;

namespace Eazly.CommonKit.Module.Template00.Startup
{
    public class ClientStartup : IClientStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            if (!services.Any(s => s.ServiceType == typeof(ITemplate00Service)))
            {
                services.AddScoped<ITemplate00Service, ClientTemplate00Service>();
            }
        }
    }
}
