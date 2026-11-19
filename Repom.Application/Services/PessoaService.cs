using System.Text.RegularExpressions;
using Repom.Application.DTOs;
using Repom.Domain.Entities;
using Repom.Domain.Exceptions;
using Repom.Domain.Interfaces;

namespace Repom.Application.Services;

public class PessoaService {
    private readonly IPessoaRepository _pessoaRepository;

    public PessoaService(IPessoaRepository pessoaRepository) {
        _pessoaRepository = pessoaRepository;
    }

    public async Task<IEnumerable<PessoaResponseDto>> ListarTodasAsync() {
        var pessoas = await _pessoaRepository.ObterTodasAsync();
        return pessoas.Select(MapearParaResponse);
    }

    public async Task<PessoaResponseDto> ObterPorIdAsync(Guid id) {
        var pessoa = await _pessoaRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Pessoa não encontrada.");
        return MapearParaResponse(pessoa);
    }

    public async Task<PessoaResponseDto> CriarAsync(PessoaRequestDto dto) {
        var cpfLimpo = Regex.Replace(dto.Cpf ?? "", @"[^\d]", "");
        var existente = await _pessoaRepository.ObterPorCpfAsync(cpfLimpo);
        if (existente is not null)
            throw new DomainException("Já existe uma pessoa cadastrada com este CPF.");

        var pessoa = new Pessoa(dto.NomeCompleto, dto.Cpf, dto.DataNascimento, dto.Email, dto.Telefone);
        await _pessoaRepository.AdicionarAsync(pessoa);
        return MapearParaResponse(pessoa);
    }

    public async Task<PessoaResponseDto> AtualizarAsync(Guid id, PessoaRequestDto dto) {
        var pessoa = await _pessoaRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Pessoa não encontrada.");

        var cpfLimpo = Regex.Replace(dto.Cpf ?? "", @"[^\d]", "");
        var existente = await _pessoaRepository.ObterPorCpfAsync(cpfLimpo);
        if (existente is not null && existente.Id != id)
            throw new DomainException("Este CPF já pertence a outro cadastro.");

        pessoa.AtualizarDados(dto.NomeCompleto, dto.Cpf, dto.DataNascimento, dto.Email, dto.Telefone);
        await _pessoaRepository.AtualizarAsync(pessoa);
        return MapearParaResponse(pessoa);
    }

    public async Task RemoverAsync(Guid id) {
        var pessoa = await _pessoaRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Pessoa não encontrada.");
        await _pessoaRepository.RemoverAsync(pessoa);
    }

    private static PessoaResponseDto MapearParaResponse(Pessoa p) => new(
        p.Id,
        p.NomeCompleto,
        p.Cpf,
        p.DataNascimento,
        p.Email,
        p.Telefone,
        p.Veiculos.Select(v => new VeiculoResumoDto(v.Id, v.Placa, v.AnoFabricacao, v.Marca, v.Modelo))
    );
}