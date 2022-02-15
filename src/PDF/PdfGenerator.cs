using System.Reflection;
using System.Runtime.CompilerServices;
using API.Contracts;
using iText.Forms;
using iText.Kernel.Pdf;

namespace PDF;

public class PdfGenerator
{
    public static string GenerateOrderPdfDocument(OrderPdfDocument order)
    {
        var filePath = GetPdfTemplateFilePath();
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
                var fieldName = property.GetCustomAttribute<OrderPdfFieldNameAttribute>().FieldName;
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
            Console.WriteLine(e);
        }

        return string.Empty;
    }

    private static string GetPdfTemplateFilePath()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var directory = new DirectoryInfo(baseDirectory);
        var fileList = directory.GetFiles("ARBETSORDER.pdf", System.IO.SearchOption.AllDirectories);
        if (fileList.Length != 1)
        {
            // more than 1 file found, should return error
            return string.Empty;
        }

        return fileList[0].FullName;
    }
}