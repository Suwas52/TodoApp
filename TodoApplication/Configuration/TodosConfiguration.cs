using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApplication.Entities;
using TodoApplication.Enum;

namespace TodoApplication.Configuration;

public sealed class TodosConfiguration : IEntityTypeConfiguration<Todos>
{
    public void Configure(EntityTypeBuilder<Todos> builder)
    {
        builder.HasKey(t => t.id);
        builder.Property(t => t.id).ValueGeneratedOnAdd();
        builder.Property(t => t.title).HasMaxLength(255).IsRequired();
        builder.Property(t => t.status).HasDefaultValue(todo_status.Pending).IsRequired();
        builder.Property(t => t.priority).HasDefaultValue(todo_priority.Low).IsRequired();
        builder.Property(t => t.created_at).IsRequired();
        builder.Property(t => t.created_by).HasMaxLength(255).IsRequired();
        builder.Property(t => t.updated_at).IsRequired();
        builder.Property(t => t.updated_by).HasMaxLength(255).IsRequired(false);
    }
}