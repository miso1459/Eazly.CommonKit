using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Oqtane.Shared;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Eazly.CommonKit.Module.TemplateURL.Services;
using Oqtane.Controllers;
using System.Net;
using System.Threading.Tasks;

namespace Eazly.CommonKit.Module.TemplateURL.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class TemplateURLController : ModuleControllerBase
    {
        private readonly ITemplateURLService _TemplateURLService;

        public TemplateURLController(ITemplateURLService TemplateURLService, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _TemplateURLService = TemplateURLService;
        }

        // GET: api/<controller>?moduleid=x
        [HttpGet]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<IEnumerable<Models.TemplateURL>> Get(string moduleid)
        {
            int ModuleId;
            if (int.TryParse(moduleid, out ModuleId) && IsAuthorizedEntityId(EntityNames.Module, ModuleId))
            {
                return await _TemplateURLService.GetTemplateURLsAsync(ModuleId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized TemplateURL Get Attempt {ModuleId}", moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }
        }

        // GET api/<controller>/5
        [HttpGet("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<Models.TemplateURL> Get(int id, int moduleid)
        {
            Models.TemplateURL TemplateURL = await _TemplateURLService.GetTemplateURLAsync(id, moduleid);
            if (TemplateURL != null && IsAuthorizedEntityId(EntityNames.Module, TemplateURL.ModuleId))
            {
                return TemplateURL;
            }
            else
            { 
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized TemplateURL Get Attempt {TemplateURLId} {ModuleId}", id, moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<Models.TemplateURL> Post([FromBody] Models.TemplateURL TemplateURL)
        {
            if (ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, TemplateURL.ModuleId))
            {
                TemplateURL = await _TemplateURLService.AddTemplateURLAsync(TemplateURL);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized TemplateURL Post Attempt {TemplateURL}", TemplateURL);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                TemplateURL = null;
            }
            return TemplateURL;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<Models.TemplateURL> Put(int id, [FromBody] Models.TemplateURL TemplateURL)
        {
            if (ModelState.IsValid && TemplateURL.TemplateURLId == id && IsAuthorizedEntityId(EntityNames.Module, TemplateURL.ModuleId))
            {
                TemplateURL = await _TemplateURLService.UpdateTemplateURLAsync(TemplateURL);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized TemplateURL Put Attempt {TemplateURL}", TemplateURL);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                TemplateURL = null;
            }
            return TemplateURL;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task Delete(int id, int moduleid)
        {
            Models.TemplateURL TemplateURL = await _TemplateURLService.GetTemplateURLAsync(id, moduleid);
            if (TemplateURL != null && IsAuthorizedEntityId(EntityNames.Module, TemplateURL.ModuleId))
            {
                await _TemplateURLService.DeleteTemplateURLAsync(id, TemplateURL.ModuleId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized TemplateURL Delete Attempt {TemplateURLId} {ModuleId}", id, moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
        }
    }
}
