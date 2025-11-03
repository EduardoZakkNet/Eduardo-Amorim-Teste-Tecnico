using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// Profile for mapping GetSale feature requests to commands
/// </summary>
public class GetSaleProfile: Profile
{
    /// <summary>
    /// Initializes the mappings for GetSale feature
    /// </summary>
    public GetSaleProfile()
    {
        CreateMap<GetSaleRequest, GetSaleCommand>();
        CreateMap<GetSaleResult, GetSaleResponse>();
        
        CreateMap<Guid, Application.Sales.GetSale.GetSaleCommand>()
            .ConstructUsing(id => new Application.Sales.GetSale.GetSaleCommand(id));
    }
}