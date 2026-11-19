using Repom.Domain.Entities;

namespace Repom.Domain.Interfaces;

public interface IPessoaRepository {
    Task<IEnumerable<Pessoa>> ObterTodasAsync();
    Task<Pessoa?> ObterPorIdAsync(Guid id);
    Task<Pessoa?> ObterPorCpfAsync(string cpf);
    Task AdicionarAsync(Pessoa pessoa);
    Task AtualizarAsync(Pessoa pessoa);
    Task RemoverAsync(Pessoa pessoa);
}