using Microsoft.EntityFrameworkCore;
using TodoApplication.Data;
using TodoApplication.Dto;
using TodoApplication.Entities;
using TodoApplication.Enum;
using TodoApplication.Repository.Interfaces;

namespace TodoApplication.Repository;

public class TodosRepository : ITodoRepository
{
    private readonly TodoAppDbContext _context;
    
    public TodosRepository(TodoAppDbContext context)
    {
        _context = context;
    }
    public async Task AddTodoAsync(Todos todos, CancellationToken ct)
    {
        await _context.Todos.AddAsync(todos, ct);
        await _context.SaveChangesAsync(ct);
        //return _todoRepositoryImplementation.AddTodoAsync(todos, ct);
    }

    public async Task<List<Todos>> GetTodosForTodayReminderAsync(CancellationToken ct)
    {
        var today = DateTime.Today;
        return await _context.Todos
            .Where(t => 
                !t.is_deleted && 
                t.created_at.Date < today && 
                t.due_date.Date == today && 
                !t.is_send_reminder && 
                t.status == todo_status.Pending)
            .Include(t => t.createdTodoUser)
            .ToListAsync(ct);
    }

    public async Task<List<Todos>> GetExpiredTodosAsync(CancellationToken ct)
    {
        return await  _context.Todos
            .Where(t => !t.is_deleted && t.due_date.Day == DateTime.Now.Day && 
                        t.status == todo_status.Pending
                        )
            .ToListAsync(ct);
    }

    public async Task UpdateTodoAsync(Todos todos, CancellationToken ct)
    {
        _context.Todos.Update(todos);
        await _context.SaveChangesAsync(ct);
    }

    public Task DeleteTodoAsync(Todos todos, CancellationToken ct)
    {
        _context.Todos.Remove(todos);
        return _context.SaveChangesAsync(ct);
    }

    public async Task<Todos?> GetTodosByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Todos
            .Where(t => t.id == id && !t.is_deleted)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<Todos>> GetAllTodosAsync(CancellationToken ct)
    {
        return await _context.Todos
            .Where(t => !t.is_deleted)
            .ToListAsync(ct);
    }

    public async Task<List<Todos>> GetAllTodosByUser(Guid userId, CancellationToken ct)
    {
        return await _context.Todos
            .Where(t => t.user_id == userId && !t.is_deleted)
            .ToListAsync(ct);
    }

    public async Task<List<TodoListDto>> TodayTodoListByUserId(Guid userId, CancellationToken ct)
    {
        return await _context.Todos
            .Where(t => t.user_id == userId && !t.is_deleted && t.created_at.Day == DateTime.Now.Day)
            .Select(t => new TodoListDto
            {
                id = t.id,
                title = t.title,
                username = t.createdTodoUser.first_name + " " + t.createdTodoUser.last_name,
                created_at = t.created_at,
                status = t.status,
                priority = t.priority,
            })
            .ToListAsync(ct);
    }

    public async Task<List<TodoListDto>> TodayTodoLists(CancellationToken ct)
    {
        return await _context.Todos
            .Where(t => !t.is_deleted)
            .Select(t => new TodoListDto
            {
                id = t.id,
                title = t.title,
                username = t.createdTodoUser.first_name + " " + t.createdTodoUser.last_name,
                created_at = t.created_at,
                status = t.status,
                priority = t.priority,
            })
            .AsNoTracking()
            .ToListAsync(ct);
    }
    
    public async Task<List<Todos>> GetSameDayDueTodosForOneHourReminderAsync(
        DateTime now,
        CancellationToken ct)
    {
        var today = now.Date;

        return await _context.Todos
            .Where(t =>
                    !t.is_deleted &&
                    !t.is_send_reminder &&
                    t.status == todo_status.Pending &&
                    t.created_at.Date == today &&
                    t.due_date.Date == today &&
                    t.due_date > now // future only
            )
            .Include(t => t.createdTodoUser)
            .ToListAsync(ct);
    }

}