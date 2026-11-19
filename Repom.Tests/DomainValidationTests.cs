using Repom.Domain.Entities;
using Repom.Domain.Exceptions;

namespace Repom.Tests;

public class DomainValidationTests {
    private const string ValidCpf = "12345678901";
    private const string ValidFormattedCpf = "123.456.789-01";
    private const string ValidEmail = "carlos.souza@exemplo.com";
    private const string ValidPhone = "27999998888";
    private const string ValidLicensePlate = "ABC1D23";
    private const string ValidBrand = "Volvo";
    private const string ValidModel = "FH 540";
    private const int MinimumAllowedManufactureYear = 1886;

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Pessoa_WhenNameIsEmptyOrWhitespace_ShouldThrowDomainException(string? invalidFullName) {
        var birthDate = new DateOnly(1992, 4, 15);

        var domainException = Assert.Throws<DomainException>(() =>
            new Pessoa(invalidFullName!, ValidCpf, birthDate, ValidEmail, ValidPhone));

        Assert.Contains("nome completo", domainException.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Pessoa_WhenUpdatingWithInvalidName_ShouldThrowDomainException(string? invalidFullName) {
        var birthDate = new DateOnly(1992, 4, 15);
        var person = new Pessoa("Carlos Souza", ValidCpf, birthDate, ValidEmail, ValidPhone);

        var domainException = Assert.Throws<DomainException>(() =>
            person.AtualizarDados(invalidFullName!, ValidCpf, birthDate, ValidEmail, ValidPhone));

        Assert.Contains("nome completo", domainException.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("email-invalido")]
    [InlineData("usuario@")]
    [InlineData("@dominio.com")]
    [InlineData("")]
    public void Pessoa_WhenEmailFormatIsInvalid_ShouldThrowDomainException(string invalidEmail) {
        var birthDate = new DateOnly(1994, 8, 20);

        var domainException = Assert.Throws<DomainException>(() =>
            new Pessoa("Ana Souza", ValidCpf, birthDate, invalidEmail, ValidPhone));

        Assert.Contains("e-mail", domainException.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Pessoa_WhenCreatedWithFormattedCpfAndUppercaseEmail_ShouldNormalizeFields() {
        var birthDate = new DateOnly(1990, 5, 10);
        const string uppercaseEmail = "  CARLOS.SOUZA@EXEMPLO.COM  ";

        var person = new Pessoa("Carlos Souza", ValidFormattedCpf, birthDate, uppercaseEmail, ValidPhone);

        Assert.Equal(ValidCpf, person.Cpf);
        Assert.Equal("carlos.souza@exemplo.com", person.Email);
    }

    [Fact]
    public void Pessoa_WhenBirthDateIsInTheFuture_ShouldThrowDomainException() {
        const int daysInFuture = 1;
        var futureBirthDate = DateOnly.FromDateTime(DateTime.Today.AddDays(daysInFuture));

        var domainException = Assert.Throws<DomainException>(() =>
            new Pessoa("Marina Costa", ValidCpf, futureBirthDate, ValidEmail, ValidPhone));

        Assert.Contains("data de nascimento", domainException.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Veiculo_WhenManufactureYearIsInTheFuture_ShouldThrowDomainException() {
        const int yearsAhead = 1;
        int futureManufactureYear = DateTime.UtcNow.Year + yearsAhead;
        var ownerId = Guid.NewGuid();

        var domainException = Assert.Throws<DomainException>(() =>
            new Veiculo(ValidLicensePlate, futureManufactureYear, ValidBrand, ValidModel, ownerId));

        Assert.Contains("ano de fabricação", domainException.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Veiculo_WhenManufactureYearIsBelowMinimumAllowed_ShouldThrowDomainException() {
        const int yearsBelowMinimum = 1;
        int invalidManufactureYear = MinimumAllowedManufactureYear - yearsBelowMinimum;
        var ownerId = Guid.NewGuid();

        var domainException = Assert.Throws<DomainException>(() =>
            new Veiculo(ValidLicensePlate, invalidManufactureYear, ValidBrand, ValidModel, ownerId));

        Assert.Contains("ano de fabricação", domainException.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Veiculo_WhenManufactureYearEqualsCurrentYear_ShouldCreateSuccessfully() {
        int currentYear = DateTime.UtcNow.Year;
        var ownerId = Guid.NewGuid();

        var vehicle = new Veiculo("abc-1d23", currentYear, ValidBrand, ValidModel, ownerId);

        Assert.Equal(currentYear, vehicle.AnoFabricacao);
        Assert.Equal("ABC1D23", vehicle.Placa);
    }

    [Theory]
    [InlineData("", "Actros")]
    [InlineData("Mercedes-Benz", "")]
    [InlineData("   ", "FH 540")]
    [InlineData("Volvo", "   ")]
    public void Veiculo_WhenBrandOrModelIsMissing_ShouldThrowDomainException(string invalidBrand, string invalidModel) {
        int currentYear = DateTime.UtcNow.Year;
        var ownerId = Guid.NewGuid();

        var domainException = Assert.Throws<DomainException>(() =>
            new Veiculo(ValidLicensePlate, currentYear, invalidBrand, invalidModel, ownerId));

        Assert.Contains("marca e modelo", domainException.Message, StringComparison.OrdinalIgnoreCase);
    }
}