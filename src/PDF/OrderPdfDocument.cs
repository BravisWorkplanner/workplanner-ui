using System.Reflection;
using API.Contracts;
using EnsureThat;

namespace PDF;

public class OrderPdfDocument
{
    [OrderPdfFieldName("form1[0].#subform[0].Header[0].InvoiceNumber[0]")]
    public string ObjectNumber { get; private set; } = "B-999";

    [OrderPdfFieldName("form1[0].#subform[0].Header[0].InvoiceDate[0]")]
    public string Date { get; private set; } = DateTime.UtcNow.ToShortDateString();

    [OrderPdfFieldName("form1[0].#subform[0].Header[0].Company[0]")]
    public string Customer { get; private set; } = "Customer 9999";

    [OrderPdfFieldName("form1[0].#subform[0].Header[0].Address[0]")]
    public string Address { get; private set; } = "Address 999";

    [OrderPdfFieldName("form1[0].#subform[0].Header[0].StateProvince[0]")]
    public string PhoneNumber { get; private set; } = "0700-999999";

    [OrderPdfFieldName("form1[0].#subform[0].Header[0].ContactName[0]")]
    public string Ref { get; private set; } = "REF123";

    public OrderPdfDocument(OrderGetResult order)
    {
        EnsureArg.IsNotNullOrWhiteSpace(order.ObjectNumber, nameof(order.ObjectNumber));
        EnsureArg.IsNotNullOrWhiteSpace(order.Address, nameof(order.Address));
        EnsureArg.IsNotNullOrWhiteSpace(order.CustomerName, nameof(order.CustomerName));
        EnsureArg.IsNotNull(order.StartDate, nameof(order.StartDate));
        EnsureArg.IsNotNullOrWhiteSpace(order.CustomerPhoneNumber, nameof(order.CustomerPhoneNumber));

        ObjectNumber = order.ObjectNumber;
        Address = order.Address;
        Customer = order.CustomerName;
        Date = order.StartDate.Value.ToShortDateString();
        PhoneNumber = order.CustomerPhoneNumber;
        Ref = "???";
    }
}