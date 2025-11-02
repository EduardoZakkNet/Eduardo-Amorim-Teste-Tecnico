using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AutoMapper;
using FluentValidation;
using System.Text.Json;
using Ambev.DeveloperEvaluation.Application.Config;
using Ambev.DeveloperEvaluation.Application.Service;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;

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

    /// <summary>
    /// Initializes a new instance of CreateUserHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
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
    /// <param name="command">The CreateUser command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var sale = _mapper.Map<Sale>(command);
        if (sale.Items != null && sale.Items.Any())
        {
            ValidateItemsSale(sale.Items);
            sale.Items = CalculateDiscountPercentage(sale.Items);
            sale.TotalSaleAmount = sale.Items.Sum(i => i.TotalItem);
        }

        var createdUser = await _saleRepository.CreateAsync(sale, cancellationToken);
        var result = _mapper.Map<CreateSaleResult>(createdUser);
        await PublishSaleCreatedEventAsync(result);
        
        return result;
    }
        
    /// <summary>
    /// Method the ValidateItemsSale
    /// </summary>
    /// <param name="items">The List of Items in the Sale</param>
    /// <returns>Validates if the total quantity of Items exceeds the limit</returns>
    private void ValidateItemsSale(IEnumerable<ItemSale> items)
    {
        var itemsGroup = items
            .GroupBy(i => i.Description)
            .Select(g => new 
            { 
                Description = g.Key, 
                Quantity = g.Sum(i => i.Quantity) 
            })
            .ToList();
        
        var invalidProducts = itemsGroup
            .Where(p => p.Quantity > 20)
            .ToList();
        
        if (invalidProducts.Any())
        {
            var itemsDescriptions = string.Join(", ", invalidProducts.Select(p => p.Description));
            throw new InvalidOperationException(
                $"The product exceeds the maximum limit of 20 items per product: {itemsDescriptions}."
            );
        }
    }
    
    /// <summary>
    /// Method the ValidateProducts
    /// </summary>
    /// <param name="items">The items of the Sale</param>
    /// <returns>Calculates the discount percentage based on the quantity</returns>
    private IEnumerable<ItemSale> CalculateDiscountPercentage(IEnumerable<ItemSale> items)
    {
        var calculateDiscountPercentage = items.ToList();
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
        
        //Apenas uma simulação, para que não ocorra erro, foi comentado para que seja logado simulando o Publish original no kafka
        //await kafkaService.PublicarAsync(message, keySecurityKafka);
        _logger.LogInformation("Event published to Kafka successfully - Topic: {Nome}", config.TopicName);
        return Task.CompletedTask;
    }
}