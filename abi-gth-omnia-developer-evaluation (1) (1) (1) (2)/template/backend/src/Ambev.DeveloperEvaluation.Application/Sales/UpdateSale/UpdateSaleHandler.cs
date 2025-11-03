using System.Data;
using System.Text.Json;
using Ambev.DeveloperEvaluation.Application.Config;
using Ambev.DeveloperEvaluation.Application.Service;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleCommand requests
/// </summary>
public class UpdateSaleHandler: IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UpdateSaleHandler> _logger;
    public event EventHandler<PurchaseChangeEventArgs> _eventPurchase;

    /// <summary>
    /// Initializes a new instance of UpdateUserHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="configuration"></param>
    /// <param name="logger">Logger of aplication</param>
    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IConfiguration configuration, 
        ILogger<UpdateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _configuration = configuration;
        _logger = logger;
    }
    
    /// <summary>
    /// Handles the UpdateSaleCommand request
    /// </summary>
    /// <param name="command">The UpdateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        Sale? updatedSale;
        var validator = new UpdateSaleCommandValidador();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var sale = GetSaleAsync(command.Id);
        var existendSale =  _mapper.Map<Sale>(command);
        
        existendSale.UpdatedAt = DateTime.UtcNow;
        
        if (existendSale.Items != null && existendSale.Items.Any())
        {
            foreach (var item in existendSale.Items)
            {
                item.SaleId = existendSale.Id;
                item.UpdatedAt = DateTime.UtcNow;
            }
            
            ValidateItemsSale(existendSale.Items, command.SaleNumber);
            existendSale.Items = CalculateDiscountPercentage(existendSale.Items, command.SaleNumber);
            existendSale.TotalSaleAmount = existendSale.Items.Sum(i => i.TotalItem);
        }

        try
        {
            updatedSale = await _saleRepository.UpdateAsync(existendSale, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError($"A database error occurred in the query CreateAsync for Sale Number {command.SaleNumber} on {DateTime.UtcNow}");
            throw new DataException($"A database error occurred in the query CreateAsync for Sale Number {command.SaleNumber} on {DateTime.UtcNow}");
        }

        var result = _mapper.Map<UpdateSaleResult>(updatedSale);
        await PublishSaleCreatedEventAsync(result);
        
        return result;
    }

    /// <summary>
    /// Method the Get Sale
    /// </summary>
    /// <param name="saleNumber"></param>
    /// <returns>Validates if the total quantity of Items exceeds the limit</returns>
    private async Task<Sale> GetSaleAsync(Guid id)
    {
        Sale? sale;
        try
        { 
            sale = await _saleRepository.GetByIdAsync(id);
        }
        catch (Exception e)
        {
            var message = $"A database error occurred in the query GetByIdAsync for sale ID {id} on {DateTime.UtcNow}";
            _logger.LogError(message);
            throw new DataException(message);
        }

        if (sale == null)
        {
            var message = $"Sales not found {id} on {DateTime.UtcNow}";
            _logger.LogError(message);
            throw new KeyNotFoundException(message);
        }

        return sale;
    }
    
    /// <summary>
    /// Method the ValidateItemsSale
    /// </summary>
    /// <param name="items">The List of Items in the Sale</param>
    /// <param name="saleNumber"></param>
    /// <returns>Validates if the total quantity of Items exceeds the limit</returns>
    private void ValidateItemsSale(IEnumerable<ItemSale> items, int saleNumber)
    {
        var itemsGroup = items
            .GroupBy(i => i.Description)
            .Select(g => new 
            { 
                Description = g.Key, 
                Quantity = g.Sum(i => i.Quantity) 
            })
            .ToList();
        
        var totalQuantity = itemsGroup.Sum(p => p.Quantity);
        
        if (totalQuantity > 20)
        {
            var itemsDescriptions = itemsGroup.Select(c => c.Description).FirstOrDefault();

            var message =
                $"The product exceeds the maximum limit of 20 items per product: {itemsDescriptions} Sale Number: {saleNumber}.";
            _logger.LogError(message);
            throw new InvalidOperationException(message);
        }
    }

    /// <summary>
    /// Method the ValidateProducts
    /// </summary>
    /// <param name="items">The items of the Sale</param>
    /// <param name="saleNumber"></param>
    /// <returns>Calculates the discount percentage based on the quantity</returns>
    private List<ItemSale> CalculateDiscountPercentage(IEnumerable<ItemSale> items, int saleNumber)
    {
        var calculateDiscountPercentage = items.ToList();
        
        try
        {
            var itemsGroup = calculateDiscountPercentage
                .GroupBy(i => i.Description)
                .Select(g => new 
                { 
                    Description = g.Key, 
                    Quantity = g.Sum(i => i.Quantity) 
                })
                .ToList();

            foreach (var itemGroup in itemsGroup)
            {
                if (itemGroup.Quantity < 4)
                    foreach (var item in calculateDiscountPercentage.Where(i => i.Description == itemGroup.Description))
                        item.Discount = 0m;
            
                if (itemGroup.Quantity <= 9)
                    foreach (var item in calculateDiscountPercentage.Where(i => i.Description == itemGroup.Description))
                        item.Discount = 0.10m;
                
                if (itemGroup.Quantity <= 20)
                    foreach (var item in calculateDiscountPercentage.Where(i => i.Description == itemGroup.Description))
                        item.Discount = 0.20m; 
            }

            CalculateTotalItemAmount(calculateDiscountPercentage);
        }
        catch (Exception e)
        {
            var message = $"It was not possible to calculate the discount for Sale Number {saleNumber}.";
            _logger.LogError(message);
            throw new ArithmeticException(message);
        }
        
        return calculateDiscountPercentage;
    }
    
    /// <summary>
    /// Method the ValidateProducts
    /// </summary>
    /// <param name="items">The Item of the Sale</param>
    /// <returns>Calculates the total value of the item considering the discount</returns>
    private IEnumerable<ItemSale> CalculateTotalItemAmount(IEnumerable<ItemSale> items)
    {
        var calculateTotalItemAmount = items.ToList();
        foreach (var item in calculateTotalItemAmount)
            item.TotalItem = item.Quantity * item.UnitValue - item.Discount;

        return calculateTotalItemAmount;
    }
    
    /// <summary>
    /// Method the ValidateProducts
    /// </summary>
    /// <param name="result">The Sale entity</param>
    /// <returns>Publish in kafka the new sale information</returns>
    private Task PublishSaleCreatedEventAsync(UpdateSaleResult result)
    {
        var config = new SaleModifiedIntegrationKafkaConfig();
        var bootstrapServers = _configuration["AmbevServerKafka:uri"];
        var keySecurityKafka = _configuration["AmbevServerKafka:key"];

        if (bootstrapServers != null)
        {
            using var kafkaService = new KafkaProducerService<SaleModifiedIntegrationKafkaConfig>(bootstrapServers, config);
        }

        var message = JsonSerializer.Serialize(result);
        
        //Apenas uma simulação, Utilizando o kafka como configuração, mas também pode ser usado qualquer outro (Rabbit, Azure)
        _eventPurchase?.Invoke(this, new PurchaseChangeEventArgs(result.Id, DateTime.Now));
        
        _logger.LogInformation("Event PurchaseChangeEventArgs published on Kafka successfully - Topic: {Nome}", config.TopicName);
        return Task.CompletedTask;
    }
}