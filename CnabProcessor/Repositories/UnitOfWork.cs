using CnabProcessor.Repositories.Data;
using CnabProcessor.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics.CodeAnalysis;

namespace CnabProcessor.Repositories;

[ExcludeFromCodeCoverage]
public class UnitOfWork : IUnitOfWork
{
    private readonly CnabContext _context;
    private IDbContextTransaction? _transaction;
    private IStoreRepository? _stores;
    private ITransactionRepository? _transactions;

    public UnitOfWork(CnabContext context)
    {
        _context = context;
    }

    public IStoreRepository Stores => _stores ??= new StoreRepository(_context);
    public ITransactionRepository Transactions => _transactions ??= new TransactionRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
