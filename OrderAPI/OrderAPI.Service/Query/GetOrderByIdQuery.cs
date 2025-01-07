using System;
using OrderAPI.Domain.Entities;
using MediatR;

namespace OrderAPI.Service.Query
{
    public class GetOrderByIdQuery : IRequest<Order>
    {
        public Guid Id { get; set; }
    }
}