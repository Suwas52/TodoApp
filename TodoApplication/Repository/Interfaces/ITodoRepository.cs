using TodoApplication.Dto;
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
    Task<List<TodoListDto>> TodayTodoListByUserId(Guid userId, CancellationToken ct);
    Task<List<TodoListDto>> TodayTodoLists(CancellationToken ct);
    Task<List<Todos>> GetExpiredTodosAsync(CancellationToken ct);
    Task<List<Todos>> GetTodosForTodayReminderAsync(CancellationToken ct);

    Task<List<Todos>> GetSameDayDueTodosForOneHourReminderAsync(
        DateTime now,
        CancellationToken ct);
}