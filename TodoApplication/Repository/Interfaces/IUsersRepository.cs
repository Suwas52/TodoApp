using TodoApplication.Entities;

namespace TodoApplication.Repository.Interfaces;

public interface IUsersRepository
{
    Task AddUserAsync(Users users, CancellationToken ct = default);
    Task UpdateUserAsync(Users users, CancellationToken ct); 
    Task DeleteUserAsync(Users users, CancellationToken ct);
    Task<Users?> GetUserByIdAsync(Guid id, CancellationToken ct);
    Task<List<Users>> GetAllUserAsync(CancellationToken ct);
    Task<Users?> GetUserByEmailAsync(string email, CancellationToken ct= default);
}