using TodoApplication.Email;
using TodoApplication.Repository.Interfaces;

namespace TodoApplication.BackGround_Job;

public sealed class TodoSameDayReminderJob
{
    private readonly ITodoRepository _todoRepository;
    private readonly ITodoEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TodoSameDayReminderJob> _logger;

    public TodoSameDayReminderJob(
        ITodoRepository todoRepository,
        ITodoEmailService emailService,
        IUnitOfWork unitOfWork,
        ILogger<TodoSameDayReminderJob> logger)
    {
        _todoRepository = todoRepository;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var now = DateTime.Now;

        _logger.LogInformation(
            "Same-day todo reminder job started at {Time}",
            now
        );

        var candidates =
            await _todoRepository.GetSameDayDueTodosForOneHourReminderAsync(
                now,
                CancellationToken.None
            );

        if (!candidates.Any())
        {
            _logger.LogInformation("No same-day todos eligible for reminder");
            return;
        }

        var eligibleTodos = candidates
            .Where(t => now >= t.due_date.AddHours(-1))
            .ToList();

        if (!eligibleTodos.Any())
            return;

        var groupedByUser = eligibleTodos.GroupBy(t => t.user_id);

        foreach (var userGroup in groupedByUser)
        {
            var user = userGroup.First().createdTodoUser;

            await _emailService.SendTodoReminderAsync(
                user.email,
                userGroup.ToList()
            );

            foreach (var todo in userGroup)
            {
                todo.is_send_reminder = true;
                todo.updated_at = now;
            }
        }

        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        _logger.LogInformation(
            "Same-day todo reminder job completed successfully"
        );
    }
}
