using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ZeusApp.Application.Features.Orders.Commands.Create;
using ZeusApp.Application.Features.Orders.Commands.Update;
using ZeusApp.Application.Features.Orders.Queries.GetAllPaged;
using ZeusApp.Application.Features.Orders.Queries.GetById;
using ZeusApp.Domain.Entities.Catalog;

namespace ZeusApp.Application.Mappings;
public class OrderMapProfile:Profile
{
    public OrderMapProfile()
    {
        CreateMap<CreateOrderCommand, Order>().ReverseMap();
        CreateMap<UpdateOrderCommand, Order>().ReverseMap();
        CreateMap<GetOrderByIdQuery, Order>().ReverseMap();
        CreateMap<GetAllOrdersQuery, Order>().ReverseMap();
    }
}
