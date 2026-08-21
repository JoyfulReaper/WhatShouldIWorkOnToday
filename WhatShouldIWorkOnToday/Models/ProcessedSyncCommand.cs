using System.ComponentModel.DataAnnotations;

namespace WhatShouldIWorkOnToday.Models;

public sealed class ProcessedSyncCommand
{
    [Key]
    public Guid CommandId { get; set; }

    [Required]
    [MaxLength(50)]
    public string CommandType { get; set; } = string.Empty;

    public DateTimeOffset ProcessedAtUtc { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = string.Empty;

    [Required]
    public string ReceiptJson { get; set; } = string.Empty;
}
