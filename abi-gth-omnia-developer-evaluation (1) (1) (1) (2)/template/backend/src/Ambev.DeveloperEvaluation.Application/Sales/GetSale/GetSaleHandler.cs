using System.Data;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Handler for processing GetSaleCommand requests
/// </summary>
public class GetSaleHandler : IRequestHandler<GetSaleCommand, GetSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSaleHandler> _logger;

    /// <summary>
    /// Initializes a new instance of GetSaleHandler
    /// </summary>
    /// <param name="saleRepository">The Sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="validator">The validator for GetSaleCommand</param>
    /// <param name="logger"></param>
    public GetSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper, ILogger<GetSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetSaleCommand request
    /// </summary>
    /// <param name="request">The GetSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The Sale details if found</returns>
    public async Task<GetSaleResult> Handle(GetSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new GetSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        Sale? sale;
        
        try
        { 
            sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        }
        catch (Exception e)
        {
            var message = $"A database error occurred in the query GetByIdAsync for sale ID {request.Id} on {DateTime.UtcNow}";
            _logger.LogError(message);
            throw new DataException(message);
        }

        if (sale == null)
        {
            var message = $"Sales not found {request.Id} on {DateTime.UtcNow}";
            _logger.LogError(message);
            throw new KeyNotFoundException(message);
        }
        
        return _mapper.Map<GetSaleResult>(sale);
    }
}