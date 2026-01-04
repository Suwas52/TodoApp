using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApplication.Migrations
{
    /// <inheritdoc />
    public partial class add_column_todo_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_send_reminder",
                table: "Todos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "reminder_sent_at",
                table: "Todos",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_send_reminder",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "reminder_sent_at",
                table: "Todos");
        }
    }
}
