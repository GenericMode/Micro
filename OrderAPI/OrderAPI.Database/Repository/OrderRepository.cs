//using System;
//using System.Threading;
//using System.Threading.Tasks;
using OrderAPI.Domain.Entities;
using OrderAPI.Database;
using Microsoft.EntityFrameworkCore;


namespace OrderAPI.Database.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(OrderContext orderContext) : base(orderContext)
        {
        }

        public async Task<Order> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await OrderContext.Order.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}