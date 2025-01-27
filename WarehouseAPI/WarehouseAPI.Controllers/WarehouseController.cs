using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using WarehouseAPI.Models;
using WarehouseAPI.Service.Command;
using WarehouseAPI.Service.Query;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Models;
using OrderAPI.Domain.Entities;
using OrderAPI;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using WarehouseAPI.Infrastructure;
using System.Collections.Concurrent;


namespace WarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
 
    public class WarehouseController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;  

        public WarehouseController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary>
        /// Action to see all existing products and their bookings.
        /// </summary>
        /// <returns>Returns a list of all products</returns>
        /// <response code="200">Returned if the products were loaded</response>
        /// <response code="400">Returned if the products couldn't be loaded</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<ActionResult<List<Product>>> Products()
        {
            try
            {
                return await _mediator.Send(new GetProductsQuery());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Action to create a new product and stored quantity in the database.
        /// </summary>
        /// <param name="createProductModel">Model to create a new product</param>
        /// <returns>Returns the created product</returns>
        /// <response code="200">Returned if the product was created</response>
        /// <response code="400">Returned if the model couldn't be parsed or the product couldn't be saved</response>
        /// <response code="422">Returned when the validation failed</response>
        /// 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [HttpPost]
        public async Task<ActionResult<Product>> Product(CreateProductModel createProductModel)
        {
            try
            {
                var newProduct = _mapper.Map<Product>(createProductModel); //map createproductmodel to product WarehouseAPI.Models.CreateProductModel -> WarehouseAPI.Domain.Entities.Product
                
                 // Debugging: Log the mapped product object
                Console.WriteLine("Mapped Product: " + System.Text.Json.JsonSerializer.Serialize(newProduct));

                var createdProduct = await _mediator.Send(new CreateProductCommand { Product = newProduct });

                // Return the created product
                return CreatedAtAction(nameof(Product), new { id = createdProduct.Id }, createdProduct);
                
            }
            catch (AutoMapperMappingException ex)
            {
                Console.WriteLine($"AutoMapper Mapping Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

         /// <summary>
        /// Action to update an existing product, stored and booked quantity
        /// </summary>
        /// <param name="updateOrderModel">Model to update an existing product.</param>
        /// <returns>Returns the updated product</returns>
        /// <response code="200">Returned if the product was updated</response>
        /// <response code="400">Returned if the model couldn't be parsed or the order couldn't be found</response>
        /// <response code="422">Returned when the validation failed</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [HttpPut]

        public async Task<ActionResult<Product>> Product(UpdateProductModel updateProductModel)
        {
            try
            {
                var product = await _mediator.Send(new GetProductByIdQuery
                {
                    Id = updateProductModel.Id
                });

                if (product == null)
                {
                    return BadRequest($"No product found with the id {updateProductModel.Id}");
                }

                return await _mediator.Send(new UpdateProductCommand
                {
                    Product = _mapper.Map(updateProductModel, product)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class UpdateProductOrderWrapper
        {   
            public UpdateProductModel Product { get; set; }

            
            public UpdateOrderModel Order { get; set; }
        }

         /// <summary>
        /// Action to delete an existing product.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [HttpDelete]

         public async Task<ActionResult<Product>> DeleteProduct([FromBody] UpdateProductModel updateProductModel)
        {
            
            try
            {
                var product = await _mediator.Send(new GetProductByIdQuery
                {
                    Id = updateProductModel.Id
                });

                if (product == null)
                {
                    return NotFound($"No product found with the id {updateProductModel.Id}");
                }
                else
                {
                Console.WriteLine($"Fetched Product: {JsonConvert.SerializeObject(product)}");
                Console.WriteLine($"Mapped Product Id (guid): {product.Id}");
                Console.WriteLine($"Mapped ProductId: {product.ProductId}");
                }

                var productId = product.ProductId; //because deleting may cause cleaning the product obj in memory

                var deletedProduct = await _mediator.Send(new DeleteProductCommand
                {
                    Product = _mapper.Map(updateProductModel, product)
                });
                Console.WriteLine("Product deleted.");

                /// If product was deleted, 1) search DB of OrderAPI for getting Order IDs with this products 
                /// 2) Call Put OrderAPI directly to cancel the orders by found IDs
                /// 3) If there are several orders with that ProductId, it should cancel all the orders.
                
                
                var orderEntities = await _mediator.Send(new GetOrderByProdIdQuery
                {
                    ProductId = productId
                });

                if (orderEntities == null)
                {
                    Console.WriteLine("Order not found with the Product id {Product.ProductId}.");
                    return NotFound($"No order found with the Product id {productId}");
                }
            
                else
                {
                Console.WriteLine($"Fetched Order: {JsonConvert.SerializeObject(orderEntities)}");
                }

                var results = new ConcurrentBag<Order>();
                //var tasks = orderEntities.Select(async orderEntity =>
                //foreach (var orderEntity in orderEntities)
                await Parallel.ForEachAsync(orderEntities, async (orderEntity, cancellationToken) =>
                {
                    try
                    {   
                        
                        // Map the orderEntity to UpdateOrderModel 
                        orderEntity.StatusId = (StatusList.StatusListEnum)5;
                        var updateOrderModel = _mapper.Map<UpdateOrderModel>(orderEntity);


                        // Debugging: Ensure mapping worked correctly
                        Console.WriteLine($"Mapped Order: {JsonConvert.SerializeObject(updateOrderModel)}");

                        // Map UpdateOrderModel to Order
                        var order = _mapper.Map<Order>(updateOrderModel);

                        // Debugging
                        Console.WriteLine($"Mapped Order: {JsonConvert.SerializeObject(order)}");
                        
                        
                        var result = await _mediator.Send(new PutOrderByIdQuery
                        {
                        
                            Order = order // Update the order details with the new values

                        });

                        Console.WriteLine("Order updated.");
                        results.Add(result);
                        //return Ok($"Product deleted and associated order updated successfully.");

                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating order: {ex.Message}");
                        //return StatusCode(500, $"Error: {ex.Message}");
                    }
                });
               return Ok(new { message = "Orders processed successfully", data = results });
                
            
            }

            catch (AutoMapperMappingException ex)
            {
                Console.WriteLine($"AutoMapper error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return BadRequest(ex.Message);
            }

             // Add a final return statement in case none of the above paths are executed
            return BadRequest("An unknown error occurred while processing your request.");
        }

    }
        

}