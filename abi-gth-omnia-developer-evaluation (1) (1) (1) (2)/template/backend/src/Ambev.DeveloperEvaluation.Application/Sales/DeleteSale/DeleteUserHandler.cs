using System.Data;
using System.Text.Json;
using Ambev.DeveloperEvaluation.Application.Config;
using Ambev.DeveloperEvaluation.Application.Service;
using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Exception = System.Exception;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Handler for processing DeleteSaleCommand requests
/// </summary>
public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<DeleteSaleHandler> _logger;
    public event EventHandler<PurchaseCancelledEventArgs> _eventPurchase;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of DeleteSaleHandler
    /// </summary>
    /// <param name="saleRepository">The Sale repository</param>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    public DeleteSaleHandler(
        ISaleRepository saleRepository, ILogger<DeleteSaleHandler> logger, IConfiguration configuration)
    {
        _saleRepository = saleRepository;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Handles the DeleteSaleCommand request
    /// </summary>
    /// <param name="request">The DeleteSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteSaleResponse> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        bool success;
        try
        {
            success = await _saleRepository.DeleteAsync(request.Id, cancellationToken);
        }
        catch (Exception e)
        {
            var message =
                $"A database error occurred in the query DeleteAsync for sale ID {request.Id} on {DateTime.UtcNow}";
            _logger.LogError(message);
            throw new DataException(message);
        }

        if (!success)
        {
            var message = $"Sales not found {request.Id} on {DateTime.UtcNow}";
            _logger.LogError(message);
            throw new KeyNotFoundException(message);
        }
        
        await PublishSaleDeletedEventAsync(request);
        return new DeleteSaleResponse { Success = true };
    }
    
    /// <summary>
    /// Method the ValidateProducts
    /// </summary>
    /// <param name="result">The Sale entity</param>
    /// <returns>Publish in kafka the new sale information</returns>
    private Task PublishSaleDeletedEventAsync(DeleteSaleCommand request)
    {
        var config = new SaleCancelledIntegrationKafkaConfig();
        var bootstrapServers = _configuration["AmbevServerKafka:uri"];
        var keySecurityKafka = _configuration["AmbevServerKafka:key"];

        if (bootstrapServers != null)
        {
            using var kafkaService = new KafkaProducerService<SaleCancelledIntegrationKafkaConfig>(bootstrapServers, config);
        }

        var message = JsonSerializer.Serialize(request);
        
        //Apenas uma simulação, Utilizando o kafka como configuração, mas também pode ser usado qualquer outro (Rabbit, Azure)
        _eventPurchase?.Invoke(this, new PurchaseCancelledEventArgs(request.Id, DateTime.Now));
        
        _logger.LogInformation("Event PurchaseCancelledEventArgs published on Kafka successfully - Topic: {Nome}", config.TopicName);
        return Task.CompletedTask;
    }
}