using System;
using WarehouseAPI.Domain.Entities;
using MediatR;

namespace WarehouseAPI.Service.Query
{
    public class GetProductByProdIdQuery : IRequest<Product>
    {
        public int? ProductId { get; set; }
    }
}