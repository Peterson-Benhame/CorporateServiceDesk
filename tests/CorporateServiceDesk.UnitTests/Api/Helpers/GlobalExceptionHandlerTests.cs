using CorporateServiceDesk.Api.Helpers;
using CorporateServiceDesk.Domain.Common.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CorporateServiceDesk.UnitTests.Api.Helpers;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_ShouldReturnBadRequest_ForDomainException()
    {
        ProblemDetailsContext? writtenContext = null;
        var problemDetailsService = new Mock<IProblemDetailsService>();
        problemDetailsService
            .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(context => writtenContext = context)
            .ReturnsAsync(true);

        var environment = new Mock<IWebHostEnvironment>();
        environment.SetupGet(x => x.EnvironmentName).Returns("Production");

        var handler = new GlobalExceptionHandler(
            Mock.Of<ILogger<GlobalExceptionHandler>>(),
            problemDetailsService.Object,
            environment.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/api/tickets";

        var handled = await handler.TryHandleAsync(
            httpContext,
            new DomainException("Invalid transition."),
            CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
        Assert.NotNull(writtenContext);
        Assert.Equal(StatusCodes.Status400BadRequest, writtenContext.ProblemDetails.Status);
        Assert.Equal("Regra de negócio inválida", writtenContext.ProblemDetails.Title);
        Assert.Equal("Invalid transition.", writtenContext.ProblemDetails.Detail);
        Assert.Equal("/api/tickets", writtenContext.ProblemDetails.Instance);
        Assert.True(writtenContext.ProblemDetails.Extensions.ContainsKey("traceId"));
    }
}
