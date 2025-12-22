using TodoApplication.Enum;

namespace TodoApplication.Dto;

public class TodoListDto
{
    public int id { get; set; }
    public string username { get; set; }
    public string title { get; set; }
    public todo_status status { get; set; }
    public todo_priority priority { get; set; }
    public DateTime created_at { get; set; }
}