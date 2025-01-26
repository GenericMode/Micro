using WarehouseAPI.Domain.Entities;
using MediatR;

namespace WarehouseAPI.Service.Command
{
    public class UpdateProductCommand : IRequest<Product>
    {
        public Product Product { get; set; }
    }
}