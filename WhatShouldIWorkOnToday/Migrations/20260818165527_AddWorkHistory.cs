using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatShouldIWorkOnToday.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkHistoryEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    TodoItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    TaskSnapshot = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    WorkedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkHistoryEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkHistoryEntries_WorkItems_WorkItemId",
                        column: x => x.WorkItemId,
                        principalTable: "WorkItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkHistoryEntries_WorkItemId",
                table: "WorkHistoryEntries",
                column: "WorkItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkHistoryEntries");
        }
    }
}
