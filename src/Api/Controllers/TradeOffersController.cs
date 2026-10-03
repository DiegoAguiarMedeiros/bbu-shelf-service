using Api.Contracts;
using Application.DTOs;
using Application.TradeOffers.Commands.CreateTradeOffer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TradeOffersController : ControllerBase
{
    private readonly ISender _sender;

    public TradeOffersController(ISender sender)
    {
        _sender = sender;
    }

    
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TradeOfferDto>>> CreateTradeOffer(
    [FromBody] CreateTradeOfferRequest request,
    CancellationToken cancellationToken)
    {
        var subject = User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(subject, out var tokenUserId))
            return Unauthorized(ApiResponse<TradeOfferDto>.Fail("Invalid authenticated user ID."));

        var command = new CreateTradeOfferCommand(tokenUserId, request.ToUserId, request.OfferedShelfIds, request.RequestedShelfIds);
        var result = await _sender.Send(command, cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<TradeOfferDto>.Ok(result, "Trade offer created successfully."));
    }
}
