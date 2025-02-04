using System.Threading;
using System.Threading.Tasks;
using OrderAPI.Database.Repository;
using OrderAPI.Domain.Entities;
using MediatR;

namespace OrderAPI.Service.Query
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Order>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Order> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            return await _orderRepository.GetOrderByIdAsync(request.Id, cancellationToken);
        }
    }
}