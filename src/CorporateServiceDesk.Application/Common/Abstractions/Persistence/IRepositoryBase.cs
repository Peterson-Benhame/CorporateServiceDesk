namespace CorporateServiceDesk.Application.Common.Abstractions.Persistence
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        Task<TEntity?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            TEntity entity,
            CancellationToken cancellationToken = default);

        void Update(TEntity entity);

        void Remove(TEntity entity);
    }
}
