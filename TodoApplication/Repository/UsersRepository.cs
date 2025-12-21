using Microsoft.EntityFrameworkCore;
using TodoApplication.Data;
using TodoApplication.Dto;
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
            .Include(u => u.userroles)
            .ThenInclude(r => r.Role)
            .FirstOrDefaultAsync(u => u.user_id == id && !u.is_deleted ,ct);
    }

    public async Task<List<UserListDto>> GetAllUserAsync(CancellationToken ct)
    {
        return await _context.Users
            .Where(t => !t.is_deleted)
            .Select(u => new UserListDto
            {
                user_id = u.user_id,
                email = u.email,
                full_name = $"{u.first_name} {u.last_name}",
                email_confirmed = u.email_confirmed,
                is_active =  u.is_active,
            })
            .ToListAsync(ct);
    }

    public async Task<Users?> GetUserByEmailAsync(string email, CancellationToken ct )
    {
        
        return await _context.Users
            .Include(u => u.userroles)
            .ThenInclude(r => r.Role)
            .FirstOrDefaultAsync(u => u.email == email && !u.is_deleted ,ct);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
    {
        return await _context.Users.AnyAsync(u => u.email == email, ct);
    }

    public IQueryable<UserListDto> GetUsers()
    {
        return _context.Users.Where(u => !u.is_deleted)
            .Select(u => new UserListDto
            {
                user_id = u.user_id,
                email = u.email,
                full_name = $"{u.first_name} {u.last_name}",
                email_confirmed = u.email_confirmed,
                is_active =  u.is_active,
                is_blocked = u.is_blocked,
            });
    }
}