using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eazly.CommonKit.Module.TemplateURL.Services
{
    public interface ITemplateURLService 
    {
        Task<List<Models.TemplateURL>> GetTemplateURLsAsync(int ModuleId);

        Task<Models.TemplateURL> GetTemplateURLAsync(int TemplateURLId, int ModuleId);

        Task<Models.TemplateURL> AddTemplateURLAsync(Models.TemplateURL TemplateURL);

        Task<Models.TemplateURL> UpdateTemplateURLAsync(Models.TemplateURL TemplateURL);

        Task DeleteTemplateURLAsync(int TemplateURLId, int ModuleId);
    }
}
