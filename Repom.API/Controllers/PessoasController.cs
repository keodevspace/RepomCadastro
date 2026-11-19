using Microsoft.AspNetCore.Mvc;
using Repom.Application.DTOs;
using Repom.Application.Services;

namespace Repom.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PessoasController : ControllerBase {
    private readonly PessoaService _service;
    public PessoasController(PessoaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _service.ListarTodasAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id) => Ok(await _service.ObterPorIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] PessoaRequestDto dto) {
        var criada = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = criada.Id }, criada);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] PessoaRequestDto dto) =>
        Ok(await _service.AtualizarAsync(id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id) {
        await _service.RemoverAsync(id);
        return NoContent();
    }
}