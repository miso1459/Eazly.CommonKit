using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Security;
using Oqtane.Shared;
using Eazly.CommonKit.Module.TemplateURL.Repository;

namespace Eazly.CommonKit.Module.TemplateURL.Services
{
    public class ServerTemplateURLService : ITemplateURLService
    {
        private readonly ITemplateURLRepository _TemplateURLRepository;
        private readonly IUserPermissions _userPermissions;
        private readonly ILogManager _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly Alias _alias;

        public ServerTemplateURLService(ITemplateURLRepository TemplateURLRepository, IUserPermissions userPermissions, ITenantManager tenantManager, ILogManager logger, IHttpContextAccessor accessor)
        {
            _TemplateURLRepository = TemplateURLRepository;
            _userPermissions = userPermissions;
            _logger = logger;
            _accessor = accessor;
            _alias = tenantManager.GetAlias();
        }

        public Task<List<Models.TemplateURL>> GetTemplateURLsAsync(int ModuleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.View))
            {
                return Task.FromResult(_TemplateURLRepository.GetTemplateURLs(ModuleId).ToList());
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized TemplateURL Get Attempt {ModuleId}", ModuleId);
                return null;
            }
        }

        public Task<Models.TemplateURL> GetTemplateURLAsync(int TemplateURLId, int ModuleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.View))
            {
                return Task.FromResult(_TemplateURLRepository.GetTemplateURL(TemplateURLId));
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized TemplateURL Get Attempt {TemplateURLId} {ModuleId}", TemplateURLId, ModuleId);
                return null;
            }
        }

        public Task<Models.TemplateURL> AddTemplateURLAsync(Models.TemplateURL TemplateURL)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, TemplateURL.ModuleId, PermissionNames.Edit))
            {
                TemplateURL = _TemplateURLRepository.AddTemplateURL(TemplateURL);
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "TemplateURL Added {TemplateURL}", TemplateURL);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized TemplateURL Add Attempt {TemplateURL}", TemplateURL);
                TemplateURL = null;
            }
            return Task.FromResult(TemplateURL);
        }

        public Task<Models.TemplateURL> UpdateTemplateURLAsync(Models.TemplateURL TemplateURL)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, TemplateURL.ModuleId, PermissionNames.Edit))
            {
                TemplateURL = _TemplateURLRepository.UpdateTemplateURL(TemplateURL);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "TemplateURL Updated {TemplateURL}", TemplateURL);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized TemplateURL Update Attempt {TemplateURL}", TemplateURL);
                TemplateURL = null;
            }
            return Task.FromResult(TemplateURL);
        }

        public Task DeleteTemplateURLAsync(int TemplateURLId, int ModuleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.Edit))
            {
                _TemplateURLRepository.DeleteTemplateURL(TemplateURLId);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "TemplateURL Deleted {TemplateURLId}", TemplateURLId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized TemplateURL Delete Attempt {TemplateURLId} {ModuleId}", TemplateURLId, ModuleId);
            }
            return Task.CompletedTask;
        }
    }
}
