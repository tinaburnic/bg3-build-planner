using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BG3BuildPlanner.Migrations
{
    /// <inheritdoc />
    public partial class AddSkillDeletedAtAndCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Skills",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Skills",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Skills");
        }
    }
}
