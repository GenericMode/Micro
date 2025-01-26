using System.Collections.Generic;
using WarehouseAPI.Domain.Entities;
using MediatR;

namespace WarehouseAPI.Service.Query
{
    public class GetProductsQuery : IRequest<List<Product>>
    {
    }
}