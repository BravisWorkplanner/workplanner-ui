using System.Threading;
using System.Threading.Tasks;
using API;
using API.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace Web.Pages.Workers
{
    public class WorkerEditBase : ComponentBase
    {
        private WorkerUpdateRequest _workerUpdateRequest;

        [Parameter]
        public int Id { get; set; }

        [Inject]
        IAPIClient APIClient { get; set; }

        [Inject]
        ISnackbar Snackbar { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        ILogger<WorkerEdit> Logger { get; set; }

        protected WorkerGetResult WorkerGetResult { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await APIClient.HandleHttpCallAsync(
                () => APIClient.Worker_GetAsync(Id, CancellationToken.None),
                Logger);
            if (!result.Success)
            {
                Snackbar.Add($"Could not load worker '{Id}'. Please try again.", Severity.Error);

                return;
            }

            WorkerGetResult = result.Data;
        }

        protected async Task OnValidSubmit()
        {
            _workerUpdateRequest = new WorkerUpdateRequest
            {
                WorkerId = WorkerGetResult.WorkerId,
                Name = WorkerGetResult.Name,
                Company = WorkerGetResult.Company,
                PhoneNumber = WorkerGetResult.PhoneNumber,
            };

            var result = await APIClient.HandleHttpCallAsync(
                () => APIClient.Worker_UpdateAsync(_workerUpdateRequest, CancellationToken.None),
                Logger);
            if (!result.Success)
            {
                Snackbar.Add($"Could not update worker '{Id}'. Please try again.", Severity.Error);

                return;
            }

            Snackbar.Add($"Worker '{Id}' was successfully updated", Severity.Success);
            NavigationManager.NavigateTo("/workers");
        }
    }
}