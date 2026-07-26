namespace CorporateServiceDesk.Application.UnitTests.Support;

public sealed class TestTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => utcNow;
}
