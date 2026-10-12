using Microsoft.EntityFrameworkCore;
using ShopSphere.Api.Data;

namespace ShopSphere.Api.Repositories;

// Generic Repository pattern
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    IQueryable<T> Query();
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<int> SaveChangesAsync();
}

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _db;
    protected readonly DbSet<T> _set;

    public Repository(AppDbContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) => await _set.FindAsync(id);
    public async Task<List<T>> GetAllAsync() => await _set.AsNoTracking().ToListAsync();
    public IQueryable<T> Query() => _set.AsQueryable();
    public async Task AddAsync(T entity) => await _set.AddAsync(entity);
    public void Update(T entity) => _set.Update(entity);
    public void Remove(T entity) => _set.Remove(entity);
    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
}
