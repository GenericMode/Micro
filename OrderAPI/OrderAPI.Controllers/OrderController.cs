using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using OrderAPI.Models;
using OrderAPI.Service.Command;
using OrderAPI.Service.Query;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace OrderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
 
    public class OrderController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;  

        public OrderController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary>
        /// Action to see all existing orders.
        /// </summary>
        /// <returns>Returns a list of all orders</returns>
        /// <response code="200">Returned if the orders were loaded</response>
        /// <response code="400">Returned if the orders couldn't be loaded</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> Orders()
        {
            try
            {
                return await _mediator.Send(new GetOrdersQuery());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Action to create a new order in the database.
        /// </summary>
        /// <param name="createOrderModel">Model to create a new order</param>
        /// <returns>Returns the created order</returns>
        /// <response code="200">Returned if the order was created</response>
        /// <response code="400">Returned if the model couldn't be parsed or the order couldn't be saved</response>
        /// <response code="422">Returned when the validation failed</response>
        /// 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [HttpPost]

        public async Task<ActionResult<Order>> Order(CreateOrderModel createOrderModel)
        {
            try
            {
                return await _mediator.Send(new CreateOrderCommand
                {
                    Order = _mapper.Map<Order>(createOrderModel)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

         /// <summary>
        /// Action to update an existing order
        /// </summary>
        /// <param name="updateOrderModel">Model to update an existing order.</param>
        /// <returns>Returns the updated order</returns>
        /// <response code="200">Returned if the order was updated</response>
        /// <response code="400">Returned if the model couldn't be parsed or the order couldn't be found</response>
        /// <response code="422">Returned when the validation failed</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [HttpPut]

        public async Task<ActionResult<Order>> Order(UpdateOrderModel updateOrderModel)
        {
            try
            {
                var order = await _mediator.Send(new GetOrderByIdQuery
                {
                    Id = updateOrderModel.Id
                });

                if (order == null)
                {
                    return BadRequest($"No order found with the id {updateOrderModel.Id}");
                }

                return await _mediator.Send(new UpdateOrderCommand
                {
                    Order = _mapper.Map(updateOrderModel, order)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
        

}