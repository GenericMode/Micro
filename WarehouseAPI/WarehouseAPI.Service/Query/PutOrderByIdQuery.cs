//Command for the direct call the Order API and change the order
using System.Threading;
using System.Threading.Tasks;
using WarehouseAPI.Database.Repository;
using WarehouseAPI.Domain.Entities;
using OrderAPI;
using OrderAPI.Database.Repository;
using OrderAPI.Domain.Entities;
using MediatR;
using OrderAPI.Models;

namespace WarehouseAPI.Service.Query
{
    public class PutOrderByIdQuery : IRequest<Order>
    {
    
        public Order Order { get; set; }

    }
}   