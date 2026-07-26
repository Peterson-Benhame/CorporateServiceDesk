using CorporateServiceDesk.Api.Helpers;
using CorporateServiceDesk.Application.Common.Abstractions.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace CorporateServiceDesk.IntegrationTests.Api.Helpers;

public sealed class ApiResponseHandlerTests
{
    private readonly TestController _controller = new();

    [Fact]
    public void GenerateResponse_ShouldReturnOk()
    {
        var response = ApiResponseHandler.GenerateResponse(
            Result<string>.Success("ticket"), _controller);
        Assert.Equal("ticket", Assert.IsType<OkObjectResult>(response).Value);
    }

    [Fact]
    public void GenerateResponse_ShouldReturnConflict()
    {
        var response = ApiResponseHandler.GenerateResponse(
            Result<string>.Failure("Conflict.", EnumErrorType.Conflict), _controller);
        Assert.Equal("Conflict.", Assert.IsType<ConflictObjectResult>(response).Value);
    }

    private sealed class TestController : ControllerBase;
}
