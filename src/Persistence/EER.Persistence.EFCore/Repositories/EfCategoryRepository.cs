﻿using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
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

    public async Task<IEnumerable<Category>> GetAllAsync(ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(int id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.FindAsync([id], cancellationToken);
    }

    public async Task<Category> AddAsync(Category category, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Categories.AddAsync(category, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return entry.Entity;
    }

    public async Task<Category> UpdateAsync(Category category, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Categories.FindAsync([category.Id], cancellationToken);

        if (entity is null)
        {
            throw new InvalidOperationException($"Category with ID '{category.Id}' was not found.");
        }

        entity.Name = category.Name;
        entity.Description = category.Description;
        entity.Slug = category.Slug;
        entity.UpdatedBy = category.UpdatedBy;
        entity.UpdatedAt = category.UpdatedAt;

        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<bool> IsSlugExistsAsync(string slug, int? excludeCategoryId = null, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Where(c => c.Slug == slug);

        if (excludeCategoryId.HasValue)
        {
            query = query.Where(c => c.Id != excludeCategoryId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, ITransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Categories.FindAsync([id], cancellationToken);

        if (existing is null) return false;

        _context.Categories.Remove(existing);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}
