using Oqtane.Models;
using Oqtane.Modules;

namespace Eazly.CommonKit.Module.Template00
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Template00",
            Description = "Template00",
            Version = "1.0.0",
            ServerManagerType = "Eazly.CommonKit.Module.Template00.Manager.Template00Manager, Eazly.CommonKit.Module.Template00.Server.Oqtane",
            ReleaseVersions = "1.0.0",
            Dependencies = "Eazly.CommonKit.Module.Template00.Shared.Oqtane,Radzen.Blazor",
			PackageName = "Eazly.CommonKit.Module.Template00" 
        };
    }
}
