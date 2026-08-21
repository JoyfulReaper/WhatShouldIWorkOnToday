using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatShouldIWorkOnToday.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessedSyncCommands : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessedSyncCommands",
                columns: table => new
                {
                    CommandId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CommandType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ProcessedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ReceiptJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedSyncCommands", x => x.CommandId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessedSyncCommands");
        }
    }
}
