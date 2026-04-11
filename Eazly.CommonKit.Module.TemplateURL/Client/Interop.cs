using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Eazly.CommonKit.Module.TemplateURL
{
    public class Interop
    {
        private readonly IJSRuntime _jsRuntime;

        public Interop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }
    }
}
