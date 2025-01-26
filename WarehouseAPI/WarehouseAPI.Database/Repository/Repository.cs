using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseAPI.Database;
using OrderAPI.Database;

namespace WarehouseAPI.Database.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        protected readonly ProductContext ProductContext;

        public Repository(ProductContext productContext)
        {
            ProductContext = productContext;
        }

        public IEnumerable<TEntity> GetAll()
        {
            try
            {
                return ProductContext.Set<TEntity>();
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
                await ProductContext.AddAsync(entity);
                await ProductContext.SaveChangesAsync();

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
                ProductContext.Update(entity);
                await ProductContext.SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be updated {ex.Message}");
            }
        }

        public async Task<TEntity> DeleteAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(DeleteAsync)} entity must not be null");
            }

            try
            {
                ProductContext.Remove(entity);
                await ProductContext.SaveChangesAsync();

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
                   EqualityComparer<ProductContext>.Default.Equals(ProductContext, repository.ProductContext);
        }
    }
}