namespace MedicineFinder.Application.Interfaces;

public interface IUnitOfWork
{
    IGenericRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
