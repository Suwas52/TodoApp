using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApplication.Entities;

namespace TodoApplication.Configuration;


public sealed class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
{
    public void Configure(EntityTypeBuilder<VerificationCode> builder)
    {
        builder.HasKey(u => u.code_id);
        builder.Property(u => u.code_id)
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(u => u.user_id).IsRequired();
        builder.Property(u => u.codehash).HasMaxLength(500).IsRequired();
        builder.Property(u => u.expires_at).IsRequired();
        builder.Property(u => u.used_at).IsRequired(false);
        builder.Property(u => u.attempt_count).HasDefaultValue(0).IsRequired();
        builder.Property(u => u.CreatedAt).IsRequired();
        
        
    }
}