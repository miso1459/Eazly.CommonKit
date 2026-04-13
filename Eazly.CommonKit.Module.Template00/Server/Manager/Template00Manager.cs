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
using Eazly.CommonKit.Module.Template00.Repository;
using System.Threading.Tasks;

namespace Eazly.CommonKit.Module.Template00.Manager
{
    public class Template00Manager : MigratableModuleBase, IInstallable, IPortable, ISearchable
    {
        private readonly IDBContextDependencies _DBContextDependencies;

        public Template00Manager(IDBContextDependencies DBContextDependencies)
        {
            _DBContextDependencies = DBContextDependencies;
        }

        public bool Install(Tenant tenant, string version)
        {
            return Migrate(new Template00Context(_DBContextDependencies), tenant, MigrationType.Up);
        }

        public bool Uninstall(Tenant tenant)
        {
            return Migrate(new Template00Context(_DBContextDependencies), tenant, MigrationType.Down);
        }

        public string ExportModule(Oqtane.Models.Module module)
        {
            return "[]";
        }

        public void ImportModule(Oqtane.Models.Module module, string content, string version)
        {
        }

        public Task<List<SearchContent>> GetSearchContentsAsync(PageModule pageModule, DateTime lastIndexedOn)
        {
           var searchContentList = new List<SearchContent>();

           return Task.FromResult(searchContentList);
        }
    }
}
