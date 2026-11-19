using System.Text.RegularExpressions;
using Repom.Application.DTOs;
using Repom.Domain.Entities;
using Repom.Domain.Exceptions;
using Repom.Domain.Interfaces;

namespace Repom.Application.Services;

public class VeiculoService {
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IPessoaRepository _pessoaRepository;

    public VeiculoService(IVeiculoRepository veiculoRepository, IPessoaRepository pessoaRepository) {
        _veiculoRepository = veiculoRepository;
        _pessoaRepository = pessoaRepository;
    }

    public async Task<IEnumerable<VeiculoResponseDto>> ListarTodosAsync() {
        var veiculos = await _veiculoRepository.ObterTodosAsync();
        return veiculos.Select(MapearParaResponse);
    }

    public async Task<VeiculoResponseDto> ObterPorIdAsync(Guid id) {
        var veiculo = await _veiculoRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Veículo não encontrado.");
        return MapearParaResponse(veiculo);
    }

    public async Task<VeiculoResponseDto> CriarAsync(VeiculoRequestDto dto) {
        var pessoaExistente = await _pessoaRepository.ObterPorIdAsync(dto.PessoaId)
            ?? throw new DomainException("A pessoa informada para vínculo não existe.");

        var placaSegura = dto.Placa ?? string.Empty;
        var placaLimpa = Regex.Replace(placaSegura, @"[^a-zA-Z0-9]", string.Empty).ToUpperInvariant();

        var existente = await _veiculoRepository.ObterPorPlacaAsync(placaLimpa);
        if (existente is not null)
            throw new DomainException("Já existe um veículo cadastrado com esta placa.");

        var veiculo = new Veiculo(
            placaSegura,
            dto.AnoFabricacao,
            dto.Marca ?? string.Empty,
            dto.Modelo ?? string.Empty,
            pessoaExistente.Id);

        await _veiculoRepository.AdicionarAsync(veiculo);

        var veiculoSalvo = await _veiculoRepository.ObterPorIdAsync(veiculo.Id);
        return MapearParaResponse(veiculoSalvo!);
    }

    public async Task<VeiculoResponseDto> AtualizarAsync(Guid id, VeiculoRequestDto dto) {
        var veiculo = await _veiculoRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Veículo não encontrado.");

        var pessoaExistente = await _pessoaRepository.ObterPorIdAsync(dto.PessoaId)
            ?? throw new DomainException("A pessoa informada para vínculo não existe.");

        var placaSegura = dto.Placa ?? string.Empty;
        var placaLimpa = Regex.Replace(placaSegura, @"[^a-zA-Z0-9]", string.Empty).ToUpperInvariant();

        var existente = await _veiculoRepository.ObterPorPlacaAsync(placaLimpa);
        if (existente is not null && existente.Id != id)
            throw new DomainException("Esta placa já pertence a outro veículo.");

        veiculo.AtualizarDados(
            placaSegura,
            dto.AnoFabricacao,
            dto.Marca ?? string.Empty,
            dto.Modelo ?? string.Empty,
            pessoaExistente.Id);

        await _veiculoRepository.AtualizarAsync(veiculo);

        var atualizado = await _veiculoRepository.ObterPorIdAsync(id);
        return MapearParaResponse(atualizado!);
    }

    public async Task RemoverAsync(Guid id) {
        var veiculo = await _veiculoRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Veículo não encontrado.");
        await _veiculoRepository.RemoverAsync(veiculo);
    }

    private static VeiculoResponseDto MapearParaResponse(Veiculo veiculo) => new(
        veiculo.Id,
        veiculo.Placa,
        veiculo.AnoFabricacao,
        veiculo.Marca,
        veiculo.Modelo,
        veiculo.PessoaId,
        veiculo.Pessoa?.NomeCompleto ?? "Não identificado"
    );
}