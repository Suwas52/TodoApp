using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApplication.Entities;
using TodoApplication.Enum;

namespace TodoApplication.Configuration;

public sealed class UsersConfiguration : IEntityTypeConfiguration<Users>
{
    public void Configure(EntityTypeBuilder<Users> builder)
    {
        builder.HasKey(u => u.user_id);
        builder.Property(u => u.user_id)
            .HasDefaultValueSql("gen_random_uuid()");
        builder.Property(u => u.email).HasMaxLength(255).IsRequired();
        builder.Property(u => u.first_name).HasMaxLength(255).IsRequired();
        builder.Property(u => u.last_name).HasMaxLength(255).IsRequired();
        builder.Property(u => u.gender).HasDefaultValue(user_gender.Male).IsRequired();
        builder.Property(u => u.address).HasMaxLength(255).IsRequired(false);
        builder.Property(u => u.phone_number).HasMaxLength(15).IsRequired(false);
        builder.Property(u => u.password_hash).HasMaxLength(500).IsRequired();
        builder.Property(u => u.is_active).HasDefaultValue(true).IsRequired();
        builder.Property(u => u.is_deleted).HasDefaultValue(false).IsRequired();
        builder.Property(u => u.is_blocked).HasDefaultValue(false).IsRequired();
        builder.Property(u => u.email_confirmed).HasDefaultValue(false).IsRequired();
        builder.Property(u => u.is_active).HasDefaultValue(true).IsRequired();
        builder.Property(u => u.created_at).IsRequired();
        builder.Property(u => u.updated_at).IsRequired(false);
        builder.Property(u => u.login_fail_count).HasDefaultValue(0).IsRequired();
        builder.Property(u => u.last_login_date).IsRequired(false);
        builder.Property(u => u.password_change_date).IsRequired(false);
        
        builder.HasIndex(u => u.email).IsUnique();
        
        
    }
}