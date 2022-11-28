﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatShouldIWorkOnToday.Domain.Entities;

namespace WhatShouldIWorkOnToday.Infrastructure.Persistence.Configurations;

public class WorkItemConfiguration : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(w => w.Description)
            .HasMaxLength(500);

        builder.Property(w => w.Url)
            .HasMaxLength(300);
    }
}