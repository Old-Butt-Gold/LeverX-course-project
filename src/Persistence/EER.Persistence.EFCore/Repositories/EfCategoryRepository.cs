using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EER.Persistence.EFCore.Repositories;

// If need can inject interface of other repositories
internal sealed class EfCategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public EfCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.FindAsync([id], cancellationToken);
    }

    public async Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Categories.AddAsync(category, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return entry.Entity;
    }

    public async Task<Category?> UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Categories.FindAsync([category.Id], cancellationToken);
        if (existing is null) return null;

        existing.Name = category.Name;
        existing.Description = category.Description;
        existing.Slug = category.Slug;
        existing.UpdatedBy = category.UpdatedBy;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Categories.FindAsync([id], cancellationToken);

        if (existing is null) return false;

        _context.Categories.Remove(existing);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}
