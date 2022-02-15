using System.Reflection;
using System.Runtime.CompilerServices;
using API.Contracts;
using iText.Forms;
using iText.Kernel.Pdf;
using Microsoft.Extensions.Logging;

namespace PDF;

public class PdfGenerator : IPdfGenerator
{
    private readonly ILogger<PdfGenerator> _logger;

    public PdfGenerator(ILogger<PdfGenerator> logger)
    {
        if (logger == null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        _logger = logger;
    }
    
    public string GenerateOrderPdfDocument(OrderPdfDocument order)
    {
        var filePath = GetPdfTemplateFilePath("ARBETSORDER.pdf");
        if (string.IsNullOrEmpty(filePath))
        {
            return string.Empty;
        }

        try
        {
            using var pdfReader = new PdfReader(filePath);
            var outputName = Path.Combine(
                Path.GetTempPath(),
                "images",
                Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".pdf");
            using var pdfWriter = new PdfWriter(outputName);
            using var pdfDocument = new PdfDocument(pdfReader, pdfWriter);
            
            var form = PdfAcroForm.GetAcroForm(pdfDocument, true);
            
            var properties = typeof(OrderPdfDocument)
                             .GetProperties()
                             .Where(x => x.GetCustomAttribute<OrderPdfFieldNameAttribute>() != null);

            foreach (var property in properties)
            {
                var fieldName = property.GetCustomAttribute<OrderPdfFieldNameAttribute>()?.FieldName;
                if (string.IsNullOrEmpty(fieldName))
                {
                    continue;
                }

                var field = form.GetField(fieldName);
                if (field == null)
                {
                    continue;
                }

                var value = property.GetValue(order);
                if (value != null)
                {
                    field.SetValue((string)value);
                }
            }

            pdfDocument.Close();

            return outputName;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }

        return string.Empty;
    }

    private string GetPdfTemplateFilePath(string orderTemplatePdfName)
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var directory = new DirectoryInfo(baseDirectory);
        var fileList = directory.GetFiles(orderTemplatePdfName, System.IO.SearchOption.AllDirectories);
        
        if (fileList.Length != 1)
        {
            _logger.LogWarning($"More than 1 ({fileList.Length}) file with template file name {orderTemplatePdfName} was found.");
            return string.Empty;
        }

        return fileList[0].FullName;
    }
}