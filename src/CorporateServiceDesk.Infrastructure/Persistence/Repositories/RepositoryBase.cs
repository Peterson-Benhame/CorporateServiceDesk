using CorporateServiceDesk.Application.Common.Abstractions.Persistence;
using CorporateServiceDesk.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CorporateServiceDesk.Infrastructure.Persistence.Repositories
{
    public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        protected RepositoryBase(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<TEntity>();
        }

        protected ApplicationDbContext DbContext { get; }
        protected DbSet<TEntity> DbSet { get; }

        public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) => DbSet.AddAsync(entity, cancellationToken).AsTask();

        public Task<TEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) => DbSet.FindAsync(new object[] { id }, cancellationToken).AsTask();

        public void Update(TEntity entity) => DbSet.Update(entity);

        public void Remove(TEntity entity) => DbSet.Remove(entity);
    }
}
