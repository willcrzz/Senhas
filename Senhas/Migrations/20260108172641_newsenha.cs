using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senhas.Migrations
{
    /// <inheritdoc />
    public partial class newsenha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataFinalizacao",
                table: "Senhas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Senhas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Senhas_UsuarioId",
                table: "Senhas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Senhas_Usuarios_UsuarioId",
                table: "Senhas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Senhas_Usuarios_UsuarioId",
                table: "Senhas");

            migrationBuilder.DropIndex(
                name: "IX_Senhas_UsuarioId",
                table: "Senhas");

            migrationBuilder.DropColumn(
                name: "DataFinalizacao",
                table: "Senhas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Senhas");
        }
    }
}
