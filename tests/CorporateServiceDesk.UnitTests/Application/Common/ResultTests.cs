using CorporateServiceDesk.Application.Common.Abstractions.Notifications;

namespace CorporateServiceDesk.UnitTests.Application.Common;

public sealed class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulOkResultByDefault()
    {
        var result = Result<string>.Success("ticket");

        Assert.True(result.IsSuccess);
        Assert.Equal("ticket", result.Value);
        Assert.Equal(EnumErrorType.OK, result.ErrorType);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Success_ShouldRejectFailureStatus()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Result<string>.Success("ticket", EnumErrorType.Conflict));
    }

    [Fact]
    public void Failure_ShouldCreateFailureWithoutValue()
    {
        var result = Result<string>.Failure(
            "Ticket was not found.",
            EnumErrorType.NotFound);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal("Ticket was not found.", result.Error);
        Assert.Equal(EnumErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public void Map_ShouldPreserveCreatedStatus()
    {
        var result = Result<int>.Success(10, EnumErrorType.Created);

        var mapped = result.Map(value => value.ToString());

        Assert.True(mapped.IsSuccess);
        Assert.Equal("10", mapped.Value);
        Assert.Equal(EnumErrorType.Created, mapped.ErrorType);
    }

    [Fact]
    public void Map_ShouldPreserveFailureWithoutCallingMapper()
    {
        var mapperCalled = false;
        var result = Result<int>.Failure("Conflict.", EnumErrorType.Conflict);

        var mapped = result.Map(value =>
        {
            mapperCalled = true;
            return value.ToString();
        });

        Assert.False(mapped.IsSuccess);
        Assert.False(mapperCalled);
        Assert.Equal("Conflict.", mapped.Error);
        Assert.Equal(EnumErrorType.Conflict, mapped.ErrorType);
    }
}
