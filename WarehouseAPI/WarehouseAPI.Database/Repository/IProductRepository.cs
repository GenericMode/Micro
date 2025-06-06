using System;
using System.Threading;
using System.Threading.Tasks;
using WarehouseAPI.Domain.Entities;
using OrderAPI;
using OrderAPI.Database;
using OrderAPI.Database.Repository;
using OrderAPI.Domain.Entities;

namespace WarehouseAPI.Database.Repository
{
    public interface IProductRepository: IRepository<Product>
    {
        Task<Product> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Product> GetProductByProdIdAsync(int? productid, CancellationToken cancellationToken);
        Task<List<Order>> GetOrderByProdIdAsync(int? productid, CancellationToken cancellationToken);
    

    }
}