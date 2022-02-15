using System.Threading;
using System.Threading.Tasks;
using API;
using API.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using PDF;
using Web.Pages.Orders.Components;

namespace Web.Pages.Orders
{
    public class OrderEditBase : ComponentBase
    {
        private OrderUpdateRequest _orderUpdateRequest;

        [Parameter]
        public int Id { get; set; }

        [Inject]
        IAPIClient APIClient { get; set; }

        [Inject]
        ISnackbar Snackbar { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        IDialogService DialogService { get; set; }

        [Inject]
        public ILogger<OrderEdit> Logger { get; set; }

        protected OrderGetResult OrderGetResult { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadOrderDataAsync();
        }

        private async Task LoadOrderDataAsync()
        {
            var order = await APIClient.HandleHttpCallAsync(
                () => APIClient.Order_GetAsync(Id, CancellationToken.None),
                Logger);

            if (order.Data == null)
            {
                Snackbar.Add($"Could not load order '{Id}'. Please try again.", Severity.Error);
            }
            else
            {
                OrderGetResult = order.Data;
            }
        }

        protected async Task OnValidSubmit()
        {
            _orderUpdateRequest = new OrderUpdateRequest
            {
                OrderId = OrderGetResult.OrderId,
                Address = OrderGetResult.Address,
                Description = OrderGetResult.Description,
                StartDate = OrderGetResult.StartDate,
                EndDate = OrderGetResult.EndDate,
                InvoiceDate = OrderGetResult.InvoiceDate,
                OrderStatus = OrderGetResult.OrderStatus,
                CustomerName = OrderGetResult.CustomerName,
                CustomerPhoneNumber = OrderGetResult.CustomerPhoneNumber,
            };

            var result = await APIClient.HandleHttpCallAsync(
                () => APIClient.Order_UpdateAsync(_orderUpdateRequest, CancellationToken.None),
                Logger);
            if (!result.Success)
            {
                Snackbar.Add($"Could not update order {Id}.", Severity.Error);
            }
            else
            {
                Snackbar.Add($"Order '{result}' was successfully updated", Severity.Success);
                NavigationManager.NavigateTo("/orders");
            }
        }

        protected async Task DeleteOrderAsync(int id)
        {
            var dialogResult = await DialogService.ShowMessageBox(
                "Warning",
                $"Are you sure you want to delete order {id}?",
                yesText: "Yes",
                cancelText: "Cancel");

            if (!dialogResult.GetValueOrDefault())
            {
                return;
            }

            var result = await APIClient.HandleHttpCallAsync(() => APIClient.Order_DeleteAsync(id), Logger);
            if (!result.Success)
            {
                Snackbar.Add($"Error when deleting order {id}", Severity.Error);
            }
            else
            {
                Snackbar.Add($"Order {result.Data} was successfully deleted", Severity.Success);
                NavigationManager.NavigateTo("/orders");
            }
        }

        protected async Task OpenCreateExpenseDialogAsync()
        {
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                FullWidth = true,
                MaxWidth = MaxWidth.ExtraSmall,
            };

            var dialogParameters = new DialogParameters { { "OrderId", Id } };

            var dialogResult = DialogService.Show<CreateExpenseDialog>("Create expense", dialogParameters, options);
            var result = await dialogResult.Result;
            if (result.Cancelled)
            {
                return;
            }

            var expense = await dialogResult.GetReturnValueAsync<OrderExpenseCreateRequest>();
            expense.OrderId = Id;
            var expenseCreateResult = await APIClient.HandleHttpCallAsync(
                () => APIClient.Expense_CreateAsync(expense),
                Logger);
            if (!expenseCreateResult.Success)
            {
                Snackbar.Add("Error occurred when creating expense", Severity.Error);
            }
            else
            {
                Snackbar.Add($"Expense {expenseCreateResult.Data} was successfully created", Severity.Success);
                await LoadOrderDataAsync();
            }
        }

        protected async Task OpenCreateTimeRegistrationDialogAsync()
        {
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                FullWidth = true,
                MaxWidth = MaxWidth.ExtraSmall,
            };

            var dialogParameters = new DialogParameters { { "OrderId", Id } };

            var dialogResult = DialogService.Show<CreateTimeRegistrationDialog>(
                "Create time registration",
                dialogParameters,
                options);
            var result = await dialogResult.Result;
            if (result.Cancelled)
            {
                return;
            }

            var timeRegistration = await dialogResult.GetReturnValueAsync<TimeRegistrationCreateRequest>();
            timeRegistration.OrderId = Id;
            var timeRegistrationResult = await APIClient.HandleHttpCallAsync(
                () => APIClient.TimeRegistration_CreateAsync(timeRegistration),
                Logger);
            if (!timeRegistrationResult.Success)
            {
                Snackbar.Add("Error occurred when creating time registration", Severity.Error);
            }
            else
            {
                Snackbar.Add($"Expense {timeRegistrationResult.Data} was successfully created", Severity.Success);
                await LoadOrderDataAsync();
            }
        }
        
        protected void GenerateWorkOrder()
        {
            var order = new OrderPdfDocument(OrderGetResult);
            var result = PdfGenerator.GenerateOrderPdfDocument(order);
            if (string.IsNullOrEmpty(result))
            {
                Snackbar.Add("Error occurred when creating order pdf", Severity.Error);
            }
            else
            {
                Snackbar.Add($"Order pdf was created at '{result}'", Severity.Success);
            }
        }
    }
}