namespace CnabProcessor.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T> AddAsync(T entity);
}
