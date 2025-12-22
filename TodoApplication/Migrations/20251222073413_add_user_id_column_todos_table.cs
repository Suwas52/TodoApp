using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApplication.Migrations
{
    /// <inheritdoc />
    public partial class add_user_id_column_todos_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "Todos",
                type: "uuid",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Todos_user_id",
                table: "Todos",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_Users_user_id",
                table: "Todos",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todos_Users_user_id",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_Todos_user_id",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "Todos");
        }
    }
}
