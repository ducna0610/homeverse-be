namespace Homeverse.Domain.Interfaces;

public interface IUnitOfWork
{
    int SaveChanges();
    Task<int> SaveChangesAsync();
    void BeginTransaction();
    void Commit();
    Task CommitAsync();
    void Rollback();
    Task ExecuteTransactionAsync(Action action);
    Task ExecuteTransactionAsync(Func<Task> action);
}