using TodoApplication.Enum;

namespace TodoApplication.Entities;

public sealed class Todos
{
    public int id { get; set; }
    public Guid user_id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public todo_status status { get; set; }
    public todo_priority priority { get; set; }
    public bool is_deleted { get; set; }
    public DateTime created_at { get; set; }
    public string created_by { get; set; }
    public DateTime updated_at { get; set; }
    public string updated_by { get; set; }

    public Users createdTodoUser  { get; set; }
}