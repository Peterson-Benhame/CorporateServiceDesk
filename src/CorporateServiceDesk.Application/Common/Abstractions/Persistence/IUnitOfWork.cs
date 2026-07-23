namespace CorporateServiceDesk.Application.Common.Abstractions.Persistence
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
    }
}
