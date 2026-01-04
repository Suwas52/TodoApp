using TodoApplication.Enum;
using TodoApplication.Repository.Interfaces;

namespace TodoApplication.BackGround_Job;

public sealed class TodoExpiredJob
{
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<TodoReminderJob> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public TodoExpiredJob(
        ITodoRepository todoRepository, 
        ILogger<TodoReminderJob> logger,
        IUnitOfWork unitOfWork)
    {
        _todoRepository = todoRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync()
    {
        var expiredTodo = await _todoRepository.GetExpiredTodosAsync(CancellationToken.None);

        foreach (var item in expiredTodo)
        {
            item.status = todo_status.Expired;
            item.updated_at = DateTime.Now;
        }
        
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);
    }
}