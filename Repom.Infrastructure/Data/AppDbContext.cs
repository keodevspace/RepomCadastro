using Microsoft.EntityFrameworkCore;
using Repom.Domain.Entities;
using System.Reflection.Emit;

namespace Repom.Infrastructure.Data;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Pessoa> Pessoas => Set<Pessoa>();
    public DbSet<Veiculo> Veiculos => Set<Veiculo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Pessoa>(entity => {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.NomeCompleto).IsRequired().HasMaxLength(150);
            entity.Property(p => p.Cpf).IsRequired().HasMaxLength(11);
            entity.HasIndex(p => p.Cpf).IsUnique();
            entity.Property(p => p.Email).IsRequired().HasMaxLength(150);
            entity.Property(p => p.Telefone).HasMaxLength(20);

            entity.HasMany(p => p.Veiculos)
                  .WithOne(v => v.Pessoa)
                  .HasForeignKey(v => v.PessoaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Veiculo>(entity => {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Placa).IsRequired().HasMaxLength(7);
            entity.HasIndex(v => v.Placa).IsUnique();
            entity.Property(v => v.Marca).IsRequired().HasMaxLength(80);
            entity.Property(v => v.Modelo).IsRequired().HasMaxLength(80);
            entity.Property(v => v.AnoFabricacao).IsRequired();
        });
    }
}