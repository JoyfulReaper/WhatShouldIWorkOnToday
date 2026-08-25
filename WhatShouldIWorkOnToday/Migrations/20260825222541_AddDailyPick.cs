using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatShouldIWorkOnToday.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyPick : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyPicks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    TodoItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyPicks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyPicks_TodoItems_TodoItemId",
                        column: x => x.TodoItemId,
                        principalTable: "TodoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyPicks_Date",
                table: "DailyPicks",
                column: "Date",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyPicks_TodoItemId",
                table: "DailyPicks",
                column: "TodoItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyPicks");
        }
    }
}
