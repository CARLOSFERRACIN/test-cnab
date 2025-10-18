using CnabProcessor.Repositories.Data;
using CnabProcessor.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace CnabProcessor.Repositories;

[ExcludeFromCodeCoverage]
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly CnabContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(CnabContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }
}
