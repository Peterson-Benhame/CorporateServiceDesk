using CorporateServiceDesk.Api.Helpers;
using CorporateServiceDesk.Domain.Common.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace CorporateServiceDesk.IntegrationTests.Api.Helpers;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_ShouldReturnBadRequest_ForDomainException()
    {
        var service = new Mock<IProblemDetailsService>();
        service.Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(true);
        var environment = new Mock<IWebHostEnvironment>();
        environment.SetupGet(x => x.EnvironmentName).Returns("Production");
        var handler = new GlobalExceptionHandler(
            Mock.Of<ILogger<GlobalExceptionHandler>>(), service.Object, environment.Object);
        var context = new DefaultHttpContext();

        var handled = await handler.TryHandleAsync(
            context, new DomainException("Invalid."), CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }
}
