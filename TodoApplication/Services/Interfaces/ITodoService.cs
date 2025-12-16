using TodoApplication.Dto;
using TodoApplication.Entities;

namespace TodoApplication.Services.Interfaces;

public interface ITodoService
{
    Task<Response> AddTodo(CreateTodoDto  todoDto, CancellationToken ct);
    Task<List<Todos>> GetAllTodos(CancellationToken ct);
    Task<Todos?> GetTodoById(int id, CancellationToken ct);
    Task<Response> DeleteTodo(int id, CancellationToken ct);
    Task<Response> UpdateTodo(int id, CreateTodoDto dto, CancellationToken ct);
}