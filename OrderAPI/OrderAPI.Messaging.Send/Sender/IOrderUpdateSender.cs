using OrderAPI.Domain.Entities;

namespace OrderAPI.Messaging.Send.Sender
{
    public interface IOrderUpdateSender
    {
        Task SendOrder(Order order);
    }
}