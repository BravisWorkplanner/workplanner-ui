using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using APIClient.Api;
using APIClient.Model;
using Microsoft.AspNetCore.Components;

namespace Web.Pages.Orders
{
    public class OrderListBase : ComponentBase
    {
        [Inject]
        public IHttpClientFactory HttpClientFactory { get; set; }

        public ICollection<OrderListResult> OrderList { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            OrderList = await new OrdersApi().OrderListAsync();
        }
    }
}
