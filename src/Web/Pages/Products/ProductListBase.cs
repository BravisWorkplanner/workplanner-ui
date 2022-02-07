using System.Collections.Generic;
using System.Threading.Tasks;
using API;
using API.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace Web.Pages.Products;

public class ProductListBase : ComponentBase
{
    protected string searchString;

    [Inject]
    private IAPIClient APIClient { get; set; }

    [Inject]
    private ILogger<ProductList> Logger { get; set; }

    [Inject]
    ISnackbar Snackbar { get; set; }

    [Inject]
    IDialogService DialogService { get; set; }

    protected ICollection<ProductListResult> ProductList { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        await FetchDataFromServerAsync();
    }

    protected async Task FetchDataFromServerAsync()
    {
        var products = await APIClient.HandleHttpCallAsync(() => APIClient.Product_ListAsync(500, 1), Logger);
        if (!products.Success)
        {
            Snackbar.Add("Error loading workers. Please try again later.", Severity.Error);

            return;
        }

        ProductList = products.Data;
    }

    protected async Task DeleteProductAsync(int productId)
    {
        var dialogResult = await DialogService.ShowMessageBox(
            "Warning",
            $"Are you sure you want to delete product {productId}?",
            yesText: "Yes",
            cancelText: "Cancel");

        if (!dialogResult.GetValueOrDefault())
        {
            return;
        }

        var result = await APIClient.HandleHttpCallAsync(() => APIClient.Product_DeleteAsync(productId), Logger);
        if (!result.Success)
        {
            Snackbar.Add($"Error deleting product {productId}", Severity.Error);

            return;
        }

        Snackbar.Add($"Product {result.Data} was successfully deleted", Severity.Success);
        await FetchDataFromServerAsync();
    }
}