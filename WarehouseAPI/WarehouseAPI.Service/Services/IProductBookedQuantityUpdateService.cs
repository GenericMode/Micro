using System.Threading;
using System.Threading.Tasks;
using WarehouseAPI.Database.Repository;
using WarehouseAPI.Domain.Entities;
using WarehouseAPI.Service.Models;
using MediatR;
using AutoMapper;

namespace WarehouseAPI.Service.Services
{
    public interface IProductBookedQuantityUpdateService
    {
        void UpdateProductQuantityInProducts(ProductBookedQuantityUpdateModel productBookedQuantityUpdateModel);
    }
}   