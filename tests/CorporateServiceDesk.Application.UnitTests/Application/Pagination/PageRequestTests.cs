using CorporateServiceDesk.Application.Common.Pagination;

namespace CorporateServiceDesk.Application.UnitTests.Application.Pagination;

public sealed class PageRequestTests
{
    [Theory]
    [InlineData(0, 0, 1, 10)]
    [InlineData(-1, -5, 1, 10)]
    [InlineData(2, 200, 2, 100)]
    public void Normalize_ShouldApplyLimits(
        int page, int pageSize, int expectedPage, int expectedPageSize)
    {
        var result = new PageRequest(page, pageSize).Normalize();
        Assert.Equal(expectedPage, result.Page);
        Assert.Equal(expectedPageSize, result.PageSize);
    }
}
