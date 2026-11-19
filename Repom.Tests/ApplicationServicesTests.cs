using Moq;
using Repom.Application.DTOs;
using Repom.Application.Services;
using Repom.Domain.Entities;
using Repom.Domain.Exceptions;
using Repom.Domain.Interfaces;

namespace Repom.Tests;

public class ApplicationServicesTests {
    private const string ExistingNormalizedCpf = "12345678901";
    private const string ExistingFormattedCpf = "123.456.789-01";
    private const string ExistingNormalizedPlate = "ABC1D23";
    private const string ExistingFormattedPlate = "abc-1d23";

    [Fact]
    public async Task PessoaService_WhenCreatingWithDuplicateCpf_ShouldThrowDomainException() {
        var pessoaRepositoryMock = new Mock<IPessoaRepository>();
        var existingPerson = new Pessoa(
            "Carlos Souza",
            ExistingNormalizedCpf,
            new DateOnly(1990, 5, 10),
            "carlos@exemplo.com",
            "27999999999");

        pessoaRepositoryMock
            .Setup(repository => repository.ObterPorCpfAsync(ExistingNormalizedCpf))
            .ReturnsAsync(existingPerson);

        var pessoaService = new PessoaService(pessoaRepositoryMock.Object);
        var duplicateCpfRequest = new PessoaRequestDto(
            "Outro Nome",
            ExistingFormattedCpf,
            new DateOnly(1995, 1, 1),
            "outro@exemplo.com",
            "27988888888");

        await Assert.ThrowsAsync<DomainException>(() => pessoaService.CriarAsync(duplicateCpfRequest));

        pessoaRepositoryMock.Verify(
            repository => repository.AdicionarAsync(It.IsAny<Pessoa>()),
            Times.Never);
    }

    [Fact]
    public async Task PessoaService_WhenUpdatingWithCpfBelongingToAnotherPerson_ShouldThrowDomainException() {
        var pessoaRepositoryMock = new Mock<IPessoaRepository>();
        var targetPerson = new Pessoa(
            "Ana Lima",
            "98765432100",
            new DateOnly(1991, 6, 12),
            "ana@exemplo.com",
            "27977777777");

        var anotherPersonWithSameCpf = new Pessoa(
            "Carlos Souza",
            ExistingNormalizedCpf,
            new DateOnly(1990, 5, 10),
            "carlos@exemplo.com",
            "27999999999");

        pessoaRepositoryMock
            .Setup(repository => repository.ObterPorIdAsync(targetPerson.Id))
            .ReturnsAsync(targetPerson);

        pessoaRepositoryMock
            .Setup(repository => repository.ObterPorCpfAsync(ExistingNormalizedCpf))
            .ReturnsAsync(anotherPersonWithSameCpf);

        var pessoaService = new PessoaService(pessoaRepositoryMock.Object);
        var updateRequest = new PessoaRequestDto(
            "Ana Lima Atualizada",
            ExistingFormattedCpf,
            new DateOnly(1991, 6, 12),
            "ana@exemplo.com",
            "27977777777");

        await Assert.ThrowsAsync<DomainException>(() => pessoaService.AtualizarAsync(targetPerson.Id, updateRequest));
    }

    [Fact]
    public async Task PessoaService_WhenGettingNonExistentId_ShouldThrowKeyNotFoundException() {
        var pessoaRepositoryMock = new Mock<IPessoaRepository>();
        var nonExistentPersonId = Guid.NewGuid();

        pessoaRepositoryMock
            .Setup(repository => repository.ObterPorIdAsync(nonExistentPersonId))
            .ReturnsAsync((Pessoa?)null);

        var pessoaService = new PessoaService(pessoaRepositoryMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => pessoaService.ObterPorIdAsync(nonExistentPersonId));
    }

    [Fact]
    public async Task PessoaService_WhenRemovingNonExistentId_ShouldThrowKeyNotFoundException() {
        var pessoaRepositoryMock = new Mock<IPessoaRepository>();
        var nonExistentPersonId = Guid.NewGuid();

        pessoaRepositoryMock
            .Setup(repository => repository.ObterPorIdAsync(nonExistentPersonId))
            .ReturnsAsync((Pessoa?)null);

        var pessoaService = new PessoaService(pessoaRepositoryMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => pessoaService.RemoverAsync(nonExistentPersonId));
    }

    [Fact]
    public async Task VeiculoService_WhenCreatingWithNonExistentOwner_ShouldThrowDomainException() {
        var veiculoRepositoryMock = new Mock<IVeiculoRepository>();
        var pessoaRepositoryMock = new Mock<IPessoaRepository>();
        var nonExistentOwnerId = Guid.NewGuid();

        pessoaRepositoryMock
            .Setup(repository => repository.ObterPorIdAsync(nonExistentOwnerId))
            .ReturnsAsync((Pessoa?)null);

        var veiculoService = new VeiculoService(veiculoRepositoryMock.Object, pessoaRepositoryMock.Object);
        var requestDto = new VeiculoRequestDto(
            ExistingNormalizedPlate,
            DateTime.UtcNow.Year,
            "Mercedes-Benz",
            "Actros 2651",
            nonExistentOwnerId);

        await Assert.ThrowsAsync<DomainException>(() => veiculoService.CriarAsync(requestDto));

        veiculoRepositoryMock.Verify(
            repository => repository.AdicionarAsync(It.IsAny<Veiculo>()),
            Times.Never);
    }

    [Fact]
    public async Task VeiculoService_WhenCreatingWithDuplicateLicensePlate_ShouldThrowDomainException() {
        var veiculoRepositoryMock = new Mock<IVeiculoRepository>();
        var pessoaRepositoryMock = new Mock<IPessoaRepository>();
        var owner = new Pessoa(
            "Carlos Lima",
            ExistingNormalizedCpf,
            new DateOnly(1988, 3, 10),
            "carlos@exemplo.com",
            "27977776666");

        var existingVehicle = new Veiculo(
            ExistingNormalizedPlate,
            2023,
            "Scania",
            "R 450",
            owner.Id);

        pessoaRepositoryMock
            .Setup(repository => repository.ObterPorIdAsync(owner.Id))
            .ReturnsAsync(owner);

        veiculoRepositoryMock
            .Setup(repository => repository.ObterPorPlacaAsync(ExistingNormalizedPlate))
            .ReturnsAsync(existingVehicle);

        var veiculoService = new VeiculoService(veiculoRepositoryMock.Object, pessoaRepositoryMock.Object);
        var duplicatePlateRequest = new VeiculoRequestDto(
            ExistingFormattedPlate,
            2024,
            "Volvo",
            "FH 540",
            owner.Id);

        await Assert.ThrowsAsync<DomainException>(() => veiculoService.CriarAsync(duplicatePlateRequest));

        veiculoRepositoryMock.Verify(
            repository => repository.AdicionarAsync(It.IsAny<Veiculo>()),
            Times.Never);
    }

    [Fact]
    public async Task VeiculoService_WhenUpdatingWithPlateBelongingToAnotherVehicle_ShouldThrowDomainException() {
        var veiculoRepositoryMock = new Mock<IVeiculoRepository>();
        var pessoaRepositoryMock = new Mock<IPessoaRepository>();
        var owner = new Pessoa(
            "Carlos Lima",
            ExistingNormalizedCpf,
            new DateOnly(1988, 3, 10),
            "carlos@exemplo.com",
            "27977776666");

        var targetVehicle = new Veiculo("XYZ9K88", 2022, "Volvo", "VM 270", owner.Id);
        var anotherVehicleWithSamePlate = new Veiculo(ExistingNormalizedPlate, 2023, "Scania", "R 450", owner.Id);

        veiculoRepositoryMock
            .Setup(repository => repository.ObterPorIdAsync(targetVehicle.Id))
            .ReturnsAsync(targetVehicle);

        pessoaRepositoryMock
            .Setup(repository => repository.ObterPorIdAsync(owner.Id))
            .ReturnsAsync(owner);

        veiculoRepositoryMock
            .Setup(repository => repository.ObterPorPlacaAsync(ExistingNormalizedPlate))
            .ReturnsAsync(anotherVehicleWithSamePlate);

        var veiculoService = new VeiculoService(veiculoRepositoryMock.Object, pessoaRepositoryMock.Object);
        var updateRequest = new VeiculoRequestDto(
            ExistingFormattedPlate,
            2022,
            "Volvo",
            "VM 270",
            owner.Id);

        await Assert.ThrowsAsync<DomainException>(() => veiculoService.AtualizarAsync(targetVehicle.Id, updateRequest));
    }
}