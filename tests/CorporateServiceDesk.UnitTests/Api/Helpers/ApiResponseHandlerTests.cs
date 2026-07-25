using CorporateServiceDesk.Api.Helpers;
using CorporateServiceDesk.Application.Common.Abstractions.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace CorporateServiceDesk.UnitTests.Api.Helpers;

public sealed class ApiResponseHandlerTests
{
    private readonly TestController _controller = new();

    [Fact]
    public void GenerateResponse_ShouldReturnOk_ForSuccessfulOkResult()
    {
        var result = Result<string>.Success("ticket");

        var response = ApiResponseHandler.GenerateResponse(result, _controller);

        var ok = Assert.IsType<OkObjectResult>(response);
        Assert.Equal("ticket", ok.Value);
    }

    [Fact]
    public void GenerateResponse_ShouldReturnCreatedAtAction_ForCreatedResult()
    {
        var result = Result<string>.Success(
            "ticket",
            EnumErrorType.Created);

        var response = ApiResponseHandler.GenerateResponse(
            result,
            _controller,
            "GetById",
            new { id = Guid.NewGuid() });

        var created = Assert.IsType<CreatedAtActionResult>(response);
        Assert.Equal("GetById", created.ActionName);
        Assert.Equal("ticket", created.Value);
    }

    [Fact]
    public void GenerateResponse_ShouldReturnConflict_ForConflictFailure()
    {
        var result = Result<string>.Failure(
            "Ticket already exists.",
            EnumErrorType.Conflict);

        var response = ApiResponseHandler.GenerateResponse(result, _controller);

        var conflict = Assert.IsType<ConflictObjectResult>(response);
        Assert.Equal("Ticket already exists.", conflict.Value);
    }

    private sealed class TestController : ControllerBase
    {
    }
}
