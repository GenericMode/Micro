using WarehouseAPI.Domain.Entities;
using MediatR;

namespace WarehouseAPI.Service.Command
{
    public class DeleteProductCommand : IRequest<Product>
    {
        public Product Product { get; set; }
    }
}