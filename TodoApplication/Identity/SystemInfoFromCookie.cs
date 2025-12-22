using System.Security.Claims;
using TodoApplication.Constant;

namespace TodoApplication.Identity;

public interface ISystemInfoFromCookie
{
    Guid user_id { get; }
    string full_name { get; }
    string? email { get; }
    List<string> roles { get; }
    bool IsInRole(string role);
    bool IsSuperAdmin { get; }
    bool IsManager { get; }
    bool IsUser { get; }
}
public sealed class SystemInfoFromCookie : ISystemInfoFromCookie
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private ClaimsPrincipal? User  => _httpContextAccessor.HttpContext?.User;

    public SystemInfoFromCookie(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Guid user_id => Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
    public string full_name => _httpContextAccessor.HttpContext?.User?.FindFirst("FullName")?.Value;
    public string email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
    
    public List<string> roles =>
        User?.FindAll(ClaimTypes.Role)
            .Select(r => r.Value)
            .ToList() ?? new List<string>();
    
    public bool IsInRole(string role) =>
        roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));

    // 🚀 Role Shortcuts
    public bool IsSuperAdmin => IsInRole(Role.SuperAdmin);
    public bool IsManager => IsInRole(Role.Manager);
    public bool IsUser => IsInRole(Role.User);
}