using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZeusApp.Application.Features.Orders.Commands.Create;
using ZeusApp.Application.Features.Orders.Commands.Delete;
using ZeusApp.Application.Features.Orders.Commands.Update;
using ZeusApp.Application.Features.Orders.Queries.GetAllPaged;
using ZeusApp.Application.Features.Orders.Queries.GetById;

namespace ZeusApp.WebApi.Controllers.v2;

[AllowAnonymous]
public class OrderController : BaseApiController<OrderController>
{
    //[HttpGet]
    //public async Task<IActionResult> GetAll(int pageNumber, int pageSize)
    //{
    //    var orders = await _mediator.Send(new GetAllOrdersQuery(pageNumber, pageSize));
    //    return Ok(orders);
    //}

    //[HttpGet("{id}")]
    //public async Task<IActionResult> GetById(int id)
    //{
    //    var Order = await _mediator.Send(new GetOrderByIdQuery { ıd = id });
    //    return Ok(Order);
    //}

    [HttpPost]
    public async Task<IActionResult> Post(CreateOrderCommand command)
    {
        var createResult = await _mediator.Send(command);
        return Ok(createResult);
    }

    //[HttpPut]
    //public async Task<IActionResult> Put(int id, UpdateOrderCommand command)
    //{

    //    var updateResult = await _mediator.Send(command);
    //    return Ok(updateResult);
    //}

    //[HttpDelete("{id}")]
    //public async Task<IActionResult> Delete(int id)
    //{
    //    var deleteResult = await _mediator.Send(new DeleteOrderCommand { Id = id });
    //    return Ok(deleteResult);
    //}


}
