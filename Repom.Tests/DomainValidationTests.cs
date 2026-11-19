using Moq;
using Repom.Application.DTOs;
using Repom.Application.Services;
using Repom.Domain.Entities;
using Repom.Domain.Exceptions;
using Repom.Domain.Interfaces;

namespace Repom.Tests;

public class DomainValidationTests {
    [Fact]
    public void Veiculo_ComAnoFabricacaoFuturo_DeveLancarDomainException() {
        int anoFuturo = DateTime.UtcNow.Year + 1;

        var ex = Assert.Throws<DomainException>(() =>
            new Veiculo("ABC1D23", anoFuturo, "Volvo", "FH 540", Guid.NewGuid()));

        Assert.Contains("ano de fabricação", ex.Message);
    }

    [Fact]
    public async Task PessoaService_CriarComCpfDuplicado_DeveLancarDomainException() {
        var repoMock = new Mock<IPessoaRepository>();
        var pessoaExistente = new Pessoa("Carlos Souza", "12345678901", new DateOnly(1990, 5, 10), "carlos@email.com", "11999999999");

        repoMock.Setup(r => r.ObterPorCpfAsync("12345678901"))
                .ReturnsAsync(pessoaExistente);

        var service = new PessoaService(repoMock.Object);
        var dto = new PessoaRequestDto("Outro Nome", "123.456.789-01", new DateOnly(1995, 1, 1), "outro@email.com", "11888888888");

        await Assert.ThrowsAsync<DomainException>(() => service.CriarAsync(dto));
    }
}