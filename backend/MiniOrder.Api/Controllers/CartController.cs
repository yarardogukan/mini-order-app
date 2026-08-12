using Microsoft.AspNetCore.Mvc;
using MiniOrder.Application.Common.Errors;
using MiniOrder.Application.DTOs.Carts.Requests;
using MiniOrder.Application.Interfaces;

namespace MiniOrder.Api.Controllers;

[ApiController]
[Route("api/cart")]
public sealed class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("{cartId:guid}")]
    public async Task<IActionResult> GetCart(Guid cartId, CancellationToken cancellationToken)
    {
        var result = await _cartService.GetCartAsync(cartId, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(
        [FromQuery] Guid? cartId,
        [FromBody] AddCartItemRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await _cartService.AddItemAsync(cartId, request, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut("{cartId:guid}/items/{productId:int}")]
    public async Task<IActionResult> UpdateItemQuantity(
        Guid cartId,
        int productId,
        [FromBody] UpdateCartItemRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await _cartService.UpdateItemQuantityAsync(
            cartId,
            productId,
            request,
            cancellationToken
        );

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{cartId:guid}/items/{productId:int}")]
    public async Task<IActionResult> RemoveItem(
        Guid cartId,
        int productId,
        CancellationToken cancellationToken
    )
    {
        var result = await _cartService.RemoveItemAsync(cartId, productId, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{cartId:guid}")]
    public async Task<IActionResult> ClearCart(Guid cartId, CancellationToken cancellationToken)
    {
        var result = await _cartService.ClearAsync(cartId, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        return NoContent();
    }

    #region Helpers

    private IActionResult HandleFailure(Error error)
    {
        return error.Code switch
        {
            "Cart.NotFound" => NotFound(error),
            "Cart.ItemNotFound" => NotFound(error),
            "Cart.ProductNotFound" => NotFound(error),

            _ => BadRequest(error),
        };
    }
    #endregion
}
