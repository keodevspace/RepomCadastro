using System.Text.RegularExpressions;
using Repom.Domain.Exceptions;

namespace Repom.Domain.Entities;

public class Veiculo {
    public Guid Id { get; private set; }
    public string Placa { get; private set; } = string.Empty;
    public int AnoFabricacao { get; private set; }
    public string Marca { get; private set; } = string.Empty;
    public string Modelo { get; private set; } = string.Empty;

    public Guid PessoaId { get; private set; }
    public Pessoa? Pessoa { get; private set; }

    protected Veiculo() { } // EF Core

    public Veiculo(string placa, int anoFabricacao, string marca, string modelo, Guid pessoaId) {
        Id = Guid.NewGuid();
        AtualizarDados(placa, anoFabricacao, marca, modelo, pessoaId);
    }

    public void AtualizarDados(string placa, int anoFabricacao, string marca, string modelo, Guid pessoaId) {
        var placaLimpa = Regex.Replace(placa ?? "", @"[^a-zA-Z0-9]", "").ToUpperInvariant();
        if (placaLimpa.Length != 7)
            throw new DomainException("A placa deve conter 7 caracteres (padrão antigo ou Mercosul).");

        int anoAtual = DateTime.UtcNow.Year;
        if (anoFabricacao < 1886 || anoFabricacao > anoAtual)
            throw new DomainException($"O ano de fabricação deve estar entre 1886 e {anoAtual}.");

        if (string.IsNullOrWhiteSpace(marca) || string.IsNullOrWhiteSpace(modelo))
            throw new DomainException("Marca e modelo são obrigatórios.");

        if (pessoaId == Guid.Empty)
            throw new DomainException("Todo veículo deve estar vinculado a uma pessoa válida.");

        Placa = placaLimpa;
        AnoFabricacao = anoFabricacao;
        Marca = marca.Trim();
        Modelo = modelo.Trim();
        PessoaId = pessoaId;
    }
}