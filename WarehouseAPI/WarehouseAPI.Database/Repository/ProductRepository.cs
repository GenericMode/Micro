//using System;
//using System.Threading;
//using System.Threading.Tasks;
using WarehouseAPI.Domain.Entities;
using WarehouseAPI.Database;
using Microsoft.EntityFrameworkCore;
using OrderAPI;
using OrderAPI.Database;
using OrderAPI.Database.Repository;
using OrderAPI.Domain.Entities;

namespace WarehouseAPI.Database.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        //public ProductRepository(ProductContext productContext) : base(productContext)
        //{
        //}

        private readonly ProductContext _productContext;
        private readonly OrderContext _orderContext;
        public ProductRepository(ProductContext productContext, OrderContext orderContext) : base(productContext)
        {
        _orderContext = orderContext;
        _productContext = productContext;

        }

        public async Task<Product> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _productContext.Product.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        

        public async Task<Order> GetOrderByProdIdAsync(int? productid, CancellationToken cancellationToken)
        {
            return await _orderContext.Order.FirstOrDefaultAsync(x => x.ProductId == productid, cancellationToken); 
        }
    }
}