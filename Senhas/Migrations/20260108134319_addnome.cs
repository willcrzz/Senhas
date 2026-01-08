using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senhas.Migrations
{
    /// <inheritdoc />
    public partial class addnome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioNome",
                table: "AuditoriaSistema");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioNome",
                table: "AuditoriaSistema",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
