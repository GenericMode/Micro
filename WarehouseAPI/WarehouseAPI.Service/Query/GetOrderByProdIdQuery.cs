//command for getting the Orders by Product ID in the OrderAPI's DB
using System;
using WarehouseAPI.Domain.Entities;
using OrderAPI.Domain.Entities;
using MediatR;

namespace WarehouseAPI.Service.Query
{
    public class GetOrderByProdIdQuery : IRequest<List<Order>>
    {
        public int? ProductId { get; set; }
    }
}