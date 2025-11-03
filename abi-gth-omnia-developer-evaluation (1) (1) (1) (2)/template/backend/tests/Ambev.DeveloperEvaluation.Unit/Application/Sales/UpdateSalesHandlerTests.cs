using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain;
using AutoMapper;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="UpdateSalesHandlerTests"/> class.
/// </summary>
public class UpdateSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly UpdateSaleHandler _handler;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly UpdateSaleCommandValidador _validator;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSalesHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public UpdateSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _configuration = Substitute.For<IConfiguration>();
        _logger = Substitute.For<ILogger<UpdateSaleHandler>>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper, _configuration, _logger);
        _validator = new UpdateSaleCommandValidador();
    }
    
    /// <summary>
    /// Tests that a valid sale creation request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When updating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var expectedMessage = "Event published to Kafka successfully";
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var saleFaker = CreateSaleHandlerTestData.GenerateValid();
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            CreatedAt = saleFaker.Date,
            Branch = saleFaker.Branch,
            SaleNumber = saleFaker.SaleNumber,
            Items = saleFaker.Items,
        };
        foreach (var item in sale.Items)
            item.Quantity = 2;
        
        foreach (var item in command.Items)
            item.Quantity = 2;
        
        var result = new CreateSaleResult
        {
            Id = sale.Id,
        };
        
        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(result);

        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        
        // When
        var updateSaleResult = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        _logger.Received(1);
        
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }
    
    /// <summary>
    /// Items exceeding limit When creating sale Then throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Given Itens exceeding limit When creating sale Then throws InvalidOperationException")]
    public async Task Handle_ItemsExceedsLimit_ThrowsInvalidOperationException()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var saleFaker = CreateSaleHandlerTestData.GenerateValid();

        var description = command.Items.Select(i => i.Description).FirstOrDefault();
        var updatedItems = command.Items
            .Select(p => 
            {
                p.Quantity = 50;
                p.Description = description;
                return p;
            }).ToList();
        
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            CreatedAt = saleFaker.Date,
            Branch = saleFaker.Branch,
            SaleNumber = saleFaker.SaleNumber,
            Items = saleFaker.Items,
        };

        foreach (var item in sale.Items)
            item.Id = command.Id;
        
        var updatedSaleItems = saleFaker.Items
            .Select(p => 
            {
                p.Quantity = 50;
                p.Description = description;
                return p;
            }).ToList();
        
        _mapper.Map<Sale>(command).Returns(sale);
        
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _handler.Handle(command, CancellationToken.None);
        });
    }
    
    /// <summary>
    /// When items are null Then throws ArgumentNullException.
    /// </summary>
    [Fact(DisplayName = "Given Items that null When creating sale Then throws ArgumentNullException")]
    public async Task Should_HaveError_When_ItemsIsNull()
    {
        var command = new UpdateSaleCommand() { Items = null, };
 
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => Task.FromResult(_validator.TestValidate(command)));
        Assert.Contains("Value cannot be null.", exception.Message);
    }
    
    /// <summary>
    /// When SaleNumber is null or zero Then throws ValidationException.
    /// </summary>
    [Fact(DisplayName = "When SaleNumber is null or zero Then throws ValidationException")]
    public async Task Should_HaveError_When_SaleNumberIsNullOrZero()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        command.SaleNumber = 0;
        
        var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, CancellationToken.None));
        Assert.Contains("Sale Number must be greater than 0.", exception.Message);
    }

    /// <summary>
    /// When Date is null Then throws ValidationException.
    /// </summary>
    [Fact(DisplayName = "When Date is null Then throws ValidationException")]
    public async Task Should_HaveError_When_DateIsNull()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        command.Date = null;
        
        var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, CancellationToken.None));
        Assert.Contains("The Date of Sale cannot be null.", exception.Message);
    }

    /// <summary>
    /// When Branch is null Then throws ValidationException.
    /// </summary>
    [Fact(DisplayName = "When Branch is null Then throws ValidationException")]
    public async Task Should_HaveError_When_BranchIsNull()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        command.Branch = null;
        
        var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, CancellationToken.None));
        Assert.Contains("The Branch cannot be null.", exception.Message);
    }

    /// <summary>
    /// When Item Description length is invalid Then throws ValidationException.
    /// </summary>
    [Fact(DisplayName = "When Item Description invalid length Then throws ValidationException")]
    public async Task  Should_HaveError_When_ItemDescriptionInvalidLength()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        foreach (var item in command.Items)
            item.Description = "short";
        
        var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, CancellationToken.None));
        Assert.Contains("Description must be between 10 and 200 characters.", exception.Message);
    }
    
    /// <summary>
    /// When Item Description is null or empty Then throws ValidationException.
    /// </summary>
    [Fact(DisplayName = "When Item Description is null or empty Then throws ValidationException")]
    public async Task  Should_HaveError_When_ItemDescriptionIsNull()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        foreach (var item in command.Items)
            item.Description = "";
        
        var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, CancellationToken.None));
        Assert.Contains("Description is required.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public async Task Should_HaveError_When_ItemQuantity(int quantity)
    {
        var error = "";
        if (quantity != 0)
            error = "Quantity is required.";
        else
            error = "Quantity must be greater than 0.";
        
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        foreach (var item in command.Items)
            item.Quantity = quantity;
        
        var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, CancellationToken.None));
        Assert.Contains(error, exception.Message);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public async Task  Should_HaveError_When_IUnitValueInvalid(int unit)
    {
        var error = "";
        if (unit != 0)
            error = "Unit Value is required.";
        else
            error = "Unit Value must be greater than 0.";
        
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        foreach (var item in command.Items)
            item.UnitValue = unit;
        
        var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, CancellationToken.None));
        Assert.Contains(error, exception.Message);
    }
}