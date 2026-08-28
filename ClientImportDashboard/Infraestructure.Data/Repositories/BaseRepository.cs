using Domain.Domain.Interfaces.Repositories;
using Domain.Entities.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infraestructure.Data.Repositories;

public class BaseRepository<T>: IBaseRepository<T> where T : class
{
    protected ApiDbContext _context;
    public BaseRepository(ApiDbContext context) => _context = context;    

    #region AsyncMethods

    /// <summary>
    /// Retrieves all entities of type T asynchronously.
    /// </summary>
    /// <returns>An enumerable collection of entities.</returns>
    public async Task<IEnumerable<T>> GetAllAsync() => await this._context.Set<T>().ToListAsync();

    /// <summary>
    /// Retrieves all entities of type T without tracking asynchronously.
    /// </summary>
    /// <returns>An enumerable collection of entities.</returns>
    public async Task<IEnumerable<T>> GetAllWithNoTrackingAsync() => await this._context.Set<T>().AsNoTracking().ToListAsync();

    /// <summary>
    /// Retrieves entities of type T based on given filters asynchronously.
    /// </summary>
    /// <param name="filters">The filter expression.</param>
    /// <returns>An enumerable collection of entities.</returns>
    public async Task<IEnumerable<T>> GetByFiltersAsync(Expression<Func<T, bool>> filters) => await this._context.Set<T>().Where(filters).ToListAsync();

    /// <summary>
    /// Retrieves entities of type T based on given filters asynchronously with includes.
    /// </summary>
    /// <param name="filters">The filter expression.</param>
    /// <param name="include">The include function.</param>
    /// <returns>An enumerable collection of entities.</returns>
    public async Task<IEnumerable<T>> GetByFiltersAsync(Expression<Func<T, bool>> filters, Func<IQueryable<T>, IQueryable<T>> include)
        => await include(this._context.Set<T>()).Where(filters).ToListAsync();

    /// <summary>
    /// Retrieves entities of type T based on given filters without tracking asynchronously.
    /// </summary>
    /// <param name="filters">The filter expression.</param>
    /// <returns>An enumerable collection of entities.</returns>
    public async Task<IEnumerable<T>> GetByFiltersWithNoTrackingAsync(Expression<Func<T, bool>> filters) => await this._context.Set<T>().AsNoTracking().Where(filters).ToListAsync();

    /// <summary>
    /// Retrieves entities of type T based on given filters without tracking asynchronously with includes.
    /// </summary>
    /// <param name="filters">The filter expression.</param>
    /// <param name="include">The include function.</param>
    /// <returns>An enumerable collection of entities.</returns>
    public async Task<IEnumerable<T>> GetByFiltersWithNoTrackingAsync(Expression<Func<T, bool>> filters, Func<IQueryable<T>, IQueryable<T>> include)
        => await include(this._context.Set<T>()).AsNoTracking().Where(filters).ToListAsync();

    /// <summary>
    /// Retrieves projected entities of type TResult based on given filters asynchronously.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected entity.</typeparam>
    /// <param name="filters">The filter expression.</param>
    /// <param name="selectors">The projection expression.</param>
    /// <returns>An enumerable collection of projected entities.</returns>
    public async Task<IEnumerable<TResult>> GetByFiltersAsync<TResult>(Expression<Func<T, bool>> filters, Expression<Func<T, TResult>> selectors) => await this._context.Set<T>().Where(filters).Select(selectors).ToListAsync();

    /// <summary>
    /// Retrieves projected entities of type TResult based on given filters without tracking asynchronously.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected entity.</typeparam>
    /// <param name="filters">The filter expression.</param>
    /// <param name="selectors">The projection expression.</param>
    /// <returns>An enumerable collection of projected entities.</returns>
    public async Task<IEnumerable<TResult>> GetByFiltersWithNoTrackingAsync<TResult>(Expression<Func<T, bool>> filters, Expression<Func<T, TResult>> selectors) => await this._context.Set<T>().AsNoTracking().Where(filters).Select(selectors).ToListAsync();

    /// <summary>
    /// Finds an entity of type T based on given filters asynchronously.
    /// </summary>
    /// <param name="filters">The filter expression.</param>
    /// <returns>The found entity or a default instance of T.</returns>
    public async Task<T> FindByFiltersAsync(Expression<Func<T, bool>> filters) => await this._context.Set<T>().FirstOrDefaultAsync(filters) ?? Activator.CreateInstance<T>();

    /// <summary>
    /// Finds an entity of type T based on given filters asynchronously with includes.
    /// </summary>
    /// <param name="filters">The filter expression.</param>
    /// <param name="include">The include function.</param>
    /// <returns>The found entity or a default instance of T.</returns>
    public async Task<T> FindByFiltersAsync(Expression<Func<T, bool>> filters, Func<IQueryable<T>, IQueryable<T>> include)
        => await include(this._context.Set<T>()).FirstOrDefaultAsync(filters) ?? Activator.CreateInstance<T>();

    /// <summary>
    /// Finds an entity of type T based on given filters without tracking asynchronously.
    /// </summary>
    /// <param name="filters">The filter expression.</param>
    /// <returns>The found entity or a default instance of T.</returns>
    public async Task<T> FindByFiltersWithNoTrackingAsync(Expression<Func<T, bool>> filters) => await this._context.Set<T>().AsNoTracking().FirstOrDefaultAsync(filters) ?? Activator.CreateInstance<T>();

    /// <summary>
    /// Finds an entity of type T based on given filters without tracking asynchronously with includes.
    /// </summary>
    /// <param name="filters">The filter expression.</param>
    /// <param name="include">The include function.</param>
    /// <returns>The found entity or a default instance of T.</returns>
    public async Task<T> FindByFiltersWithNoTrackingAsync(Expression<Func<T, bool>> filters, Func<IQueryable<T>, IQueryable<T>> include)
        => await include(this._context.Set<T>()).AsNoTracking().FirstOrDefaultAsync(filters) ?? Activator.CreateInstance<T>();

    /// <summary>
    /// Finds a projected entity of type TResult based on given filters asynchronously.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected entity.</typeparam>
    /// <param name="filters">The filter expression.</param>
    /// <param name="selectors">The projection expression.</param>
    /// <returns>The found projected entity or a default instance of TResult.</returns>
    public async Task<TResult> FindByFiltersAsync<TResult>(Expression<Func<T, bool>> filters, Expression<Func<T, TResult>> selectors) => await this._context.Set<T>().Where(filters).Select(selectors).FirstOrDefaultAsync() ?? Activator.CreateInstance<TResult>();

    /// <summary>
    /// Finds a projected entity of type TResult based on given filters without tracking asynchronously.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected entity.</typeparam>
    /// <param name="filters">The filter expression.</param>
    /// <param name="selectors">The projection expression.</param>
    /// <returns>The found projected entity or a default instance of TResult.</returns>
    public async Task<TResult> FindByFiltersWithNoTrackingAsync<TResult>(Expression<Func<T, bool>> filters, Expression<Func<T, TResult>> selectors) => await this._context.Set<T>().AsNoTracking().Where(filters).Select(selectors).FirstOrDefaultAsync() ?? Activator.CreateInstance<TResult>();

    /// <summary>
    /// Retrieves a value based on given filters asynchronously.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="filters">The filter expression.</param>
    /// <param name="selector">The value selector expression.</param>
    /// <returns>The retrieved value or a default value of TValue.</returns>
    public async Task<TValue?> GetValueByFiltersAsync<TValue>(Expression<Func<T, bool>> filters, Expression<Func<T, TValue>> selector) => await this._context.Set<T>().AsNoTracking().Where(filters).Select(selector).FirstOrDefaultAsync() ?? Activator.CreateInstance<TValue>();

    /// <summary>
    /// Retrieves the count of entities based on given filters asynchronously.
    /// </summary>
    /// <param name="filters">The filter expression.</param>
    /// <returns>The count of entities.</returns>
    public async Task<int> GetCountAsync(Expression<Func<T, bool>> filters) => await this._context.Set<T>().AsNoTracking().Where(filters).CountAsync();

    /// <summary>
    /// Retrieves the sum of values based on the given selector asynchronously.
    /// </summary>
    /// <param name="selector">The value selector expression.</param>
    /// <returns>The sum of values.</returns>
    public async Task<decimal> GetValueSumAsync(Expression<Func<T, decimal>> selector) => await this._context.Set<T>().AsNoTracking().SumAsync(selector);

    /// <summary>
    /// Retrieves the sum of values based on given filters and selector asynchronously.
    /// </summary>
    /// <param name="filters">The filter expression.</param>
    /// <param name="selector">The value selector expression.</param>
    /// <returns>The sum of values.</returns>
    public async Task<decimal> GetValueSumByFiltersAsync(Expression<Func<T, bool>> filters, Expression<Func<T, decimal>> selector)
    {
        var a = await this._context.Set<T>().AsNoTracking().Where(filters).ToListAsync();
        if (a.Count() == 0) return default(decimal);
        return a.Sum(selector.Compile());
    }

    /// <summary>
    /// Adds a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    public async Task AddAsync(T entity)
    {
        await this._context.Set<T>().AddAsync(entity);
        await this._context.SaveChangesAsync();
    }

    /// <summary>
    /// Adds a collection of entities asynchronously.
    /// </summary>
    /// <param name="entities">The collection of entities to add.</param>
    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await this._context.Set<T>().AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    public async Task UpdateAsync(T entity)
    {
        this._context.Entry(entity).State = EntityState.Modified;
        await this._context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates a collection of existing entities asynchronously.
    /// </summary>
    /// <param name="entities">The collection of entities to update.</param>
    public async Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        this._context.Set<T>().UpdateRange(entities);
        await this._context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes an existing entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    public async Task DeleteAsync(T entity)
    {
        this._context.Entry(entity).State = EntityState.Deleted;
        await this._context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a collection of existing entities asynchronously.
    /// </summary>
    /// <param name="entities">The collection of entities to delete.</param>
    public async Task DeleteRangeAsync(IEnumerable<T> entities)
    {
        this._context.Set<T>().RemoveRange(entities);
        await this._context.SaveChangesAsync();
    }

    #endregion
}