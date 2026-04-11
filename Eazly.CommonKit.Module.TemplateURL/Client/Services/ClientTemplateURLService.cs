using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Services;
using Oqtane.Shared;

namespace Eazly.CommonKit.Module.TemplateURL.Services
{

    public class ClientTemplateURLService : ServiceBase, ITemplateURLService
    {
        public ClientTemplateURLService(HttpClient http, SiteState siteState) : base(http, siteState) { }

        private string Apiurl => CreateApiUrl("TemplateURL");

        public async Task<List<Models.TemplateURL>> GetTemplateURLsAsync(int ModuleId)
        {
            List<Models.TemplateURL> TemplateURLs = await GetJsonAsync<List<Models.TemplateURL>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={ModuleId}", EntityNames.Module, ModuleId), Enumerable.Empty<Models.TemplateURL>().ToList());
            return TemplateURLs.OrderBy(item => item.Name).ToList();
        }

        public async Task<Models.TemplateURL> GetTemplateURLAsync(int TemplateURLId, int ModuleId)
        {
            return await GetJsonAsync<Models.TemplateURL>(CreateAuthorizationPolicyUrl($"{Apiurl}/{TemplateURLId}/{ModuleId}", EntityNames.Module, ModuleId));
        }

        public async Task<Models.TemplateURL> AddTemplateURLAsync(Models.TemplateURL TemplateURL)
        {
            return await PostJsonAsync<Models.TemplateURL>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, TemplateURL.ModuleId), TemplateURL);
        }

        public async Task<Models.TemplateURL> UpdateTemplateURLAsync(Models.TemplateURL TemplateURL)
        {
            return await PutJsonAsync<Models.TemplateURL>(CreateAuthorizationPolicyUrl($"{Apiurl}/{TemplateURL.TemplateURLId}", EntityNames.Module, TemplateURL.ModuleId), TemplateURL);
        }

        public async Task DeleteTemplateURLAsync(int TemplateURLId, int ModuleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{TemplateURLId}/{ModuleId}", EntityNames.Module, ModuleId));
        }
    }
}
