namespace SP_000.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        /* Repositories(Properties) */
        IEmployeeRepo repoEmployee { get; }

        /* Methods */
        Task Save();
    }
}