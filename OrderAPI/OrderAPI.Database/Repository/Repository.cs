using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderAPI.Database;

namespace OrderAPI.Database.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        protected readonly OrderContext OrderContext;

        public Repository(OrderContext orderContext)
        {
            OrderContext = orderContext;
        }

        public IEnumerable<TEntity> GetAll()
        {
            try
            {
                return OrderContext.Set<TEntity>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve entities: {ex.Message}");
            }
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }

            try
            {
                await OrderContext.AddAsync(entity);
                await OrderContext.SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be saved: {ex.Message}");
            }
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(UpdateAsync)} entity must not be null");
            }

            try
            {
                OrderContext.Update(entity);
                await OrderContext.SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be updated {ex.Message}");
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is Repository<TEntity> repository &&
                   EqualityComparer<OrderContext>.Default.Equals(OrderContext, repository.OrderContext);
        }
    }
}