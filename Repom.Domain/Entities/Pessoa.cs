using System.Text.RegularExpressions;
using Repom.Domain.Exceptions;

namespace Repom.Domain.Entities;

public class Pessoa {
    public Guid Id { get; private set; }
    public string NomeCompleto { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;
    public DateOnly DataNascimento { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Telefone { get; private set; } = string.Empty;

    private readonly List<Veiculo> _veiculos = new();
    public IReadOnlyCollection<Veiculo> Veiculos => _veiculos.AsReadOnly();

    protected Pessoa() { }

    public Pessoa(string nomeCompleto, string cpf, DateOnly dataNascimento, string email, string telefone) {
        Id = Guid.NewGuid();
        AtualizarDados(nomeCompleto, cpf, dataNascimento, email, telefone);
    }

    public void AtualizarDados(string nomeCompleto, string cpf, DateOnly dataNascimento, string email, string telefone) {
        if (string.IsNullOrWhiteSpace(nomeCompleto))
            throw new DomainException("O nome completo é obrigatório.");

        var cpfLimpo = Regex.Replace(cpf ?? string.Empty, @"[^\d]", string.Empty);
        if (cpfLimpo.Length != 11)
            throw new DomainException("CPF inválido. Deve conter 11 dígitos numéricos.");

        var emailNormalizado = (email ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(emailNormalizado) || !Regex.IsMatch(emailNormalizado, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new DomainException("Formato de e-mail inválido.");

        if (dataNascimento > DateOnly.FromDateTime(DateTime.Today))
            throw new DomainException("A data de nascimento não pode ser futura.");

        NomeCompleto = nomeCompleto.Trim();
        Cpf = cpfLimpo;
        DataNascimento = dataNascimento;
        Email = emailNormalizado;
        Telefone = (telefone ?? string.Empty).Trim();
    }
}