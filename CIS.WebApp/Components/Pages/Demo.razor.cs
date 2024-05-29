using CIS.Application.Features.Shared;
using Microsoft.AspNetCore.Components;

namespace CIS.WebApp.Components.Pages
{
    public partial class Demo : ComponentBase
    {
        [Inject]
        public required DemoService DemoService { get; set; }

        private async Task Reset()
        {
            await DemoService.ResetAllData(CancellationToken.None);
        }
    }
}
