namespace EER.Domain.DatabaseAbstractions;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IUserRepository UserRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IOfficeRepository OfficeRepository { get; }

    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
