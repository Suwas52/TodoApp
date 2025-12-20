using Microsoft.EntityFrameworkCore.Storage;
using TodoApplication.Data;
using TodoApplication.Repository.Interfaces;

namespace TodoApplication.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly TodoAppDbContext _context;
    private IDbContextTransaction _transaction;
    
    public UnitOfWork(TodoAppDbContext context)
    {
        _context = context;
    }
    public async Task BeginTransactionAsync(CancellationToken ct)
    {
        _transaction = await _context.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitTransactionAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
        if (_transaction != null)
        {
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }
}