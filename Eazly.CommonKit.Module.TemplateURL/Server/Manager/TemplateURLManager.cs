using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Oqtane.Modules;
using Oqtane.Models;
using Oqtane.Infrastructure;
using Oqtane.Interfaces;
using Oqtane.Enums;
using Oqtane.Repository;
using Eazly.CommonKit.Module.TemplateURL.Repository;
using System.Threading.Tasks;

namespace Eazly.CommonKit.Module.TemplateURL.Manager
{
    public class TemplateURLManager : MigratableModuleBase, IInstallable, IPortable, ISearchable
    {
        private readonly ITemplateURLRepository _TemplateURLRepository;
        private readonly IDBContextDependencies _DBContextDependencies;

        public TemplateURLManager(ITemplateURLRepository TemplateURLRepository, IDBContextDependencies DBContextDependencies)
        {
            _TemplateURLRepository = TemplateURLRepository;
            _DBContextDependencies = DBContextDependencies;
        }

        public bool Install(Tenant tenant, string version)
        {
            return Migrate(new TemplateURLContext(_DBContextDependencies), tenant, MigrationType.Up);
        }

        public bool Uninstall(Tenant tenant)
        {
            return Migrate(new TemplateURLContext(_DBContextDependencies), tenant, MigrationType.Down);
        }

        public string ExportModule(Oqtane.Models.Module module)
        {
            string content = "";
            List<Models.TemplateURL> TemplateURLs = _TemplateURLRepository.GetTemplateURLs(module.ModuleId).ToList();
            if (TemplateURLs != null)
            {
                content = JsonSerializer.Serialize(TemplateURLs);
            }
            return content;
        }

        public void ImportModule(Oqtane.Models.Module module, string content, string version)
        {
            List<Models.TemplateURL> TemplateURLs = null;
            if (!string.IsNullOrEmpty(content))
            {
                TemplateURLs = JsonSerializer.Deserialize<List<Models.TemplateURL>>(content);
            }
            if (TemplateURLs != null)
            {
                foreach(var TemplateURL in TemplateURLs)
                {
                    _TemplateURLRepository.AddTemplateURL(new Models.TemplateURL { ModuleId = module.ModuleId, Name = TemplateURL.Name });
                }
            }
        }

        public Task<List<SearchContent>> GetSearchContentsAsync(PageModule pageModule, DateTime lastIndexedOn)
        {
           var searchContentList = new List<SearchContent>();

           foreach (var TemplateURL in _TemplateURLRepository.GetTemplateURLs(pageModule.ModuleId))
           {
               if (TemplateURL.ModifiedOn >= lastIndexedOn)
               {
                   searchContentList.Add(new SearchContent
                   {
                       EntityName = "Eazly.CommonKitTemplateURL",
                       EntityId = TemplateURL.TemplateURLId.ToString(),
                       Title = TemplateURL.Name,
                       Body = TemplateURL.Name,
                       ContentModifiedBy = TemplateURL.ModifiedBy,
                       ContentModifiedOn = TemplateURL.ModifiedOn
                   });
               }
           }

           return Task.FromResult(searchContentList);
        }
    }
}
