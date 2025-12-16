using Microsoft.EntityFrameworkCore;
using TodoApplication.Entities;

namespace TodoApplication.Data;

public class TodoAppDbContext : DbContext
{
    public TodoAppDbContext(DbContextOptions<TodoAppDbContext> Options) : base(Options)
    {
        
    }
    
    public DbSet<Todos>  Todos { get; set; }
    public DbSet<Users>  Users { get; set; }
    public DbSet<Roles>  Roles { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodoAppDbContext).Assembly);
    }
}