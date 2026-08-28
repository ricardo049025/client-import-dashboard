using System.Linq.Expressions;

namespace Domain.Domain.Interfaces.Repositories;

public interface IBaseRepository <T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllWithNoTrackingAsync();
    Task<IEnumerable<T>> GetByFiltersAsync(Expression<Func<T, bool>> filters);
    Task<IEnumerable<T>> GetByFiltersWithNoTrackingAsync(Expression<Func<T, bool>> filters);
    Task<IEnumerable<TResult>> GetByFiltersAsync<TResult>(Expression<Func<T, bool>> filters, Expression<Func<T, TResult>> selectors);
    Task<IEnumerable<TResult>> GetByFiltersWithNoTrackingAsync<TResult>(Expression<Func<T, bool>> filters, Expression<Func<T, TResult>> selectors);
    Task<T> FindByFiltersAsync(Expression<Func<T, bool>> filters);
    Task<TResult> FindByFiltersAsync<TResult>(Expression<Func<T, bool>> filters, Expression<Func<T, TResult>> selectors);
    Task<T> FindByFiltersWithNoTrackingAsync(Expression<Func<T, bool>> filters);
    Task<TResult> FindByFiltersWithNoTrackingAsync<TResult>(Expression<Func<T, bool>> filters, Expression<Func<T, TResult>> selectors);
    Task<TValue?> GetValueByFiltersAsync<TValue>(Expression<Func<T, bool>> filters, Expression<Func<T, TValue>> selector);
    Task<decimal> GetValueSumAsync(Expression<Func<T, decimal>> selector);
    Task<decimal> GetValueSumByFiltersAsync(Expression<Func<T, bool>> filters, Expression<Func<T, decimal>> selector);
    Task<int> GetCountAsync(Expression<Func<T, bool>> filters);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task UpdateRangeAsync(IEnumerable<T> entities);
    Task DeleteAsync(T entity);
    Task DeleteRangeAsync(IEnumerable<T> entities);
}
