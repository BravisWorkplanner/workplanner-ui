using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using APIClient.Model;
using Microsoft.AspNetCore.Components;

namespace Web.Pages.Orders
{
    public class OrderEditBase : ComponentBase
    {
        private OrderUpdateRequest _orderUpdateRequest;

        [Parameter]
        public int Id { get; set; }

        [Inject]
        public IHttpClientFactory HttpClientFactory { get; set; }

        protected OrderGetResult OrderGetResult { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var client = HttpClientFactory.CreateClient();
            OrderGetResult = await client.GetFromJsonAsync<OrderGetResult>("https://localhost:5001/api/v1/orders/" + Id, CancellationToken.None);
        }

        protected async Task OnValidSubmit()
        {
            // map to order update request and send api call
        }

        protected async Task OnInvalidSubmit()
        {
            //  display some error
        }
    }
}
