using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginWebApi.Migrations
{
    public partial class AddDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "fechaPublicacionLibro",
                table: "libros",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fechaPublicacionLibro",
                table: "libros");
        }
    }
}
