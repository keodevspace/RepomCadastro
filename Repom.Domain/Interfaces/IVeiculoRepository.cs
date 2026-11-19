using Repom.Domain.Entities;

namespace Repom.Domain.Interfaces;

public interface IVeiculoRepository {
    Task<IEnumerable<Veiculo>> ObterTodosAsync();
    Task<Veiculo?> ObterPorIdAsync(Guid id);
    Task<Veiculo?> ObterPorPlacaAsync(string placa);
    Task AdicionarAsync(Veiculo veiculo);
    Task AtualizarAsync(Veiculo veiculo);
    Task RemoverAsync(Veiculo veiculo);
}