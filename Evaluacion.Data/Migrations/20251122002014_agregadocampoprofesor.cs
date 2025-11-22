using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluacion.Data.Migrations
{
    /// <inheritdoc />
    public partial class agregadocampoprofesor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Profesor",
                table: "Cursos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Profesor",
                table: "Cursos");
        }
    }
}
