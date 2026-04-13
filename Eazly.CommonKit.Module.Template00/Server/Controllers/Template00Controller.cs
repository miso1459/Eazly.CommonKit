using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Oqtane.Shared;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Eazly.CommonKit.Module.Template00.Services;
using Oqtane.Controllers;
using System.Net;
using System.Threading.Tasks;
using System.Data;

namespace Eazly.CommonKit.Module.Template00.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class Template00Controller : ModuleControllerBase
    {
        private readonly ITemplate00Service _Template00Service;

        public Template00Controller(ITemplate00Service Template00Service, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _Template00Service = Template00Service;
        }

        // GET api/<controller>/contents?moduleid=x&queryID=...&jsonParam=...
        //[HttpGet("contents")]
        //[Authorize(Policy = PolicyNames.ViewModule)]
        //public async Task<DataTable> GetContentsID(string moduleid, string queryID, string jsonParam)
        //{
        //    int ModuleId;
        //    if (int.TryParse(moduleid, out ModuleId) && IsAuthorizedEntityId(EntityNames.Module, ModuleId))
        //    {
        //        return await _Template00Service.GetContentsIDAsync(ModuleId, queryID, jsonParam);
        //    }
        //    else
        //    {
        //        _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized GetContentsID Attempt {ModuleId}", moduleid);
        //        HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
        //        return null;
        //    }
        //}

        // POST api/<controller>/execute?moduleid=x&queryID=...
        //[HttpPost("execute")]
        //[Authorize(Policy = PolicyNames.EditModule)]
        //public async Task ExecuteQueryID(string moduleid, string queryID, [FromBody] string jsonParam)
        //{
        //    int ModuleId;
        //    if (int.TryParse(moduleid, out ModuleId) && IsAuthorizedEntityId(EntityNames.Module, ModuleId))
        //    {
        //        await _Template00Service.ExecuteQueryIDAsync(ModuleId, queryID, jsonParam);
        //    }
        //    else
        //    {
        //        _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ExecuteQueryID Attempt {ModuleId}", moduleid);
        //        HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
        //    }
        //}
    }
}
