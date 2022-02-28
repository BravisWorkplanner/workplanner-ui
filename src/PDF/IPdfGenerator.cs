using API.Contracts;

namespace PDF;

public interface IPdfGenerator
{
    string SaveOrderPdfDocument(OrderGetResult order);
}