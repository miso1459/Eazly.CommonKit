using Oqtane.Models;
using Oqtane.Modules;

namespace Eazly.CommonKit.Module.TemplateURL
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "TemplateURL",
            Description = "TemplateURL",
            Version = "1.0.0",
            ServerManagerType = "Eazly.CommonKit.Module.TemplateURL.Manager.TemplateURLManager, Eazly.CommonKit.Module.TemplateURL.Server.Oqtane",
            ReleaseVersions = "1.0.0",
            Dependencies = "Eazly.CommonKit.Module.TemplateURL.Shared.Oqtane",
            PackageName = "Eazly.CommonKit.Module.TemplateURL" 
        };
    }
}
