using TodoApplication.Enum;

namespace TodoApplication.Dto;

public class CreateTodoDto
{
    public string title { get; set; }
    public string description  { get; set; }
    public todo_priority priority { get; set; }
}