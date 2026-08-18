using Microsoft.EntityFrameworkCore;

namespace WhatShouldIWorkOnToday.Data;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
}