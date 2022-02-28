using System;
using System.Collections.Generic;
using System.IO;
using API.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PDF;
using Shared;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests;

public class PdfGeneratorTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public PdfGeneratorTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void PdfGenerator_Should_Validate_Configuration_Correct()
    {
        var mockLogger = new Mock<ILogger<PdfGenerator>>();
        var configuration = new ConfigurationBuilder();
        configuration.AddInMemoryCollection(new []
        {
            new KeyValuePair<string, string>("FileSettings:TemplateFileName", "somefile"),
            new KeyValuePair<string, string>("FileSettings:OutputPath", "C:\\Users\\vma11743\\AppData\\Local\\Temp\\images"),
        });
        
        _ = new PdfGenerator(mockLogger.Object, configuration.Build());
    }
    
    [Fact]
    public void PdfGenerator_Validation_Should_Fail_For_Missing_TemplateFileName()
    {
        var mockLogger = new Mock<ILogger<PdfGenerator>>();
        var configuration = new ConfigurationBuilder();
        configuration.AddInMemoryCollection(new []
        {
            new KeyValuePair<string, string>("FileSettings:OutputPath", "C:\\Users\\vma11743\\AppData\\Local\\Temp\\images"),
        });
        
        Assert.Throws<KeyNotFoundException>(() => new PdfGenerator(mockLogger.Object, configuration.Build()));
    }
    
    [Fact]
    public void PdfGenerator_Validation_Should_Fail_For_Missing_OutputPath()
    {
        var mockLogger = new Mock<ILogger<PdfGenerator>>();
        var configuration = new ConfigurationBuilder();
        configuration.AddInMemoryCollection(new []
        {
            new KeyValuePair<string, string>("FileSettings:TemplateFileName", "somefile"),
        });
        
        Assert.Throws<KeyNotFoundException>(() => new PdfGenerator(mockLogger.Object, configuration.Build()));
    }

    [Fact]
    public void PdfGenerator_GenerateOrderPdfDocument_Should_Throw_For_Missing_TemplateFile()
    {
        var mockLogger = new Mock<ILogger<PdfGenerator>>();
        var configuration = new ConfigurationBuilder();
        configuration.AddInMemoryCollection(new []
        {
            new KeyValuePair<string, string>("FileSettings:TemplateFileName", "somefile"),
            new KeyValuePair<string, string>("FileSettings:OutputPath", "incorrect\\path"),
        });
        
        var sut = new PdfGenerator(mockLogger.Object, configuration.Build());
        
        Assert.Throws<PdfException>(() => sut.SaveOrderPdfDocument(
            new OrderGetResult
            {
                ObjectNumber = "Value",
                Address = "Value",
                CustomerName = "Value",
                CustomerPhoneNumber = "Value",
                StartDate = DateTime.UtcNow,
            }));
    }
    
    [Fact]
    public void PdfGenerator_GenerateOrderPdfDocument_Should_Return_Created_FileName()
    {
        var mockLogger = new Mock<ILogger<PdfGenerator>>();
        var configuration = new ConfigurationBuilder();
        configuration.AddInMemoryCollection(new []
        {
            new KeyValuePair<string, string>("FileSettings:TemplateFileName", "ARBETSORDER.pdf"),
            new KeyValuePair<string, string>("FileSettings:OutputPath", AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\')),
        });
        
        var sut = new PdfGenerator(mockLogger.Object, configuration.Build());
        
        var order = new OrderGetResult
        {
            ObjectNumber = Guid.NewGuid().ToString("N"),
            Address = "Value",
            CustomerName = "Value",
            CustomerPhoneNumber = "Value",
            StartDate = DateTime.UtcNow,
        };
        
        var result = sut.SaveOrderPdfDocument(order);
        
        Assert.Equal(order.ObjectNumber + ".pdf", result.Substring(result.LastIndexOf(Path.DirectorySeparatorChar) + 1));
        Assert.True(File.Exists(result));
    }
    
    [Fact]
    public void PdfGenerator_GenerateOrderPdfDocument_Should_Throw_For_Missing_Order_Values()
    {
        var mockLogger = new Mock<ILogger<PdfGenerator>>();
        var configuration = new ConfigurationBuilder();
        configuration.AddInMemoryCollection(new []
        {
            new KeyValuePair<string, string>("FileSettings:TemplateFileName", "somefile"),
            new KeyValuePair<string, string>("FileSettings:OutputPath", "incorrect\\path"),
        });
        
        var sut = new PdfGenerator(mockLogger.Object, configuration.Build());
        
        Assert.Throws<PdfException>(() => sut.SaveOrderPdfDocument(
            new OrderGetResult
            {
                ObjectNumber = "Value",
                Address = "Value",
                CustomerName = "Value",
                StartDate = DateTime.UtcNow,
            }));
    }
}