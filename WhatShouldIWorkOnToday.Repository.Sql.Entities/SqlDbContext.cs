using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Repository.Sql.Entities.Entities;

namespace WhatShouldIWorkOnToday.Repository.Sql.Entities;

public partial class SqlDbContext : DbContext
{
    public SqlDbContext()
    {
    }

    public SqlDbContext(DbContextOptions<SqlDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Note> Notes { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<ToDoItem> ToDoItems { get; set; }

    public virtual DbSet<WorkItem> WorkItems { get; set; }

    public virtual DbSet<WorkItemHistory> WorkItemHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=Wsiwot;Trusted_Connection=True;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.NoteId).HasName("PK__Note__EACE355FE2381320");

            entity.ToTable("Note");

            entity.Property(e => e.DateCreated).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Text).HasMaxLength(2000);

            entity.HasOne(d => d.WorkItem).WithMany(p => p.Notes)
                .HasForeignKey(d => d.WorkItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Note_WorkItem");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.SettingId).HasName("PK__Setting__54372B1D8DFAB45C");

            entity.ToTable("Setting");

            entity.Property(e => e.DateSet)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("date");
        });

        modelBuilder.Entity<ToDoItem>(entity =>
        {
            entity.HasKey(e => e.ToDoItemId).HasName("PK__ToDoItem__F05DDEC755263142");

            entity.ToTable("ToDoItem");

            entity.Property(e => e.DateAdded).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Task)
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.HasOne(d => d.WorkItem).WithMany(p => p.ToDoItems)
                .HasForeignKey(d => d.WorkItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ToDoItem_WorkItem");
        });

        modelBuilder.Entity<WorkItem>(entity =>
        {
            entity.HasKey(e => e.WorkItemId).HasName("PK__WorkItem__A10D1B4548AA8A86");

            entity.ToTable("WorkItem");

            entity.Property(e => e.DateCreated).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(300);
            entity.Property(e => e.Url).HasMaxLength(300);
        });

        modelBuilder.Entity<WorkItemHistory>(entity =>
        {
            entity.HasKey(e => e.WorkItemHistoryId).HasName("PK__WorkItem__A11122F03433EA2B");

            entity.ToTable("WorkItemHistory");

            entity.Property(e => e.DateWorkedOn).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.WorkItem).WithMany(p => p.WorkItemHistories)
                .HasForeignKey(d => d.WorkItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkItemHistory_WorkItem");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
