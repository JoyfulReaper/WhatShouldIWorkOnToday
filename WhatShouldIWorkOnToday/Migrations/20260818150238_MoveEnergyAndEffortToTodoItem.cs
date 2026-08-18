using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatShouldIWorkOnToday.Migrations
{
    /// <inheritdoc />
    public partial class MoveEnergyAndEffortToTodoItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Effort",
                table: "WorkItems");

            migrationBuilder.DropColumn(
                name: "Energy",
                table: "WorkItems");

            migrationBuilder.AddColumn<int>(
                name: "Effort",
                table: "TodoItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Energy",
                table: "TodoItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Effort",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "Energy",
                table: "TodoItems");

            migrationBuilder.AddColumn<int>(
                name: "Effort",
                table: "WorkItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Energy",
                table: "WorkItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
