//deal with database directly or use repository design for the greater Maintainability, Testability, 
//Flexibility (for example changing DB), if we use repository design we focuses purely on the business logic here
using System.Threading;
using System.Threading.Tasks;
using WarehouseAPI.Database.Repository;
using WarehouseAPI.Domain.Entities;
using MediatR;
using AutoMapper;

namespace WarehouseAPI.Service.Command
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            if (request.Product == null)
            {
                throw new InvalidOperationException("Product cannot be null");
            }
            return await _productRepository.AddAsync(request.Product);
        }
    }
}