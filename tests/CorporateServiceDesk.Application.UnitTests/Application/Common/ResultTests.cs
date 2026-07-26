using CorporateServiceDesk.Application.Common.Abstractions.Notifications;

namespace CorporateServiceDesk.Application.UnitTests.Application.Common;

public sealed class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulOkResultByDefault()
    {
        var result = Result<string>.Success("ticket");
        Assert.True(result.IsSuccess);
        Assert.Equal("ticket", result.Value);
        Assert.Equal(EnumErrorType.OK, result.ErrorType);
    }

    [Fact]
    public void Failure_ShouldCreateFailureWithoutValue()
    {
        var result = Result<string>.Failure("Not found.", EnumErrorType.NotFound);
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal(EnumErrorType.NotFound, result.ErrorType);
    }
}
