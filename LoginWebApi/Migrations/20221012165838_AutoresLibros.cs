using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginWebApi.Migrations
{
    public partial class AutoresLibros : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AutorLibros",
                columns: table => new
                {
                    LibroId = table.Column<int>(type: "int", nullable: false),
                    AutorId = table.Column<int>(type: "int", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutorLibros", x => new { x.AutorId, x.LibroId });
                    table.ForeignKey(
                        name: "FK_AutorLibros_Autors_AutorId",
                        column: x => x.AutorId,
                        principalTable: "Autors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AutorLibros_libros_LibroId",
                        column: x => x.LibroId,
                        principalTable: "libros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutorLibros_LibroId",
                table: "AutorLibros",
                column: "LibroId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutorLibros");
        }
    }
}
