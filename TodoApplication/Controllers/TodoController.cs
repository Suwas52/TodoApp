using Microsoft.AspNetCore.Mvc;
using TodoApplication.Dto;
using TodoApplication.Services.Interfaces;

namespace TodoApplication.Controllers;

public class TodoController : Controller
{
    private readonly ILogger<TodoController> _logger;
    private readonly ITodoService _todoService;

    public TodoController(
        ILogger<TodoController> logger
        , ITodoService todoService)
    {
        _logger = logger;
        _todoService = todoService;
    }

    public async Task<IActionResult> TodoList(CancellationToken ct)
    {
        var todoList = await _todoService.GetAllTodos(ct);
        return View(todoList);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTodo(CreateTodoDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var result = await _todoService.AddTodo(dto, ct);
        if (!result.issucceed)
        {
            ModelState.AddModelError("", result.message);
            return View(dto);
        }

        return RedirectToAction(nameof(TodoList));
    }
    
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var todo = await _todoService.GetTodoById(id, ct);
        if (todo == null) return NotFound();

        return View(todo);
    }

    public async Task<IActionResult> DeleteTodo(int id, CancellationToken ct)
    {
        
        var result = await _todoService.DeleteTodo(id, ct);
        if (!result.issucceed)
        {
            ModelState.AddModelError("", result.message);
            return View(result);
        }

        return RedirectToAction(nameof(TodoList));
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var todo = await _todoService.GetTodoById(id, ct);
        if (todo == null) return NotFound();

        var dto = new CreateTodoDto
        {
            title = todo.title,
            description = todo.description,
            priority = todo.priority
        };

        return View(dto);
    }

    // EDIT - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateTodoDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await _todoService.UpdateTodo(id, dto, ct);
        return RedirectToAction(nameof(TodoList));
    }
    
}