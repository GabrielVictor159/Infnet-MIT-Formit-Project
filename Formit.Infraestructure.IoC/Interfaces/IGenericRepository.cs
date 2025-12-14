using Formit.Shared.DTOs;
using System.Linq.Expressions;

namespace Formit.Infraestructure.Interfaces;
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<PagedResultDto<T>> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? filter = null
    );
    Task<IEnumerable<T>> GetAllAsync();
    IQueryable<T> GetQueryable();
    Task<IEnumerable<T>> GetAsync(IQueryable<T> query, Expression<Func<T, bool>>? expression = null);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}
