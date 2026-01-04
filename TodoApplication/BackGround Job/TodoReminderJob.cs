using TodoApplication.Email;
using TodoApplication.Repository.Interfaces;

namespace TodoApplication.BackGround_Job;


public class TodoReminderJob
{
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<TodoReminderJob> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TodoEmailService _todoEmailService;
    public TodoReminderJob(
        ITodoRepository todoRepository,
        ILogger<TodoReminderJob> logger,
        IUnitOfWork unitOfWork,
        TodoEmailService todoEmailService)
    {  
        _todoRepository = todoRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _todoEmailService = todoEmailService;
    }
    public async Task ExecuteAsync()
    {
       var todos = await  _todoRepository.GetTodosForTodayReminderAsync(CancellationToken.None);

       if (!todos.Any())
       {
           _logger.LogInformation("No todos to remind today");
           return;
       }

       var groupByUser = todos.GroupBy(t => t.user_id);

       foreach (var userTodos in groupByUser)
       {
           var user = userTodos.First().createdTodoUser;
           
           _todoEmailService.SendTodoReminderAsync(
               user.email,
               userTodos.ToList());
           
           foreach(var todo in userTodos)
           {
               todo.is_send_reminder = true;
               todo.reminder_sent_at = DateTime.Now;
           }

       }
       
       await _unitOfWork.SaveChangesAsync(CancellationToken.None);
       
    }
}