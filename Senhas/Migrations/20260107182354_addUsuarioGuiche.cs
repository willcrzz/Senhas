using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senhas.Migrations
{
    /// <inheritdoc />
    public partial class addUsuarioGuiche : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioGuiches_Guiches_GuicheId",
                table: "UsuarioGuiches");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioGuiches_Usuarios_UsuarioId",
                table: "UsuarioGuiches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioGuiches",
                table: "UsuarioGuiches");

            migrationBuilder.RenameTable(
                name: "UsuarioGuiches",
                newName: "UsuariosGuiches");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioGuiches_GuicheId",
                table: "UsuariosGuiches",
                newName: "IX_UsuariosGuiches_GuicheId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuariosGuiches",
                table: "UsuariosGuiches",
                columns: new[] { "UsuarioId", "GuicheId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosGuiches_Guiches_GuicheId",
                table: "UsuariosGuiches",
                column: "GuicheId",
                principalTable: "Guiches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosGuiches_Usuarios_UsuarioId",
                table: "UsuariosGuiches",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosGuiches_Guiches_GuicheId",
                table: "UsuariosGuiches");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosGuiches_Usuarios_UsuarioId",
                table: "UsuariosGuiches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuariosGuiches",
                table: "UsuariosGuiches");

            migrationBuilder.RenameTable(
                name: "UsuariosGuiches",
                newName: "UsuarioGuiches");

            migrationBuilder.RenameIndex(
                name: "IX_UsuariosGuiches_GuicheId",
                table: "UsuarioGuiches",
                newName: "IX_UsuarioGuiches_GuicheId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioGuiches",
                table: "UsuarioGuiches",
                columns: new[] { "UsuarioId", "GuicheId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioGuiches_Guiches_GuicheId",
                table: "UsuarioGuiches",
                column: "GuicheId",
                principalTable: "Guiches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioGuiches_Usuarios_UsuarioId",
                table: "UsuarioGuiches",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
