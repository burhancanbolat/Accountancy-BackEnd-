﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AspNetCoreHero.Abstractions.Repository;

public abstract class Repository<T> : IRepository<T> where T : class
{
    private readonly ICommandRepository<T> _commandRepository;
    private readonly IQueryRepository<T> _queryRepository;

    protected Repository(ICommandRepository<T> commandRepository, IQueryRepository<T> queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public IQueryable<T> Queryable => _queryRepository.Queryable;

    #region Commands
    #region Add
    public void Add(T item)
    {
        _commandRepository.Add(item);
    }

    public Task AddAsync(T item)
    {
        return _commandRepository.AddAsync(item);
    }

    public void AddRange(IEnumerable<T> items)
    {
        _commandRepository.AddRange(items);
    }

    public Task AddRangeAsync(IEnumerable<T> items)
    {
        return _commandRepository.AddRangeAsync(items);
    }
    #endregion

    #region Delete
    public void Delete(object key)
    {
        _commandRepository.Delete(key);
    }

    public void Delete(Expression<Func<T, bool>> where)
    {
        _commandRepository.Delete(where);
    }

    public Task DeleteAsync(object key)
    {
        return _commandRepository.DeleteAsync(key);
    }

    public Task DeleteAsync(Expression<Func<T, bool>> where)
    {
        return _commandRepository.DeleteAsync(where);
    }
    #endregion

    #region Update
    public void Update(object key, T item)
    {
        _commandRepository.Update(key, item);
    }

    public Task UpdateAsync(object key, T item)
    {
        return _commandRepository.UpdateAsync(key, item);
    }

    public void UpdatePartial(object key, object item)
    {
        _commandRepository.UpdatePartial(key, item);
    }

    public Task UpdatePartialAsync(object key, object item)
    {
        return _commandRepository.UpdatePartialAsync(key, item);
    }

    public void UpdateRange(IEnumerable<T> items)
    {
        _commandRepository.UpdateRange(items);
    }

    public Task UpdateRangeAsync(IEnumerable<T> items)
    {
        return _commandRepository.UpdateRangeAsync(items);
    }
    #endregion
    #endregion

    #region Queries
    #region Any
    public bool Any()
    {
        return _queryRepository.Any();
    }

    public bool Any(Expression<Func<T, bool>> where)
    {
        return _queryRepository.Any(where);
    }

    public Task<bool> AnyAsync()
    {
        return _queryRepository.AnyAsync();
    }

    public Task<bool> AnyAsync(Expression<Func<T, bool>> where)
    {
        return _queryRepository.AnyAsync(where);
    }
    #endregion

    #region Count
    public long Count()
    {
        return _queryRepository.Count();
    }

    public long Count(Expression<Func<T, bool>> where)
    {
        return _queryRepository.Count(where);
    }

    public Task<long> CountAsync()
    {
        return _queryRepository.CountAsync();
    }

    public Task<long> CountAsync(Expression<Func<T, bool>> where)
    {
        return _queryRepository.CountAsync(where);
    }
    #endregion

    #region Get
    public T Get(object key)
    {
        return _queryRepository.Get(key);
    }

    public Task<T> GetAsync(object key)
    {
        return _queryRepository.GetAsync(key);
    }
    #endregion

    #region List
    public IEnumerable<T> List()
    {
        return _queryRepository.List();
    }

    public IEnumerable<T> List(Expression<Func<T, bool>> where)
    {
        return _queryRepository.List(where);
    }

    public Task<IEnumerable<T>> ListAsync()
    {
        return _queryRepository.ListAsync();
    }

    public Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> where)
    {
        return _queryRepository.ListAsync(where);
    }
    #endregion
    #endregion
}
