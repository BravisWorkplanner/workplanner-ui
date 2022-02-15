using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using API;
using API.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using Web.Pages.Orders.Components;

namespace Web.Pages.Orders
{
    public class OrderListBase : ComponentBase
    {
        protected string searchString;

        [Inject]
        ISnackbar Snackbar { get; set; }

        [Inject]
        IAPIClient APIClient { get; set; }

        [Inject]
        IDialogService DialogService { get; set; }

        [Inject]
        public ILogger<OrderList> Logger { get; set; }

        protected ICollection<OrderListResult> OrderList { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            await FetchListDataFromServerAsync();
        }

        protected async Task DeleteOrderAsync(int orderId)
        {
            var dialogResult = await DialogService.ShowMessageBox(
                "Warning",
                $"Are you sure you want to delete order {orderId}?",
                yesText: "Yes",
                cancelText: "Cancel");

            if (!dialogResult.GetValueOrDefault())
            {
                return;
            }

            var result = await APIClient.HandleHttpCallAsync(() => APIClient.Order_DeleteAsync(orderId), Logger);
            if (!result.Success)
            {
                Snackbar.Add($"Error when deleting order {orderId}", Severity.Error);
            }
            else
            {
                Snackbar.Add($"Order {result.Data} was successfully deleted", Severity.Success);
                await FetchListDataFromServerAsync();
            }
        }

        protected async Task OpenCreateOrderDialogAsync()
        {
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                FullWidth = true,
                MaxWidth = MaxWidth.ExtraSmall,
            };

            var dialogResult = DialogService.Show<CreateOrderDialog>("Create order", options);
            var result = await dialogResult.Result;
            if (result.Cancelled)
            {
                return;
            }

            var order = await dialogResult.GetReturnValueAsync<OrderCreateRequest>();
            var orderCreateResult = await APIClient.HandleHttpCallAsync(() => APIClient.Order_CreateAsync(order), Logger);
            if (!orderCreateResult.Success)
            {
                Snackbar.Add("Error occurred when creating order", Severity.Error);
            }
            else
            {
                Snackbar.Add($"Order {orderCreateResult.Data} was successfully created", Severity.Success);
                await FetchListDataFromServerAsync();
            }
        }

        protected bool FilterFunc(OrderListResult order)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return true;
            }

            if (order.ObjectNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (order.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private async Task FetchListDataFromServerAsync()
        {
            var orders = await APIClient.HandleHttpCallAsync(
                () => APIClient.Order_ListAsync(20, 1, CancellationToken.None),
                Logger);
            if (!orders.Success)
            {
                Snackbar.Add("Error loading orders. Please try again later.", Severity.Error);
            }
            else
            {
                OrderList = orders.Data;
            }
        }
    }
}