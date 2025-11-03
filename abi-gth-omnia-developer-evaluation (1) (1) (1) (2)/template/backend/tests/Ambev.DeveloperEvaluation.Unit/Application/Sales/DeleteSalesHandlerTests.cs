using System.Data;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
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
/// Contains unit tests for the <see cref="DeleteSalesHandlerTests"/> class.
/// </summary>
public class DeleteSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly DeleteSaleHandler _handler;
    private readonly ILogger<DeleteSaleHandler> _logger;
    private readonly DeleteSaleValidator _validator;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSalesHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public DeleteSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _logger = Substitute.For<ILogger<DeleteSaleHandler>>();
        _configuration = Substitute.For<IConfiguration>();
        _handler = new DeleteSaleHandler(_saleRepository, _logger, _configuration);
        _validator = new DeleteSaleValidator();
    }

    /// <summary>
    /// Tests that a valid sale delete request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When delete sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = DeleteUserHandlerTestData.GenerateValidCommand();
        command = new DeleteSaleCommand(Guid.NewGuid());
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

        _saleRepository.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // When
        var deleteSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        deleteSaleResult.Should().NotBeNull();
        deleteSaleResult.Success.Should().Be(true);

        // Assert
        _logger.Received(1);

        await _saleRepository.Received(1).DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should throw ValidationException when request is invalid")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var invalidRequest = new DeleteSaleCommand(Guid.Empty);

        var handler = new DeleteSaleHandler(_saleRepository, _logger, _configuration);

        // Act & Assert
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () =>
            await handler.Handle(invalidRequest, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw DataException when repository throws exception")]
    public async Task Handle_RepositoryThrowsException_ThrowsDataException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var request = new DeleteSaleCommand(saleId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(act);
        exception.Message.Should().Contain($"Sales not found {saleId}");
        await _saleRepository.Received(1).DeleteAsync(saleId, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should throw KeyNotFoundException when sale is not found")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var request = new DeleteSaleCommand(saleId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        // Act
        var act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(act);
        exception.Message.Should().Contain($"Sales not found {saleId}");
        await _saleRepository.Received(1).DeleteAsync(saleId, Arg.Any<CancellationToken>());
    }
}