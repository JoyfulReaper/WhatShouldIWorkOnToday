using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatShouldIWorkOnToday.Domain.Entities;

namespace WhatShouldIWorkOnToday.Infrastructure.Persistence.Configurations;

public class WsiwotSettingsConfiguration : IEntityTypeConfiguration<WsiwotSettings>
{
    public void Configure(EntityTypeBuilder<WsiwotSettings> builder)
    {
    }
}
