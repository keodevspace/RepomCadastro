CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "Pessoas" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Pessoas" PRIMARY KEY,
    "NomeCompleto" TEXT NOT NULL,
    "Cpf" TEXT NOT NULL,
    "DataNascimento" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "Telefone" TEXT NOT NULL
);

CREATE TABLE "Veiculos" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Veiculos" PRIMARY KEY,
    "Placa" TEXT NOT NULL,
    "AnoFabricacao" INTEGER NOT NULL,
    "Marca" TEXT NOT NULL,
    "Modelo" TEXT NOT NULL,
    "PessoaId" TEXT NOT NULL,
    CONSTRAINT "FK_Veiculos_Pessoas_PessoaId" FOREIGN KEY ("PessoaId") REFERENCES "Pessoas" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_Pessoas_Cpf" ON "Pessoas" ("Cpf");

CREATE INDEX "IX_Veiculos_PessoaId" ON "Veiculos" ("PessoaId");

CREATE UNIQUE INDEX "IX_Veiculos_Placa" ON "Veiculos" ("Placa");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261119084932_InitialCreate', '10.0.12');

COMMIT;

