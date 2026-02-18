using MedicineFinder.Application.Interfaces;
using MedicineFinder.Infrastructure.Data;
using MedicineFinder.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace MedicineFinder.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        if (_repositories.TryGetValue(type, out var repo))
        {
            return (IGenericRepository<T>)repo;
        }

        var repository = new GenericRepository<T>(_context);
        _repositories[type] = repository;
        return repository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        if (IsInMemoryProvider())
        {
            return;
        }

        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction == null)
        {
            return;
        }

        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync()
    {
        if (_transaction == null)
        {
            return;
        }

        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    private bool IsInMemoryProvider()
    {
        return _context.Database.ProviderName?.Contains("InMemory") == true;
    }
}
