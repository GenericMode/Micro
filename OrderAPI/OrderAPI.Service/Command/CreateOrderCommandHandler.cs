//deal with database directly or use repository design for the greater Maintainability, Testability, 
//Flexibility (for example changing DB), if we use repository design we focuses purely on the business logic here
using System.Threading;
using System.Threading.Tasks;
using OrderAPI.Database.Repository;
using OrderAPI.Domain.Entities;
using OrderAPI.Messaging.Send.Sender;
using MediatR;

namespace OrderAPI.Service.Command
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderUpdateSender _orderUpdateSender;
        private readonly ILogger<UpdateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(IOrderUpdateSender orderUpdateSender, IOrderRepository orderRepository, ILogger<UpdateOrderCommandHandler> logger)
        {
            _orderUpdateSender = orderUpdateSender ?? throw new ArgumentNullException(nameof(orderUpdateSender));;
            _orderRepository = orderRepository;
            _logger = logger;
            _logger.LogInformation("UpdateOrderCommandHandler constructed successfully.");
        }

        public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.AddAsync(request.Order);

            if (_orderUpdateSender == null)
                {
                    Console.WriteLine("🚨 _orderUpdateSender is NULL! Make sure it's registered in DI.");
                    throw new InvalidOperationException("OrderUpdateSender was not initialized.");
                }
            else
            {
                    _logger.LogInformation("OrderUpdateSender is not null. Sending order update...");
            }    


            await _orderUpdateSender.SendOrder(order);

            return order;


        }
    }
}