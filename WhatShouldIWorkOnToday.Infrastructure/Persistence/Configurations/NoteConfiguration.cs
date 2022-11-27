using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatShouldIWorkOnToday.Domain.Entities;

namespace WhatShouldIWorkOnToday.Infrastructure.Persistence.Configurations;

public class NoteConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder
            .HasKey(n => n.Id);

        builder.Property(n => n.Text)
            .HasMaxLength(2000)
            .IsRequired();
    }
}
