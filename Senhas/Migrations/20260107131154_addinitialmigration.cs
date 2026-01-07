using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Senhas.Migrations
{
    /// <inheritdoc />
    public partial class addinitialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guiches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guiches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposSenha",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Prefixo = table.Column<string>(type: "text", nullable: false),
                    Prioridade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposSenha", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Senhas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    TipoSenhaId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataChamada = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GuicheId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Senhas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Senhas_Guiches_GuicheId",
                        column: x => x.GuicheId,
                        principalTable: "Guiches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Senhas_TiposSenha_TipoSenhaId",
                        column: x => x.TipoSenhaId,
                        principalTable: "TiposSenha",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Senhas_GuicheId",
                table: "Senhas",
                column: "GuicheId");

            migrationBuilder.CreateIndex(
                name: "IX_Senhas_TipoSenhaId",
                table: "Senhas",
                column: "TipoSenhaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Senhas");

            migrationBuilder.DropTable(
                name: "Guiches");

            migrationBuilder.DropTable(
                name: "TiposSenha");
        }
    }
}
