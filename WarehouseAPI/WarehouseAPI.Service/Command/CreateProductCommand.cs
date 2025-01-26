using WarehouseAPI.Domain.Entities;
using MediatR; //MediatR pattern code (Service folder)
using WarehouseAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WarehouseAPI.Service.Command
{
    public class CreateProductCommand : IRequest<Product>
    {
        public Product Product { get; set; }
    }
}