using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaslManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCreationDateToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                schema: "TaskManagementDB",
                table: "UserTasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                schema: "TaskManagementDB",
                table: "UserTasks");
        }
    }
}
