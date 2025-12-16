using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApplication.Entities;

namespace TodoApplication.Configuration;

public sealed class RolesConfiguration : IEntityTypeConfiguration<Roles>
{
    public void Configure(EntityTypeBuilder<Roles> builder)
    {
        builder.HasKey(r => r.role_id);
        builder.Property(u => u.role_id)
            .HasDefaultValueSql("gen_random_uuid()");
        builder.Property(r => r.role_name).HasMaxLength(255).IsRequired();
        builder.Property(r => r.created_at).IsRequired();
        builder.Property(r => r.updated_at).IsRequired(false);
        builder.Property(r => r.is_deleted).HasDefaultValue(false).IsRequired();
        builder.HasIndex(r => r.role_name).IsUnique();
    }
}