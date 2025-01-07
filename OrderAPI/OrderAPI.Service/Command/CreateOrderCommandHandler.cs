//deal with database directly or use repository design for the greater Maintainability, Testability, 
//Flexibility (for example changing DB), if we use repository design we focuses purely on the business logic here
using System.Threading;
using System.Threading.Tasks;
using OrderAPI.Database.Repository;
using OrderAPI.Domain.Entities;
using MediatR;

namespace OrderAPI.Service.Command
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            return await _orderRepository.AddAsync(request.Order);
        }
    }
}