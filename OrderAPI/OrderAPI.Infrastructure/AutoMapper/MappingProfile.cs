using AutoMapper;
using OrderAPI.Domain.Entities;
using OrderAPI.Models;

namespace OrderAPI.Infrastructure.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateOrderModel, Order>().ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<UpdateOrderModel, Order>();
        }
    }
}