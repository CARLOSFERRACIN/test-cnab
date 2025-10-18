using CnabProcessor.Models.Entity;

namespace CnabProcessor.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IStoreRepository Stores { get; }
    ITransactionRepository Transactions { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
