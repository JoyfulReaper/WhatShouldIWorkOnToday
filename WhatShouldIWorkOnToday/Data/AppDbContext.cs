using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Models;

namespace WhatShouldIWorkOnToday.Data;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public DbSet<DailyPick> DailyPicks => Set<DailyPick>();

    public DbSet<WorkHistoryEntry> WorkHistoryEntries =>
        Set<WorkHistoryEntry>();

    public DbSet<ProcessedSyncCommand> ProcessedSyncCommands =>
        Set<ProcessedSyncCommand>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DailyPick>()
            .HasIndex(x => x.Date)
            .IsUnique();
    }
}
