using System.Data;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AutoMapper;
using FluentValidation;
using System.Text.Json;
using Ambev.DeveloperEvaluation.Application.Config;
using Ambev.DeveloperEvaluation.Application.Service;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using ArithmeticException = System.ArithmeticException;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand requests
/// </summary>
public class CreateSaleHandler: IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CreateSaleHandler> _logger;
    public event EventHandler<PurchaseCreatedEventArgs> _eventPurchase;

    /// <summary>
    /// Initializes a new instance of CreateUserHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="configuration"></param>
    /// <param name="logger">Logger of aplication</param>
    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IConfiguration configuration, 
        ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _configuration = configuration;
        _logger = logger;
    }
    
    /// <summary>
    /// Handles the CreateSaleCommand request
    /// </summary>
    /// <param name="command">The CreateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        Sale? createdSale;
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var sale = _mapper.Map<Sale>(command);
        sale.Status = SaleStatus.Active;
        
        if (sale.Items != null && sale.Items.Any())
        {
            await ValidateSaleNumberAsync(command.SaleNumber);
            ValidateItemsSale(sale.Items, command.SaleNumber);
            sale.Items = CalculateDiscountPercentage(sale.Items, command.SaleNumber);
            sale.TotalSaleAmount = sale.Items.Sum(i => i.TotalItem);
        }

        try
        {
            createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError($"A database error occurred in the query CreateAsync for Sale Number {command.SaleNumber} on {DateTime.UtcNow}");
            throw new DataException($"A database error occurred in the query CreateAsync for Sale Number {command.SaleNumber} on {DateTime.UtcNow}");
        }

        var result = _mapper.Map<CreateSaleResult>(createdSale);
        await PublishSaleCreatedEventAsync(result);
        
        return result;
    }

    /// <summary>
    /// Method the ValidateSaleNumber
    /// </summary>
    /// <param name="saleNumber"></param>
    /// <returns>Validates if the total quantity of Items exceeds the limit</returns>
    private async Task ValidateSaleNumberAsync(int saleNumber)
    {
        var saleExists = await _saleRepository.GetBySaleNumberAsync(saleNumber) != null;
    
        if (saleExists)
        {
            var message = $"A sale with Sale Number {saleNumber} already exists.";
            _logger.LogError(message);
            throw new InvalidOperationException(message);
        }
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
    private Task PublishSaleCreatedEventAsync(CreateSaleResult result)
    {
        var config = new SaleCreatedIntegrationKafkaConfig();
        var bootstrapServers = _configuration["AmbevServerKafka:uri"];
        var keySecurityKafka = _configuration["AmbevServerKafka:key"];

        if (bootstrapServers != null)
        {
            using var kafkaService = new KafkaProducerService<SaleCreatedIntegrationKafkaConfig>(bootstrapServers, config);
        }

        var message = JsonSerializer.Serialize(result);
        
        //Apenas uma simulação, Utilizando o kafka como configuração, mas também pode ser usado qualquer outro (Rabbit, Azure)
        _eventPurchase?.Invoke(this, new PurchaseCreatedEventArgs(result.Id, DateTime.Now));
        
        _logger.LogInformation("Event PurchaseCreatedEventArgs published on Kafka successfully - Topic: {Nome}", config.TopicName);
        return Task.CompletedTask;
    }
}