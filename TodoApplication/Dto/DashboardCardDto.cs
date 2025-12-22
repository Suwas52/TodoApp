namespace TodoApplication.Dto;

public class DashboardCardDto
{
    public int? total_user_counts { get; set; }
    public int total_todo_counts { get; set; }
    public int expired_todo_counts { get; set; }
    public int? active_user_counts { get; set; }
    public int? inactive_user_counts { get; set; }
    public int? blocked_user_counts { get; set; }
    public int? completed_todo_counts { get; set; }
    public int? high_priority_todo_counts { get; set; }
    public int? pending_todo_counts { get; set; }
    public int? urgent_todo_counts { get; set; }
}

public class dashboard
{
    public DashboardCardDto dashboard_card { get; set; }
    public List<TodoListDto>  todo_list { get; set; }
}
