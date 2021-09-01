using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API;
using API.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using Web.Pages.Workers.Components;

namespace Web.Pages.Workers
{
    public class WorkerListBase : ComponentBase
    {
        protected string searchString;

        [Inject]
        IAPIClient APIClient { get; set; }

        [Inject]
        ISnackbar Snackbar { get; set; }

        [Inject]
        private IDialogService DialogService { get; set; }

        [Inject]
        ILogger<WorkerList> Logger { get; set; }

        protected ICollection<WorkerListResult> WorkerList { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            await FetchDataFromServerAsync();
        }

        protected async Task DeleteWorkerAsync(int workerId)
        {
            var dialogResult = await DialogService.ShowMessageBox(
                "Warning",
                $"Are you sure you want to delete worker {workerId}?",
                yesText: "Yes",
                cancelText: "Cancel");

            if (!dialogResult.GetValueOrDefault())
            {
                return;
            }

            var result = await APIClient.HandleHttpCallAsync(
                () => APIClient.Worker_DeleteAsync(workerId),
                Logger);
            if (!result.Success)
            {
                Snackbar.Add($"Error deleting worker {workerId}", Severity.Error);

                return;
            }

            Snackbar.Add($"Worker {result.Data} was successfully deleted", Severity.Success);
            await FetchDataFromServerAsync();
        }

        protected async Task OpenCreateWorkerDialogAsync()
        {
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                FullWidth = true,
                MaxWidth = MaxWidth.ExtraSmall,
            };

            var dialogResult = DialogService.Show<CreateWorkerDialog>("Create worker", options);
            var result = await dialogResult.Result;
            if (result.Cancelled)
            {
                return;
            }

            var worker = await dialogResult.GetReturnValueAsync<WorkerCreateRequest>();
            var workerCreateResult = await APIClient.HandleHttpCallAsync(
                () => APIClient.Worker_CreateAsync(worker),
                Logger);
            if (!workerCreateResult.Success)
            {
                Snackbar.Add("Error occurred when creating worker", Severity.Error);
            }
            else
            {
                Snackbar.Add($"Worker {workerCreateResult.Data} was successfully created", Severity.Success);
                await FetchDataFromServerAsync();
            }
        }

        protected bool FilterFunc(WorkerListResult worker)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return true;
            }

            if (worker.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (worker.Company.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (worker.PhoneNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        protected async Task FetchDataFromServerAsync()
        {
            var workers = await APIClient.HandleHttpCallAsync(
                () => APIClient.Worker_ListAsync(20, 1),
                Logger);
            if (!workers.Success)
            {
                Snackbar.Add("Error loading workers. Please try again later.", Severity.Error);
                return;
            }

            WorkerList = workers.Data;
        }
    }
}