using TodoApplication.Data;
using TodoApplication.Dto;
using TodoApplication.Enum;
using TodoApplication.Repository.Interfaces;

namespace TodoApplication.Repository;

public class DashbordCardRepo : IDashbordCardRepo
{
    private readonly TodoAppDbContext _context;
    public DashbordCardRepo(TodoAppDbContext context)
    {
        _context = context;
    }
    public async Task<DashboardCardDto> AdminDashboardCard(CancellationToken ct =default)
    {
        var dashboard = new DashboardCardDto()
        {
            total_user_counts = _context.Users.Where(u => !u.is_deleted).Count(),
            verified_user_counts = _context.Users.Where(u => u.email_confirmed && !u.is_deleted && !u.is_blocked).Count(),
            total_todo_counts = _context.Todos.Where(u => !u.is_deleted).Count(),
            high_priority_todo_counts = _context.Todos.Where(c => c.priority == todo_priority.High && !c.is_deleted).Count(),
            completed_todo_counts = _context.Todos.Where(c => c.status == todo_status.Completed && !c.is_deleted).Count(),
            expired_todo_counts = _context.Todos.Where(c => c.status == todo_status.Completed && !c.is_deleted).Count(),
            blocked_user_counts = _context.Users.Where(u => !u.is_deleted && u.is_blocked).Count(),
            pending_todo_counts = _context.Todos.Where(t => !t.is_deleted && t.status == todo_status.Pending).Count(),
            urgent_todo_counts = _context.Todos.Where(t => !t.is_deleted && t.priority == todo_priority.Urgent).Count()
        };
        return dashboard;
    }
    
    public async Task<DashboardCardDto> UserDashboardCard(Guid user_id, CancellationToken ct =default)
    {
        var dashboard = new DashboardCardDto()
        {
            total_todo_counts = _context.Todos.Where(t => t.user_id == user_id && !t.is_deleted).Count(),
            high_priority_todo_counts =
                _context.Todos.Where(t => t.user_id == user_id && t.priority == todo_priority.High && !t.is_deleted).Count(),
            completed_todo_counts =
                _context.Todos.Where(t =>t.user_id == user_id && t.status == todo_status.Completed && !t.is_deleted).Count(),
            expired_todo_counts = _context.Todos.Where(t =>t.user_id == user_id && t.status == todo_status.Completed && !t.is_deleted).Count(),
            pending_todo_counts = _context.Todos.Where(t =>t.user_id == user_id && !t.is_deleted && t.status == todo_status.Pending).Count(),
            urgent_todo_counts = _context.Todos.Where(t =>t.user_id == user_id && !t.is_deleted && t.priority == todo_priority.Urgent).Count(),
        };
        return dashboard;
    }
}