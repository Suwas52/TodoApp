using System.Net;
using System.Text;
using TodoApplication.Entities;

namespace TodoApplication.Email;

public interface ITodoEmailService
{
    Task SendTodoReminderAsync(
        string to,
        IReadOnlyList<Todos> todos,
        CancellationToken ct = default);
}
public sealed class TodoEmailService : ITodoEmailService
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<TodoEmailService> _logger;

    public TodoEmailService(
        IEmailSender emailSender,
        ILogger<TodoEmailService> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task SendTodoReminderAsync(
        string to,
        IReadOnlyList<Todos> todos,
        CancellationToken ct = default)
    {
        if (todos == null || todos.Count == 0)
            return;

        var subject = BuildSubject(todos);
        var body = BuildHtmlBody(todos);

        await _emailSender.SendAsync(
            to,
            subject,
            body,
            ct
        );

        _logger.LogInformation(
            "Todo reminder email sent to {Email} with {Count} todos",
            to,
            todos.Count
        );
    }

    private static string BuildSubject(IReadOnlyList<Todos> todos)
    {
        if (todos.Count == 1)
            return "Reminder: You have a task due today";

        return $"Reminder: You have {todos.Count} tasks due today";
    }

    private static string BuildHtmlBody(IReadOnlyList<Todos> todos)
    {
        var sb = new StringBuilder();

        sb.Append("""
            <html>
            <body style="font-family: Arial, sans-serif; color:#333;">
                <h2>⏰ Upcoming Tasks</h2>
                <p>You have the following tasks scheduled:</p>
                <ul style="padding-left:20px;">
        """);

        foreach (var todo in todos.OrderBy(t => t.due_date))
        {
            sb.Append($"""
                <li style="margin-bottom:10px;">
                    <strong>{WebUtility.HtmlEncode(todo.title)}</strong><br/>
                    <span>
                        🕒 Due at: {todo.due_date:hh:mm tt}
                    </span>
                </li>
            """);
        }

        sb.Append("""
                </ul>
                <hr/>
                <p style="font-size:12px;color:#777;">
                    This is an automated reminder. Please do not reply.
                </p>
            </body>
            </html>
        """);

        return sb.ToString();
    }
}
