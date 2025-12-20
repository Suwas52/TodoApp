using Microsoft.EntityFrameworkCore;
using TodoApplication.Data;
using TodoApplication.Entities;
using TodoApplication.Repository.Interfaces;

namespace TodoApplication.Repository;

public class RolesRepository : IRolesRepository
{
    private readonly TodoAppDbContext _context;
    public RolesRepository(TodoAppDbContext context)
    {
        _context = context;
    }
    public async Task AddRoleAsync(Roles users)
    {
        await _context.Roles.AddAsync(users);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRoleAsync(Roles users, CancellationToken ct)
    {
        _context.Roles.Update(users);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteRoleAsync(Roles users, CancellationToken ct)
    {
        _context.Roles.Remove(users);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Roles?> GetRoleByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Roles
            .Where(t => t.role_id == id && !t.is_deleted)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Roles?> GetRoleByNameAsync(string roleName, CancellationToken ct)
    {
        return await _context.Roles
            .Where(t => t.role_name == roleName && !t.is_deleted)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<Roles>> GetAllRoleAsync(CancellationToken ct)
    {
        return await _context.Roles
            .Where(t => !t.is_deleted)
            .ToListAsync(ct);
    }

    public async Task<bool> RoleExistsAsync(string roleName )
    {
        return await _context.Roles.AnyAsync(t => t.role_name == roleName && !t.is_deleted);
    }
    
    public async Task<bool> RolesExistsAsync(List<string> roleNames, CancellationToken ct)
    {
        var count = await _context.Roles.CountAsync(r => roleNames.Contains(r.role_name) && !r.is_deleted, ct);
        return count == roleNames.Count;
    }
}