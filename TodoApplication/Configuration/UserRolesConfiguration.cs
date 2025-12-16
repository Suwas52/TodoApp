using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApplication.Entities;

namespace TodoApplication.Configuration;

public sealed class UserRolesConfiguration : IEntityTypeConfiguration<UserRoles>
{
    public void Configure(EntityTypeBuilder<UserRoles> builder)
    {
        builder.HasKey(ur => new { ur.user_id, ur.role_id });
        
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.userroles)
            .HasForeignKey(ur => ur.user_id)
            .OnDelete(DeleteBehavior.Cascade);
        
                
        builder.HasOne(ur => ur.Role)
            .WithMany(u => u.userroles)
            .HasForeignKey(ur => ur.role_id)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(ur => new { ur.role_id , ur.user_id});
    }
}