using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pessoas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NomeCompleto = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Cpf = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    DataNascimento = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pessoas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Veiculos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Placa = table.Column<string>(type: "TEXT", maxLength: 7, nullable: false),
                    AnoFabricacao = table.Column<int>(type: "INTEGER", nullable: false),
                    Marca = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Modelo = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    PessoaId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veiculos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veiculos_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_Cpf",
                table: "Pessoas",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_PessoaId",
                table: "Veiculos",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_Placa",
                table: "Veiculos",
                column: "Placa",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Veiculos");

            migrationBuilder.DropTable(
                name: "Pessoas");
        }
    }
}
