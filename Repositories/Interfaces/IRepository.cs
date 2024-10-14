using System.Linq.Expressions;
using SP_000.Helpers;

namespace SP_000.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<(IEnumerable<T> data, int totalRecords, int totalPages)> GetAll(
            BaseQuery? query = null,
            List<string>? propsInclude = null
        );
        Task<(IEnumerable<T> data, int totalRecords, int totalPages)> GetAllSearched(
            BaseQuery? query = null,
            SearchQuery? searchQuery = null,
            List<string>? propsInclude = null
        );
        Task<T?> Get(Expression<Func<T, bool>> filter, List<string>? propsInclude = null);
        Task Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}