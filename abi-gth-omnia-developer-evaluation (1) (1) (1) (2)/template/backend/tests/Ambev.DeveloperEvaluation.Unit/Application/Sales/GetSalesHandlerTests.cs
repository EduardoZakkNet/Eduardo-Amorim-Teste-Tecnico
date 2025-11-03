using System.Data;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="GetSalesHandlerTests"/> class.
/// </summary>
public class GetSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;
    private readonly ILogger<GetSaleHandler> _logger;
    private readonly GetSaleValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSalesHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public GetSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<GetSaleHandler>>();
        _handler = new GetSaleHandler(_saleRepository, _mapper, _logger);
        _validator = new GetSaleValidator();
    }

    /// <summary>
    /// Tests that a valid sale Get request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When Get sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = GetUserHandlerTestData.GenerateValidCommand();
        command = new GetSaleCommand(Guid.NewGuid());

        var saleFaker = CreateSaleHandlerTestData.GenerateValid();

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            CreatedAt = saleFaker.Date,
            Branch = saleFaker.Branch,
            SaleNumber = saleFaker.SaleNumber,
            Items = saleFaker.Items,
        };

        var result = new GetSaleResult(sale.Items)
        {
            Id = sale.Id,
            Branch = saleFaker.Branch,
            SaleNumber = saleFaker.SaleNumber,
        };

        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        _saleRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var getSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getSaleResult.Should().NotBeNull();
        getSaleResult.Id.Should().Be(sale.Id);

        // Assert
        _logger.Received(1);

        await _saleRepository.Received(1).GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should throw ValidationException when request is invalid")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var invalidRequest = new GetSaleCommand(Guid.Empty);

        var handler = new GetSaleHandler(_saleRepository, _mapper, _logger);

        // Act & Assert
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () =>
            await handler.Handle(invalidRequest, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw DataException when repository throws exception")]
    public async Task Handle_RepositoryThrowsException_ThrowsDataException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var request = new GetSaleCommand(saleId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        var exception = await Assert.ThrowsAsync<DataException>(act);
        exception.Message.Should().Contain($"A database error occurred in the query GetByIdAsync for sale ID {saleId}");
        await _saleRepository.Received(1).GetByIdAsync(saleId, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should throw KeyNotFoundException when sale is not found")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var request = new GetSaleCommand(saleId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        // Act
        var act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(act);
        exception.Message.Should().Contain($"Sales not found {saleId}");
        await _saleRepository.Received(1).GetByIdAsync(saleId, Arg.Any<CancellationToken>());
    }
}