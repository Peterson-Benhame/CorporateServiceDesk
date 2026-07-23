namespace CorporateServiceDesk.Domain.Common.Common
{
    public sealed class DomainException : Exception
    {
        public DomainException(string message)
            : base(message)
        {
        }
    }
}
