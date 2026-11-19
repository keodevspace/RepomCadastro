using Microsoft.EntityFrameworkCore;
using Repom.Domain.Entities;
using Repom.Domain.Interfaces;
using Repom.Infrastructure.Data;

namespace Repom.Infrastructure.Repositories;

public class VeiculoRepository : IVeiculoRepository {
    private readonly AppDbContext _context;
    public VeiculoRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Veiculo>> ObterTodosAsync() =>
        await _context.Veiculos.Include(v => v.Pessoa).AsNoTracking().ToListAsync();

    public async Task<Veiculo?> ObterPorIdAsync(Guid id) =>
        await _context.Veiculos.Include(v => v.Pessoa).FirstOrDefaultAsync(v => v.Id == id);

    public async Task<Veiculo?> ObterPorPlacaAsync(string placa) =>
        await _context.Veiculos.AsNoTracking().FirstOrDefaultAsync(v => v.Placa == placa);

    public async Task AdicionarAsync(Veiculo veiculo) {
        await _context.Veiculos.AddAsync(veiculo);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Veiculo veiculo) {
        _context.Veiculos.Update(veiculo);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Veiculo veiculo) {
        _context.Veiculos.Remove(veiculo);
        await _context.SaveChangesAsync();
    }
}