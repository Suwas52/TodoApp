using TodoApplication.Entities;

namespace TodoApplication.Repository.Interfaces;

public interface ITodoRepository
{
    Task AddTodoAsync(Todos todos, CancellationToken ct);
    Task UpdateTodoAsync(Todos todos, CancellationToken ct); 
    Task DeleteTodoAsync(Todos todos, CancellationToken ct);
    Task<Todos?> GetTodosByIdAsync(int id, CancellationToken ct);
    Task<List<Todos>> GetAllTodosAsync(CancellationToken ct);
    Task<List<Todos>> GetAllTodosByUser(Guid userId, CancellationToken ct);
}