//command for getting the Orders by Product ID in the OrderAPI's DB
using System.Threading;
using System.Threading.Tasks;
using WarehouseAPI.Database.Repository;
using WarehouseAPI.Domain.Entities;
using OrderAPI;
using OrderAPI.Database.Repository;
using OrderAPI.Domain.Entities;
using MediatR;
using AutoMapper;

namespace WarehouseAPI.Service.Query
{
    public class GetOrderByProdIdHandler : IRequestHandler<GetOrderByProdIdQuery, Order>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetOrderByProdIdHandler(IProductRepository productRepository,  IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Order> Handle(GetOrderByProdIdQuery request, CancellationToken cancellationToken)
        {
            var orderEntity = await _productRepository.GetOrderByProdIdAsync(request.ProductId, cancellationToken);

            // Map to the Order model and return it for use in the following process (change calling web-service etc)
            var order = _mapper.Map<Order>(orderEntity);

            return order;
        }
    }
}