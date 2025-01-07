using System.Threading;
using System.Threading.Tasks;
using OrderAPI.Database.Repository;
using OrderAPI.Domain.Entities;
//using OrderAPI.Messaging.Send.Sender.v1;
using MediatR;

namespace OrderAPI.Service.Command
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Order>
    {
        private readonly IOrderRepository _orderRepository;
        //private readonly IOrderUpdateSender _orderUpdateSender;

        //public UpdateOrderCommandHandler(IOrderUpdateSender orderUpdateSender, IOrderRepository orderRepository)
        //{
        //    _orderUpdateSender = orderUpdateSender;
        //    _orderRepository = orderRepository;
        //}

        //temporary variant

         public UpdateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Order> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.UpdateAsync(request.Order);

            //_orderUpdateSender.SendOrder(order);

            return order;
        }
    }
}