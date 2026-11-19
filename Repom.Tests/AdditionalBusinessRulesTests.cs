using Moq;
using Repom.Application.DTOs;
using Repom.Application.Services;
using Repom.Domain.Entities;
using Repom.Domain.Exceptions;
using Repom.Domain.Interfaces;

namespace Repom.Tests;

public class AdditionalBusinessRulesTests {
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Pessoa_SemNomeValido_DeveLancarDomainException(string? nomeInvalido) {
        var exception = Assert.Throws<DomainException>(() =>
            new Pessoa(
                nomeInvalido!,
                "12345678901",
                new DateOnly(1992, 4, 15),
                "pessoa@exemplo.com",
                "27999999999"));

        Assert.Contains("nome completo", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("email-invalido")]
    [InlineData("usuario@")]
    [InlineData("@dominio.com")]
    public void Pessoa_ComEmailInvalido_DeveLancarDomainException(string emailInvalido) {
        var exception = Assert.Throws<DomainException>(() =>
            new Pessoa(
                "Ana Souza",
                "12345678901",
                new DateOnly(1994, 8, 20),
                emailInvalido,
                "27988887777"));

        Assert.Contains("e-mail", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task VeiculoService_CriarComPessoaInexistente_DeveLancarDomainException() {
        var veiculoRepositoryMock = new Mock<IVeiculoRepository>();
        var pessoaRepositoryMock = new Mock<IPessoaRepository>();
        var pessoaIdInexistente = Guid.NewGuid();

        pessoaRepositoryMock
            .Setup(repository => repository.ObterPorIdAsync(pessoaIdInexistente))
            .ReturnsAsync((Pessoa?)null);

        var veiculoService = new VeiculoService(veiculoRepositoryMock.Object, pessoaRepositoryMock.Object);
        var requestDto = new VeiculoRequestDto(
            "ABC1D23",
            DateTime.UtcNow.Year,
            "Mercedes-Benz",
            "Actros 2651",
            pessoaIdInexistente);

        await Assert.ThrowsAsync<DomainException>(() => veiculoService.CriarAsync(requestDto));
        veiculoRepositoryMock.Verify(
            repository => repository.AdicionarAsync(It.IsAny<Veiculo>()),
            Times.Never);
    }

    [Fact]
    public async Task VeiculoService_CriarComPlacaDuplicada_DeveLancarDomainException() {
        var veiculoRepositoryMock = new Mock<IVeiculoRepository>();
        var pessoaRepositoryMock = new Mock<IPessoaRepository>();
        var proprietarioId = Guid.NewGuid();

        var proprietario = new Pessoa(
            "Carlos Lima",
            "12345678901",
            new DateOnly(1988, 3, 10),
            "carlos@exemplo.com",
            "27977776666");

        var veiculoExistente = new Veiculo(
            "ABC1D23",
            2023,
            "Scania",
            "R 450",
            proprietarioId);

        pessoaRepositoryMock
            .Setup(repository => repository.ObterPorIdAsync(proprietarioId))
            .ReturnsAsync(proprietario);

        veiculoRepositoryMock
            .Setup(repository => repository.ObterPorPlacaAsync("ABC1D23"))
            .ReturnsAsync(veiculoExistente);

        var veiculoService = new VeiculoService(veiculoRepositoryMock.Object, pessoaRepositoryMock.Object);
        var requestDto = new VeiculoRequestDto(
            "abc-1d23",
            2024,
            "Volvo",
            "FH 540",
            proprietarioId);

        await Assert.ThrowsAsync<DomainException>(() => veiculoService.CriarAsync(requestDto));
    }

    [Fact]
    public async Task PessoaService_ObterPorIdInexistente_DeveLancarKeyNotFoundException() {
        var pessoaRepositoryMock = new Mock<IPessoaRepository>();
        var idInexistente = Guid.NewGuid();

        pessoaRepositoryMock
            .Setup(repository => repository.ObterPorIdAsync(idInexistente))
            .ReturnsAsync((Pessoa?)null);

        var pessoaService = new PessoaService(pessoaRepositoryMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => pessoaService.ObterPorIdAsync(idInexistente));
    }
}