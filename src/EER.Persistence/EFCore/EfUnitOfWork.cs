using System.Data;
using EER.Domain.DatabaseAbstractions;
using EER.Persistence.EFCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EER.Persistence.EFCore;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private readonly Lazy<IUserRepository> _users;
    private readonly Lazy<ICategoryRepository> _categories;
    private readonly Lazy<IOfficeRepository> _offices;
    private readonly Lazy<IRentalRepository> _rentals;
    private readonly Lazy<IEquipmentRepository> _equipment;
    private readonly Lazy<IEquipmentItemRepository> _equipmentItems;

    public EfUnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        _users = new Lazy<IUserRepository>(() =>
            new EfUserRepository(context));

        _categories = new Lazy<ICategoryRepository>(() =>
            new EfCategoryRepository(context));

        _offices = new Lazy<IOfficeRepository>(() =>
            new EfOfficeRepository(context));

        _rentals = new Lazy<IRentalRepository>(() =>
            new EfRentalRepository(context));

        _equipment = new Lazy<IEquipmentRepository>(() =>
            new EfEquipmentRepository(context));

        _equipmentItems = new Lazy<IEquipmentItemRepository>(() =>
            new EfEquipmentItemRepository(context));
    }

    public IUserRepository UserRepository => _users.Value;
    public ICategoryRepository CategoryRepository => _categories.Value;
    public IOfficeRepository OfficeRepository => _offices.Value;
    public IRentalRepository RentalRepository => _rentals.Value;
    public IEquipmentRepository EquipmentRepository => _equipment.Value;
    public IEquipmentItemRepository EquipmentItemRepository => _equipmentItems.Value;

    public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        if (_transaction != null) return;

        _transaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken: cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("Transaction not started");

        await _context.SaveChangesAsync(cancellationToken);
        await _transaction.CommitAsync(cancellationToken);
        await DisposeAsync();
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null) return;

        await _transaction.RollbackAsync(cancellationToken);
        await DisposeAsync();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _transaction?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        if (_transaction is not null)
            await _transaction.DisposeAsync();
        _transaction = null;
    }
}
