using System;
using API.Contracts;
using NuGet.Frameworks;
using PDF;
using Xunit;

namespace UnitTests;

public class OrderPdfDocumentTests
{
    [Fact]
    public void OrderPdfDocument_Should_Create_Document_With_Proper_Parameters()
    {
        var orderGetResult = new OrderGetResult
        {
            ObjectNumber = "Value",
            Address = "Value",
            CustomerName = "Value",
            CustomerPhoneNumber = "ValuE",
            StartDate = DateTime.UtcNow,
        };
        
        var pdfOrder = new OrderPdfDocument(orderGetResult);
        Assert.Equal(orderGetResult.ObjectNumber, pdfOrder.ObjectNumber);
        Assert.Equal(orderGetResult.Address, pdfOrder.Address);
        Assert.Equal(orderGetResult.CustomerName, pdfOrder.Customer);
        Assert.Equal(orderGetResult.CustomerPhoneNumber, pdfOrder.PhoneNumber);
        Assert.Equal(orderGetResult.StartDate.Value.ToShortDateString(), pdfOrder.Date);
    }
    
    [Fact]
    public void OrderPdfDocument_Should_Throw_Exception_For_Null_Order()
    {
        Assert.Throws<ArgumentNullException>(() => new OrderPdfDocument((OrderGetResult)null));
    }
    
    [Fact]
    public void OrderPdfDocument_Should_Throw_Exception_For_Null_ObjectNumber()
    {
        var orderGetResult = new OrderGetResult
        {
            ObjectNumber = null,
            Address = "Value",
            CustomerName = "Value",
            CustomerPhoneNumber = "ValuE",
            StartDate = DateTime.UtcNow,
        };
        
        Assert.Throws<ArgumentNullException>(() => new OrderPdfDocument(orderGetResult));
    }
    
    [Fact]
    public void OrderPdfDocument_Should_Throw_Exception_For_Null_Address()
    {
        var orderGetResult = new OrderGetResult
        {
            ObjectNumber = "Value",
            Address = null,
            CustomerName = "Value",
            CustomerPhoneNumber = "ValuE",
            StartDate = DateTime.UtcNow,
        };
        Assert.Throws<ArgumentNullException>(() => new OrderPdfDocument(orderGetResult));
    }
    
    [Fact]
    public void OrderPdfDocument_Should_Throw_Exception_For_Null_CustomerName()
    {
        var orderGetResult = new OrderGetResult
        {
            ObjectNumber = "Value",
            Address = "value",
            CustomerName = null,
            CustomerPhoneNumber = "ValuE",
            StartDate = DateTime.UtcNow,
        };
        Assert.Throws<ArgumentNullException>(() => new OrderPdfDocument(orderGetResult));
    }
    
    [Fact]
    public void OrderPdfDocument_Should_Throw_Exception_For_Null_CustomerPhoneNumber()
    {
        var orderGetResult = new OrderGetResult
        {
            ObjectNumber = "Value",
            Address = "value",
            CustomerName = "null",
            CustomerPhoneNumber = null,
            StartDate = DateTime.UtcNow,
        };
        Assert.Throws<ArgumentNullException>(() => new OrderPdfDocument(orderGetResult));
    }
    
    [Fact]
    public void OrderPdfDocument_Should_Throw_Exception_For_Null_StartDate()
    {
        var orderGetResult = new OrderGetResult
        {
            ObjectNumber = "Value",
            Address = "value",
            CustomerName = "null",
            CustomerPhoneNumber = "null",
            StartDate = null,
        };
        Assert.Throws<ArgumentException>(() => new OrderPdfDocument(orderGetResult));
    }
    
    [Fact]
    public void OrderPdfDocument_Should_Throw_Exception_For_Default_StartDate()
    {
        var orderGetResult = new OrderGetResult
        {
            ObjectNumber = "Value",
            Address = "value",
            CustomerName = "null",
            CustomerPhoneNumber = "null",
            StartDate = default,
        };
        
        Assert.Throws<ArgumentException>(() => new OrderPdfDocument(orderGetResult));
    }
}