using System.Collections.Generic;
using OrderAPI.Domain.Entities;
using MediatR;

namespace OrderAPI.Service.Query
{
    public class GetOrdersQuery : IRequest<List<Order>>
    {
    }
}