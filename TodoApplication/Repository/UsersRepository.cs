using Microsoft.EntityFrameworkCore;
using TodoApplication.Data;
using TodoApplication.Entities;
using TodoApplication.Repository.Interfaces;

namespace TodoApplication.Repository;

public class UsersRepository : IUsersRepository
{
    private readonly TodoAppDbContext _context;
    public UsersRepository(TodoAppDbContext context)
    {
        _context = context;
    }
    public async Task AddUserAsync(Users users, CancellationToken ct =default)
    {
        await _context.Users.AddAsync(users, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateUserAsync(Users users, CancellationToken ct)
    {
        _context.Users.Update(users);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteUserAsync(Users users, CancellationToken ct)
    {
        _context.Users.Remove(users);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Users?> GetUserByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Users
            .Where(t => t.user_id == id && !t.is_deleted)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<Users>> GetAllUserAsync(CancellationToken ct)
    {
        return await _context.Users
            .Where(t => !t.is_deleted)
            .ToListAsync(ct);
    }

    public async Task<Users?> GetUserByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Users.Where(u => u.email == email && !u.is_deleted)
            .FirstOrDefaultAsync(ct);
    }
}