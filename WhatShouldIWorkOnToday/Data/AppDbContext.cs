using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Models;

namespace WhatShouldIWorkOnToday.Data;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}