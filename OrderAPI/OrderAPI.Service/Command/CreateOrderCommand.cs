using OrderAPI.Domain.Entities;
using MediatR; //MediatR pattern code (Service folder)
using OrderAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace OrderAPI.Service.Command
{
    public class CreateOrderCommand : IRequest<Order>
    {
        public Order Order { get; set; }
    }
}