using Microsoft.EntityFrameworkCore;
using Repom.Domain.Entities;
using Repom.Domain.Interfaces;
using Repom.Infrastructure.Data;

namespace Repom.Infrastructure.Repositories;

public class PessoaRepository : IPessoaRepository {
    private readonly AppDbContext _context;
    public PessoaRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Pessoa>> ObterTodasAsync() =>
        await _context.Pessoas.Include(p => p.Veiculos).AsNoTracking().ToListAsync();

    public async Task<Pessoa?> ObterPorIdAsync(Guid id) =>
        await _context.Pessoas.Include(p => p.Veiculos).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Pessoa?> ObterPorCpfAsync(string cpf) =>
        await _context.Pessoas.AsNoTracking().FirstOrDefaultAsync(p => p.Cpf == cpf);

    public async Task AdicionarAsync(Pessoa pessoa) {
        await _context.Pessoas.AddAsync(pessoa);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Pessoa pessoa) {
        _context.Pessoas.Update(pessoa);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Pessoa pessoa) {
        _context.Pessoas.Remove(pessoa);
        await _context.SaveChangesAsync();
    }
}