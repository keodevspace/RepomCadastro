namespace Repom.Application.DTOs;

public record PessoaRequestDto(
    string NomeCompleto,
    string Cpf,
    DateOnly DataNascimento,
    string Email,
    string Telefone
);

public record VeiculoResumoDto(
    Guid Id,
    string Placa,
    int AnoFabricacao,
    string Marca,
    string Modelo
);

public record PessoaResponseDto(
    Guid Id,
    string NomeCompleto,
    string Cpf,
    DateOnly DataNascimento,
    string Email,
    string Telefone,
    IEnumerable<VeiculoResumoDto> Veiculos
);

public record VeiculoRequestDto(
    string Placa,
    int AnoFabricacao,
    string Marca,
    string Modelo,
    Guid PessoaId
);

public record VeiculoResponseDto(
    Guid Id,
    string Placa,
    int AnoFabricacao,
    string Marca,
    string Modelo,
    Guid PessoaId,
    string ProprietarioNome
);