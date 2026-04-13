using Oqtane.Services;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Eazly.CommonKit.Module.Template00.Services
{

    public class ClientTemplate00Service : ServiceBase, ITemplate00Service
    {
        public ClientTemplate00Service(HttpClient http, SiteState siteState) : base(http, siteState) { }

        private string Apiurl => CreateApiUrl("Template00");

        public async Task<DataTable> GetContentsIDAsync(int ModuleId, string queryID, string jsonParam)
        {
            string url = CreateAuthorizationPolicyUrl($"{Apiurl}/contents?moduleid={ModuleId}&queryID={Uri.EscapeDataString(queryID ?? string.Empty)}&jsonParam={Uri.EscapeDataString(jsonParam ?? string.Empty)}", EntityNames.Module, ModuleId);
            return GetJsonAsync<DataTable>(url).GetAwaiter().GetResult();
        }

        public async Task ExecuteQueryIDAsync(int ModuleId, string queryID, string jsonParam)
        {
            string url = CreateAuthorizationPolicyUrl($"{Apiurl}/execute?moduleid={ModuleId}&queryID={Uri.EscapeDataString(queryID ?? string.Empty)}", EntityNames.Module, ModuleId);
            // send jsonParam as request body
            PostJsonAsync<string, object>(url, jsonParam ?? string.Empty).GetAwaiter().GetResult();
        }

        public async Task<Models.Contition> GetConditionAsync(int ModuleId)
        {
            return await GetJsonAsync<Models.Contition>(
                CreateAuthorizationPolicyUrl($"{Apiurl}/condition?moduleid={ModuleId}", EntityNames.Module, ModuleId)
            );
        }

        public async Task<Models.ContentsConifg> GetConfigContentsAsync(int ModuleId, string queryID)
        {
            string url = CreateAuthorizationPolicyUrl($"{Apiurl}/config?moduleid={ModuleId}&queryID={Uri.EscapeDataString(queryID ?? string.Empty)}", EntityNames.Module, ModuleId);
            return await GetJsonAsync<Models.ContentsConifg>(url);
        }
    }
}
