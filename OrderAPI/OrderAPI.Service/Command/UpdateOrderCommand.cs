using OrderAPI.Domain.Entities;
using MediatR;

namespace OrderAPI.Service.Command
{
    public class UpdateOrderCommand : IRequest<Order>
    {
        public Order Order { get; set; }
    }
}