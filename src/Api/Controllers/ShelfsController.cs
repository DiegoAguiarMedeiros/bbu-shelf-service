using Api.Contracts;
using Application.DTOs;
using Application.Shelfs.Commands.CreateShelf;
using Application.Shelfs.Commands.DeleteShelf;
using Application.Shelfs.Queries.GetShelf;
using Application.Shelfs.Queries.GetShelfByUserId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShelfsController : ControllerBase
{
    private readonly ISender _sender;

    public ShelfsController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ShelfDto>>>> GetShelfByUserId( CancellationToken cancellationToken)
    {
        var subject = User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(subject, out var tokenUserId))
            return Unauthorized(ApiResponse<IEnumerable<ShelfDto>>.Fail("Invalid authenticated user ID."));

        var result = await _sender.Send(new GetShelfByUserIdQuery(tokenUserId), cancellationToken);
        if (result == null)
            return NotFound(ApiResponse<IEnumerable<ShelfDto>>.Fail($"Shelf with userId '{tokenUserId}' was not found."));

        return Ok(ApiResponse<IEnumerable<ShelfDto>>.Ok(result));
    }
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ShelfDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetShelfQuery(id), cancellationToken);
        if (result == null)
            return NotFound(ApiResponse<ShelfDto>.Fail($"Shelf with Id '{id}' was not found."));

        return Ok(ApiResponse<ShelfDto>.Ok(result));
    }


    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var success = await _sender.Send(new DeleteShelfCommand(id), cancellationToken);
        if (!success)
            return NotFound(ApiResponse<bool>.Fail($"Shelf with Id '{id}' was not found."));

        return Ok(ApiResponse<bool>.Ok(true, "Shelf deleted successfully."));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ShelfDto>>> RedeemBook(
    [FromBody] RedeemBookRequest request,
    CancellationToken cancellationToken)
    {
        var subject = User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(subject, out var tokenUserId))
            return Unauthorized(ApiResponse<ShelfDto>.Fail("Invalid authenticated user ID."));

        var command = new CreateShelfCommand(tokenUserId, request.BookId, request.CopyType.ToString(), request.AcquiredSource.ToString());
        var result = await _sender.Send(command, cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<ShelfDto>.Ok(result, "Shelf created successfully."));
    }
}
