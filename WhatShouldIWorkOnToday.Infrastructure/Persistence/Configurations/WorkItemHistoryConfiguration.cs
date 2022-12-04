using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatShouldIWorkOnToday.Domain.WorkItem.Entities;

namespace WhatShouldIWorkOnToday.Infrastructure.Persistence.Configurations;

public class WorkItemHistoryConfiguration : IEntityTypeConfiguration<WorkItemHistory>
{
    public void Configure(EntityTypeBuilder<WorkItemHistory> builder)
    {
    }
}
