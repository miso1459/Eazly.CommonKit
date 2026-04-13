using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Eazly.CommonKit.Module.Template00
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
