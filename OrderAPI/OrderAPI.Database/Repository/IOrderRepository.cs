using System;
using System.Threading;
using System.Threading.Tasks;
using OrderAPI.Domain.Entities;

namespace OrderAPI.Database.Repository
{
    public interface IOrderRepository: IRepository<Order>
    {
        Task<Order> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}