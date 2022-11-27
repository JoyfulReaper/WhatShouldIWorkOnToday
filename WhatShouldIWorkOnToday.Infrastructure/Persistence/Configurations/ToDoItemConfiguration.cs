using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatShouldIWorkOnToday.Domain.Entities;

namespace WhatShouldIWorkOnToday.Infrastructure.Persistence.Configurations;
public class ToDoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.Property(t => t.Text)
            .IsRequired()
            .HasMaxLength(300);
    }
}
