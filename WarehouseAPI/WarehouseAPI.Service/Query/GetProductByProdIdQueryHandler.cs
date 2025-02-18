using System.Threading;
using System.Threading.Tasks;
using WarehouseAPI.Database.Repository;
using WarehouseAPI.Domain.Entities;
using MediatR;
using AutoMapper;

namespace WarehouseAPI.Service.Query
{
    public class GetProductByProdIdQueryHandler : IRequestHandler<GetProductByProdIdQuery, Product>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetProductByProdIdQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Product> Handle(GetProductByProdIdQuery request, CancellationToken cancellationToken)
        {
            var productEntity = await _productRepository.GetProductByProdIdAsync(request.ProductId, cancellationToken);

            // Map to the Product model and return it for use in the following process (search DB etc)
            var product = _mapper.Map<Product>(productEntity);
            return product;
        }
    }
}