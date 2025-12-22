using TodoApplication.Dto;
using TodoApplication.Entities;
using TodoApplication.Enum;
using TodoApplication.Identity;
using TodoApplication.Repository.Interfaces;
using TodoApplication.Services.Interfaces;

namespace TodoApplication.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;
    private readonly ISystemInfoFromCookie _cookieInfo;

    public TodoService(
        ITodoRepository todoRepository,
        ISystemInfoFromCookie cookieInfo)
    {
        _todoRepository = todoRepository;
        _cookieInfo = cookieInfo;
    }
    public async Task<Response> AddTodo(CreateTodoDto todoDto, CancellationToken ct)
    {
        var todo = new Todos
        {
            user_id = _cookieInfo.user_id,
            title =  todoDto.title,
            description = todoDto.description,
            status = todo_status.Pending,
            priority = todoDto.priority,
            is_deleted = false,
            created_at = DateTime.UtcNow,
            created_by = _cookieInfo.full_name,
            updated_at = DateTime.UtcNow,
            
        };

        await _todoRepository.AddTodoAsync(todo, ct);

        return new Response()
        {
            issucceed =  true,
            statusCode = 200,
            message = "Todo Added Successfully",
        };
    }

    public async Task<List<Todos>> GetAllTodos(CancellationToken ct)
    {
        if (_cookieInfo.IsSuperAdmin || _cookieInfo.IsManager)
        {
            return await _todoRepository.GetAllTodosAsync(ct);
        }
        return await _todoRepository.GetAllTodosByUser(_cookieInfo.user_id, ct);
        
    }

    public async Task<Todos?> GetTodoById(int id, CancellationToken ct)
    {
        return await _todoRepository.GetTodosByIdAsync(id, ct);
    }

    public async Task<Response> UpdateTodo(int id, CreateTodoDto dto, CancellationToken ct)
    {
        var todo = await _todoRepository.GetTodosByIdAsync(id, ct);
        if (todo == null)
            return new Response()
            {
                issucceed = false,
                statusCode = 404,
                message = "Todo not found"
            };

        todo.title = dto.title;
        todo.description = dto.description;
        todo.priority = dto.priority;
        todo.updated_at = DateTime.UtcNow;
        todo.updated_by = "suwas";

        await _todoRepository.UpdateTodoAsync(todo, ct);
        return new Response()
        {
            issucceed = false,
            statusCode = 200,
            message = "Todo update successfully"
        };
    }

    public async Task<Response> DeleteTodo(int id, CancellationToken ct)
    {
        var todo = await _todoRepository.GetTodosByIdAsync(id, ct);
        if (todo == null)
            return new Response()
            {
                issucceed = false,
                statusCode = 404,
                message = "Todo not found"
            };
        
        todo.is_deleted = true;
        todo.updated_at = DateTime.UtcNow;
        
        await _todoRepository.UpdateTodoAsync(todo, ct);
        return new Response()
        {
            issucceed = true,
            statusCode = 200,
            message = "Todo Deleted Successfully",
        };
    }
}