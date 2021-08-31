using System;
using System.Text.Json.Serialization;
using APIClient.Model;

namespace Web.Pages.Workers.ViewModels
{
    public class WorkerListVm
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("endAt")]
        public DateTime EndAt { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("objectNumber")]
        public string ObjectNumber { get; set; }

        [JsonPropertyName("startAt")]
        public DateTime StartAt { get; set; }

        [JsonPropertyName("workOrderStatus")]
        public OrderStatus WorkOrderStatus { get; set; }
    }
}