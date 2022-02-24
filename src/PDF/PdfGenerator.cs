using System.Reflection;
using EnsureThat;
using iText.Forms;
using iText.Kernel.Pdf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared;

namespace PDF;

public class PdfGenerator : IPdfGenerator
{
    private readonly ILogger<PdfGenerator> _logger;
    private readonly IConfiguration _configuration;

    private const string FileSettingsSection = "FileSettings";
    private const string TemplateFilename = "TemplateFileName";
    private const string OutputPath = "OutputPath";

    public PdfGenerator(ILogger<PdfGenerator> logger, IConfiguration config)
    {
        _logger = EnsureArg.IsNotNull(logger);
        _configuration = EnsureArg.IsNotNull(config);
        ValidateFileConfiguration();
    }

    public string GenerateOrderPdfDocument(OrderPdfDocument order)
    {
        var fileConfiguration = _configuration.GetSection(FileSettingsSection);

        try
        {
            var templateFilePath = FindTemplateFilePath(fileConfiguration[TemplateFilename]);
            var outputPath = FindOutputFilePath(fileConfiguration[OutputPath], order.ObjectNumber);
            
            using var pdfReader = new PdfReader(templateFilePath);
            using var pdfWriter = new PdfWriter(outputPath);
            using var pdfDocument = new PdfDocument(pdfReader, pdfWriter);

            var form = PdfAcroForm.GetAcroForm(pdfDocument, true);

            ReplacePdfFormValues(order, form);
            pdfDocument.Close();
            return outputPath;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    private static void ReplacePdfFormValues(OrderPdfDocument order, PdfAcroForm form)
    {
        var properties = typeof(OrderPdfDocument)
                         .GetProperties()
                         .Where(x => x.GetCustomAttribute<OrderPdfFieldNameAttribute>() != null);

        // TODO: Handle missing property values
        // https://github.com/BravisWorkplanner/workplanner-ui/issues/9
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
    }

    private void ValidateFileConfiguration()
    {
        var rootSection = _configuration.GetSection(FileSettingsSection); 
        if (!rootSection.Exists())
        {
            throw new KeyNotFoundException($"Key {FileSettingsSection} missing from configuration");
        }
        
        var templateFileNameKey = rootSection.GetSection(TemplateFilename);
        if (!templateFileNameKey.Exists() || string.IsNullOrEmpty(templateFileNameKey.Value))
        {
            throw new KeyNotFoundException($"Key or value for {TemplateFilename} missing from configuration");
        }
        
        var outputFilePathKey = rootSection.GetSection(OutputPath);
        if (!outputFilePathKey.Exists() || string.IsNullOrEmpty(outputFilePathKey.Value))
        {
            throw new KeyNotFoundException($"Key or value {OutputPath} missing from configuration");
        }
    }

    private string FindTemplateFilePath(string orderTemplatePdfName)
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var directory = new DirectoryInfo(baseDirectory);
        var fileList = directory.GetFiles(orderTemplatePdfName, SearchOption.AllDirectories);

        if (fileList.Length != 1)
        {
            throw new PdfException($"{fileList.Length} # of files were found with template file name {orderTemplatePdfName}");
        }

        return fileList[0].FullName;
    }
    
    private string FindOutputFilePath(string outputFilePath, string orderObjectNumber)
    {
        var directory = new DirectoryInfo(outputFilePath);
        if (!directory.Exists)
        {
            throw new PdfException($"Output file path: {outputFilePath} does not exist, please create folder on your system");
        }
        
        return Path.Combine(directory.FullName, orderObjectNumber + ".pdf");
    }
}