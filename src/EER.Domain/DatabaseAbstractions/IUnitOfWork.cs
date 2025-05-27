namespace EER.Domain.DatabaseAbstractions;

// Can be done with IUnitOfWorkCreator, because of _transaction in it every time etc

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IUserRepository UserRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IOfficeRepository OfficeRepository { get; }
    IRentalRepository RentalRepository { get; }

    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
