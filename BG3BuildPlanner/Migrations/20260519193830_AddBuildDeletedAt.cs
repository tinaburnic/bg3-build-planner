using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BG3BuildPlanner.Migrations
{
    /// <inheritdoc />
    public partial class AddBuildDeletedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Builds",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Builds");
        }
    }
}
