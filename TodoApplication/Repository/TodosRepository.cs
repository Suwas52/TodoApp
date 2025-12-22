using Microsoft.EntityFrameworkCore;
using TodoApplication.Data;
using TodoApplication.Entities;
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
}