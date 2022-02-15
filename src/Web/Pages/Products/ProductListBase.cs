using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API;
using API.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using Web.Pages.Products.Components;

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

    protected async Task OpenCreateProductDialogAsync()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            FullWidth = true,
            MaxWidth = MaxWidth.ExtraSmall,
        };

        var dialogResult = DialogService.Show<CreateProductDialog>("Create product", options);
        var result = await dialogResult.Result;
        if (result.Cancelled)
        {
            return;
        }

        var product = await dialogResult.GetReturnValueAsync<ProductCreateRequest>();
        var productCreateResult = await APIClient.HandleHttpCallAsync(() => APIClient.Product_CreateAsync(product), Logger);
        if (!productCreateResult.Success)
        {
            Snackbar.Add("Error occurred when creating product", Severity.Error);
        }
        else
        {
            Snackbar.Add($"Product {productCreateResult.Data} was successfully created", Severity.Success);
            await FetchDataFromServerAsync();
        }
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
    
    protected bool FilterFunc(ProductListResult product)
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            return true;
        }

        if (product.Type.Contains(searchString, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (product.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }
}