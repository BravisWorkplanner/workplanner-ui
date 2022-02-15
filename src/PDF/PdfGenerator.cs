using System.Reflection;
using EnsureThat;
using iText.Forms;
using iText.Kernel.Pdf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
    }

    public string GenerateOrderPdfDocument(OrderPdfDocument order)
    {
        var fileConfiguration = ValidateFileConfiguration();

        try
        {
            var templateFilePath = FindTemplateFilePath(fileConfiguration[TemplateFilename]);
            using var pdfReader = new PdfReader(templateFilePath);
            using var pdfWriter = new PdfWriter(fileConfiguration[OutputPath]);
            using var pdfDocument = new PdfDocument(pdfReader, pdfWriter);

            var form = PdfAcroForm.GetAcroForm(pdfDocument, true);

            var properties = typeof(OrderPdfDocument).GetProperties().
                                                      Where(x => x.GetCustomAttribute<OrderPdfFieldNameAttribute>() != null);

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

            return templateFilePath;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }

        return string.Empty;
    }

    private IConfiguration ValidateFileConfiguration()
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
        
        return rootSection;
    }

    private string FindTemplateFilePath(string orderTemplatePdfName)
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var directory = new DirectoryInfo(baseDirectory);
        var fileList = directory.GetFiles(orderTemplatePdfName, SearchOption.AllDirectories);

        if (fileList.Length != 1)
        {
            _logger.LogWarning(
                $"More than 1 ({fileList.Length}) file with template file name {orderTemplatePdfName} was found.");

            return string.Empty;
        }

        return fileList[0].FullName;
    }
}