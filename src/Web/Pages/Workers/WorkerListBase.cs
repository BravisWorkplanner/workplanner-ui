using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Web.Pages.Workers
{
    public class WorkerListBase : ComponentBase
    {
        [Inject]
        public IHttpClientFactory HttpClientFactory { get; set; }

        public ICollection<WorkerList> WorkerList { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            var client = HttpClientFactory.CreateClient();
            WorkerList = await client.GetFromJsonAsync<List<WorkerList>>("https://localhost:5001/api/v1/workers", CancellationToken.None);
        }
    }
}
