using System;
using WarehouseAPI.Domain.Entities;
using MediatR;

namespace WarehouseAPI.Service.Query
{
    public class GetProductByIdQuery : IRequest<Product>
    {
        public Guid Id { get; set; }
    }
}