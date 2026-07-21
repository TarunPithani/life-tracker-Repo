using LifeTracker.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeTracker.Infrastructure.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(token => token.Id);

        builder.Property(token => token.Token)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(token => token.CreatedByIp)
            .HasMaxLength(64);

        builder.Property(token => token.RevokedByIp)
            .HasMaxLength(64);

        builder.Property(token => token.ReplacedByToken)
            .HasMaxLength(512);

        builder.HasIndex(token => token.Token)
            .IsUnique();

        builder.HasOne(token => token.User)
            .WithMany(user => user.RefreshTokens)
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
