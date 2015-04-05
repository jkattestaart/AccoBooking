using System;
using System.Collections.Generic;
using Cocktail;
using DomainModel;
using IdeaBlade.EntityModel;

namespace DomainServices.Repositories
{
  public class BaseRepository<T> : Repository<T> where T : class 
  {
    public BaseRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public virtual IEntityQuery<T> GetQuery()
    {
      return EntityManager.GetQuery<T>();
    }



    ////////Override ivm NamedQuery
    //////protected override IEntityQuery<T> GetFindQuery(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, string includeProperties)
    //////{
    //////  IEntityQuery<T> query = GetQuery();            //EntityManager.ExpiredBookings;

    //////  if (predicate != null)
    //////    query = query.Where(predicate);
    //////  if (orderBy != null)
    //////    query = (IEntityQuery<T>)orderBy(query);
    //////  if (!string.IsNullOrWhiteSpace(includeProperties))
    //////    query = ParseIncludeProperties(includeProperties)
    //////        .Aggregate(query, (q, includeProperty) => q.Include(includeProperty));

    //////  return query.With(DefaultQueryStrategy);
    //////}

    ////////Override ivm NamedQuery
    //////protected override IEntityQuery<TResult> GetFindQuery<TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector, Expression<Func<T, bool>> predicate, Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy)
    //////{
    //////  IEntityQuery<T> baseQuery = GetQuery();            //EntityManager.ExpiredBookings;

    //////  if (predicate != null)
    //////    baseQuery = baseQuery.Where(predicate);

    //////  var query = (IEntityQuery<TResult>)selector(baseQuery);
    //////  if (orderBy != null)
    //////    query = (IEntityQuery<TResult>)orderBy(query);

    //////  return query.With(DefaultQueryStrategy);
    //////}

    ////////Override ivm NamedQuery
    //////protected override IEntityQuery GetFindQuery(Func<IQueryable<T>, IQueryable> selector, Expression<Func<T, bool>> predicate, Func<IQueryable, IOrderedQueryable> orderBy)
    //////{
    //////  IEntityQuery<T> baseQuery = GetQuery();            //EntityManager.ExpiredBookings;

    //////  if (predicate != null)
    //////    baseQuery = baseQuery.Where(predicate);

    //////  var query = selector(baseQuery);
    //////  if (orderBy != null)
    //////    query = orderBy(query);

    //////  return ((IEntityQuery)query).With(DefaultQueryStrategy);
    //////}

    private IEnumerable<string> ParseIncludeProperties(string includeProperties)
    {
      return includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    }
  }

}
