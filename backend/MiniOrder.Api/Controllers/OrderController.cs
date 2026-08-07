using Microsoft.AspNetCore.Mvc;
using MiniOrder.Application.DTOs.Orders.Requests;
using MiniOrder.Application.Interfaces;

namespace MiniOrder.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(
        IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _orderService.CreateAsync(
            request,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
    int id,
    CancellationToken cancellationToken)
    {
        var result = await _orderService.GetByIdAsync(
            id,
            cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
    CancellationToken cancellationToken)
    {
        var result = await _orderService.GetAllAsync(
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}