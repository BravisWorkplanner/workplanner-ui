namespace PDF;

public interface IPdfGenerator
{
    string GenerateOrderPdfDocument(OrderPdfDocument order);
}