namespace CorporateServiceDesk.UnitTests.Domain
{
    public sealed class TestTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }

}
