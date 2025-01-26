using AutoMapper;
using WarehouseAPI.Domain.Entities;
using WarehouseAPI.Models;
using OrderAPI.Domain.Entities;
using OrderAPI.Models;

namespace WarehouseAPI.Infrastructure.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateProductModel, Product>()
            .ForMember(x => x.Id, opt => opt.Ignore());
            //.ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Map all other members if they are not null

            CreateMap<UpdateProductModel, Product>();

            CreateMap<UpdateOrderModel, Order>()
            //.ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => (StatusList.StatusListEnum)Enum.Parse(typeof(StatusList.StatusListEnum), src.StatusId.ToString())))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Map all other members if they are not null

                
        // Reverse mapping from Order to UpdateOrderModel (without StatusId)
            CreateMap<Order, UpdateOrderModel>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Map all other members if they are not null

            //CreateMap<StatusList, StatusList>();
       
        }
    }
}