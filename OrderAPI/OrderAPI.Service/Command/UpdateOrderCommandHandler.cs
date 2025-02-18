using System.Threading;
using System.Threading.Tasks;
using OrderAPI.Database.Repository;
using OrderAPI.Domain.Entities;
using OrderAPI.Messaging.Send.Sender;
using MediatR;

namespace OrderAPI.Service.Command
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Order>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderUpdateSender _orderUpdateSender;
        private readonly ILogger<UpdateOrderCommandHandler> _logger;

        public UpdateOrderCommandHandler(IOrderUpdateSender orderUpdateSender, IOrderRepository orderRepository, ILogger<UpdateOrderCommandHandler> logger)
        {
            _orderUpdateSender = orderUpdateSender ?? throw new ArgumentNullException(nameof(orderUpdateSender));;
            _orderRepository = orderRepository;
            _logger = logger;
            _logger.LogInformation("UpdateOrderCommandHandler constructed successfully.");
        }

        

         public UpdateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Order> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.UpdateAsync(request.Order);

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