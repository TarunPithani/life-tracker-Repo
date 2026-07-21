using LifeTracker.Domain.Common.Constants;
using LifeTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeTracker.Infrastructure.Configurations;

public class TodoConfiguration : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {
        builder.ToTable("Todos");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(TodoConstants.TitleMaxLength);

        builder.Property(t => t.Description)
            .HasMaxLength(TodoConstants.DescriptionMaxLength);

        builder.Property(t => t.Version)
            .IsRowVersion();
    }
}
