using System.Threading;
using System.Threading.Tasks;
using WarehouseAPI.Database.Repository;
using WarehouseAPI.Domain.Entities;
using MediatR;
using AutoMapper;

namespace WarehouseAPI.Service.Models
{
    public class ProductBookedQuantityUpdateModel
    {


        public int? ProductId { get; set; }
        public int? ProductQuantity { get; set; }
        public StatusList.StatusListEnum StatusId { get; set; }

    }
}   