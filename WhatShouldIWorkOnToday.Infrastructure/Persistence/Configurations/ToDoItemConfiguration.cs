using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatShouldIWorkOnToday.Domain.Entities;

namespace WhatShouldIWorkOnToday.Infrastructure.Persistence.Configurations;

public class ToDoItemConfiguration : IEntityTypeConfiguration<ToDoItem>
{
    public void Configure(EntityTypeBuilder<ToDoItem> builder)
    {
        builder.Property(t => t.Task)
            .IsRequired()
            .HasMaxLength(300);
    }
}
