using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatShouldIWorkOnToday.Domain.WorkItem.Entities;

namespace WhatShouldIWorkOnToday.Infrastructure.Persistence.Configurations;

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.Property(n => n.Text)
            .HasMaxLength(2000)
            .IsRequired();
    }
}
