using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using SP_000.Helpers;
using SP_000.Repositories.Interfaces;
using SP_000.Data;

namespace SP_000.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        /*** Properties ***/
        private readonly AppDbContext _db;
        internal DbSet<T> _dbSet;

        /*** Constructor ***/
        public Repository(AppDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }

        /*** Methods ***/
        public async Task<(IEnumerable<T> data, int totalRecords, int totalPages)> GetAll(
            BaseQuery? query = null,
            List<string>? propsInclude = null
        )
        {
            IEnumerable<T> result = Enumerable.Empty<T>(); ;
            IQueryable<T> queryableData = _dbSet;

            /** Eager Loading **/
            if (propsInclude != null && propsInclude.Count != 0)
            {
                foreach (string prop in propsInclude)
                {
                    queryableData = queryableData.Include(prop);
                }
            }

            result = await queryableData.ToListAsync();

            if (query == null)
            {
                int _pageSize = 20;
                int _totalRecords = result.Count();
                int _totalPages = (int)Math.Ceiling((double)_totalRecords / _pageSize);
                return (result, _totalRecords, _totalPages);
            }

            Sorting(ref result, query);
            var (totalRecords, totalPages) = Pagination(ref result, query);

            return (result, totalRecords, totalPages);
        }

        public async Task<(IEnumerable<T> data, int totalRecords, int totalPages)> GetAllSearched(
            BaseQuery? query = null,
            SearchQuery? searchQuery = null,
            List<string>? propsInclude = null
        )
        {
            IEnumerable<T> result = Enumerable.Empty<T>(); ;
            IQueryable<T> queryableData = _dbSet;

            /** Eager Loading **/
            if (propsInclude != null && propsInclude.Count != 0)
            {
                foreach (string prop in propsInclude)
                {
                    queryableData = queryableData.Include(prop);
                }
            }

            result = await queryableData.ToListAsync();

            Searching(ref result, searchQuery);

            if (query == null)
            {
                int _pageSize = 20;
                int _totalRecords = result.Count();
                int _totalPages = (int)Math.Ceiling((double)_totalRecords / _pageSize);
                return (result, _totalRecords, _totalPages);
            }

            Sorting(ref result, query);
            var (totalRecords, totalPages) = Pagination(ref result, query);

            return (result, totalRecords, totalPages);
        }

        public async Task<T?> Get(Expression<Func<T, bool>> filter, List<string>? propsInclude = null)
        {
            // IEnumerable is typically used for querying in-memory collections, such as arrays, lists, or collections.
            // IQueryable is used for querying data from external data sources like databases, web services, or other data providers.
            IQueryable<T> data = _dbSet;
            if (propsInclude != null && propsInclude.Count != 0)
            {
                foreach (string prop in propsInclude)
                {
                    /* Eager Loading */
                    data = data.Include(prop);
                }
            }

            return await data.FirstOrDefaultAsync(filter);
        }

        public async Task Add(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        /*** Private Methods ***/
        private (int totalRecords, int totalPages) Pagination(
            ref IEnumerable<T> result, BaseQuery? baseQuery
        )
        {
            int totalRecords = 0;
            int totalPages = 0;

            if (baseQuery != null)
            {
                int pageSize = Math.Min(50, baseQuery.PageSize);

                totalRecords = result.Count();
                totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                /* Clamped Page Value */
                int page = Math.Max(1, Math.Min(baseQuery.Page, totalPages));

                result = result.Skip((page - 1) * pageSize).Take(pageSize);
            }
            return (totalRecords, totalPages);
        }

        private void Sorting(ref IEnumerable<T> result, BaseQuery? baseQuery)
        {
            if (baseQuery != null && !string.IsNullOrWhiteSpace(baseQuery.SortBy))
            {
                if (baseQuery.SortOrder == "desc")
                {
                    result = result
                        .AsQueryable()
                        .OrderByDescending(e => GetQueryProperty(e, baseQuery.SortBy));
                }
                else
                {
                    result = result
                        .AsQueryable()
                        .OrderBy(e => GetQueryProperty(e, baseQuery.SortBy));
                }
            }
        }

        private void Searching(ref IEnumerable<T> result, SearchQuery? searchQuery)
        {
            if (searchQuery != null)
            {
                foreach (var item in GetAllQueryProperties(searchQuery))
                {
                    if (item.Value != null)
                    {
                        string propName = item.Key;
                        string propValue = ((string)item.Value).ToLower();
                        result = result.Where(
                            e => e.GetType().GetProperty(propName)?.GetValue(e)?
                                .ToString()?.ToLower()
                                .Contains(propValue)
                                ?? false
                            );
                    }
                }
            }
        }

        private object? GetQueryProperty(object obj, string propName)
        {
            if (obj == null) return null;

            PropertyInfo[] properties = obj.GetType().GetProperties();
            var propInfo = properties.FirstOrDefault(
                p => string.Equals(
                    p.Name, propName,
                    StringComparison.OrdinalIgnoreCase)
            );
            if (propInfo == null) return null;

            return propInfo.GetValue(obj);
        }

        private Dictionary<string, object> GetAllQueryProperties(object obj)
        {
            Dictionary<string, object> propertyDictionary = new Dictionary<string, object>();

            if (obj == null) return propertyDictionary;

            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property != null)
                {
                    object? value = property.GetValue(obj);
                    if (value != null)
                    {
                        propertyDictionary[property.Name] = value;
                    }
                }
            }

            return propertyDictionary;
        }
    }
}