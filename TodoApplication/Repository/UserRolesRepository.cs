using Microsoft.EntityFrameworkCore;
using TodoApplication.Data;
using TodoApplication.Dto;
using TodoApplication.Entities;
using TodoApplication.Repository.Interfaces;

namespace TodoApplication.Repository;

public class UserRolesRepository : IUserRolesRepository
{
    private readonly TodoAppDbContext _context;

    public UserRolesRepository(TodoAppDbContext context)
    {
        _context = context;
    }
    public async Task AddUserRolesAsync(UserRoles userRoles, CancellationToken ct)
    {
        await _context.UserRoles.AddAsync(userRoles, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Response> AddToRoles(Users user, List<string> roleNames, CancellationToken ct)
    {
        var roles = await _context.Roles.Where(r => roleNames.Contains(r.role_name) && !r.is_deleted).ToListAsync(ct);
        if (roles == null)
            return new Response()
            {
                issucceed = false,
                statusCode = StatusCodes.Status404NotFound,
                message = "One or more Role not found",
            };

        List<UserRoles> userRole = new List<UserRoles>();
        foreach (var role in roles)
        {
            var userrole = new UserRoles()
            {
                user_id = user.user_id,
                role_id = role.role_id,
            };
            
            userRole.Add(userrole);
        }
        _context.UserRoles.AddRange(userRole);
        await _context.SaveChangesAsync(ct);
        return new Response()
        {
            issucceed = true,
            statusCode = StatusCodes.Status201Created,
            message = "Roles add successful",
        };
        
    }

    public async Task<Response> AddToRole(Users user, string RoleName, CancellationToken ct = default)
    {
        var role = await _context.Roles
            .Where(r => r.role_name == RoleName && r.is_deleted == false)
            .FirstOrDefaultAsync(ct);
        if (role == null)
        {
            return new Response()
            {
                issucceed = false,
                statusCode = StatusCodes.Status404NotFound,
                message = "Role not found",
            };
        }

        var userrole = new UserRoles()
        {
            user_id = user.user_id,
            role_id = role.role_id,
        };
        _context.UserRoles.Add(userrole);
        await _context.SaveChangesAsync(ct);
        return new Response()
        {
            issucceed = true,
            statusCode = StatusCodes.Status201Created,
            message = "Role created",
        };
    }

    public async Task UpdateUserRoleAsync(UserRoles userRoles, CancellationToken ct)
    {
        _context.UserRoles.Update(userRoles);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteUserRolesAsync(UserRoles userRoles, CancellationToken ct)
    {
        _context.UserRoles.Remove(userRoles);
        await _context.SaveChangesAsync(ct);
    }

    // public async Task<UserRoles?> GetUserRoleByIdAsync(int id, CancellationToken ct)
    // {
    //     return await _context.UserRoles
    //         .Where(t => t.user_id == id && !t.is_deleted)
    //         .FirstOrDefaultAsync(ct);
    // }
    //
    // public async Task<List<UserRoles>> GetAllUserRoleAsync(CancellationToken ct)
    // {
    //     return await _context.UserRoles
    //         .Where(t => !t.is_deleted)
    //         .ToListAsync(ct);
    // }
}