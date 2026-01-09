using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senhas.Migrations
{
    /// <inheritdoc />
    public partial class removidousername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "Usuarios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Usuarios",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
