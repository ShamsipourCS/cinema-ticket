using CinemaTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaTicket.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();

        builder.Property(rt => rt.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(rt => rt.UserId)
            .IsRequired();

        // Unique index to ensure tokens are globally unique
        builder.HasIndex(rt => rt.Token)
            .IsUnique();

        // Index for performance on token lookup
        builder.HasIndex(rt => new { rt.UserId, rt.Token });

        // Index for finding non-revoked tokens
        builder.HasIndex(rt => new { rt.UserId, rt.IsRevoked, rt.ExpiresAt });

        // Relationship with User (already configured from User side)
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
