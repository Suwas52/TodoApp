using TodoApplication.Enum;
using TodoApplication.Repository.Interfaces;

namespace TodoApplication.BackGround_Job;


public class TodoExpiredJob
{
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<TodoExpiredJob> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public TodoExpiredJob(
        ITodoRepository todoRepository,
        ILogger<TodoExpiredJob> logger,
        IUnitOfWork unitOfWork)
    {
        _todoRepository = todoRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       var expiredTodo = await  _todoRepository.GetExpiredTodosAsync(stoppingToken);

       foreach (var todo in expiredTodo)
       {
           todo.status = todo_status.Expired;
           todo.updated_at = DateTime.Now;
           
       }
       
       await _unitOfWork.SaveChangesAsync(stoppingToken);
       
    }
}