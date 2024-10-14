using SP_000.Data;
using SP_000.Repositories.Interfaces;

namespace SP_000.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        /*** Properties ***/
        private readonly AppDbContext _db;

        /*** Repositories(Properties) ***/
        public IEmployeeRepo repoEmployee { get; private set; }

        /*** Constructor ***/
        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            repoEmployee = new EmployeeRepo(_db);
        }

        /*** Methods ***/
        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}