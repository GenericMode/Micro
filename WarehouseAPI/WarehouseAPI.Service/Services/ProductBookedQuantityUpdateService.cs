//RabbitMQ service
//logic to update booked quantity when Order is created: +booked quantity - logic in Domain?
//if Order is canceled: -booked quantity (if product still exists) - logic in Domain?

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WarehouseAPI.Database.Repository;
using WarehouseAPI.Domain.Entities;
using MediatR;
using AutoMapper;
using WarehouseAPI.Service.Query;
using WarehouseAPI.Service.Models;
using WarehouseAPI.Service.Command;
using Newtonsoft.Json;

namespace WarehouseAPI.Service.Services
{
public class ProductBookedQuantityUpdateService : IProductBookedQuantityUpdateService
{

    //private readonly IMediator _mediator;
    private readonly ILogger<ProductBookedQuantityUpdateService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory; //we need to create DI scope service for 
    //Mediatr because it calls ProductContext (DB), which is scoped service, and Rabbit service is Singleton,
    //and we cannot use scoped services in singletone service.

    public ProductBookedQuantityUpdateService(IServiceScopeFactory serviceScopeFactory, ILogger<ProductBookedQuantityUpdateService> logger)
    {
        //_mediator = mediator;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }


    public async void UpdateProductQuantityInProducts(ProductBookedQuantityUpdateModel productBookedQuantityUpdateModel)
    {
       
            using (var scope = _serviceScopeFactory.CreateScope())  // 🔹 Create a new DI scope
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>(); // 🔹 Resolve Mediator inside scope

                try
                {
                var productInOrder = await mediator.Send(new GetProductByProdIdQuery
                {
                    ProductId = productBookedQuantityUpdateModel.ProductId
                });

                if (productInOrder != null)
                    {
                    //foreach (var product in productInOrder)
                    //{
                    _logger.LogInformation("productInOrder is not null. Sending product update...");
                    Console.WriteLine($"Fetched Product: {JsonConvert.SerializeObject(productInOrder)}");
                    Console.WriteLine($"Fetched UpdateModel: {JsonConvert.SerializeObject(productBookedQuantityUpdateModel)}");
               

                        if (productBookedQuantityUpdateModel.StatusId == (StatusList.StatusListEnum)1) //created
                        {
                            productInOrder.ProductBookedQuantity += productBookedQuantityUpdateModel.ProductQuantity;
                            Console.WriteLine($"Updated Product Quantity: {productInOrder.ProductBookedQuantity}");
                        }
                    
                        else if (productBookedQuantityUpdateModel.StatusId == (StatusList.StatusListEnum)5) //cancelled
                        {
                            productInOrder.ProductBookedQuantity -= productBookedQuantityUpdateModel.ProductQuantity;
                            Console.WriteLine($"Updated Product Quantity: {productInOrder.ProductBookedQuantity}");
                        }

                   
                    }

                    await mediator.Send(new UpdateProductCommand
                    {
                        Product = productInOrder
                        
                    }); 
                   

                 }
            
                catch (Exception ex)
                {
                        Debug.WriteLine(ex.Message);
                        _logger.LogInformation("Exception");
                }
            }    
    }



}
}   