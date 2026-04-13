using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Eazly.CommonKit.Module.Template00.Services
{
    public interface ITemplate00Service 
    {
        Task<DataTable> GetContentsIDAsync(int ModuleId, string queryID, string jsonParam);

        Task ExecuteQueryIDAsync(int ModuleId, string queryID, string jsonParam);

        Task<Models.Contition> GetConditionAsync(int ModuleId);

        Task<Models.ContentsConifg> GetConfigContentsAsync(int ModuleId, string queryID);

	}
}
