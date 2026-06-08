using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitGuardAI.Application.DTOs;
using OrbitGuardAI.Application.Exceptions;
using OrbitGuardAI.Domain.Entities;
using OrbitGuardAI.Domain.Interfaces;

namespace OrbitGuardAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AreasRiscoController : ControllerBase
{
    private readonly IAreaRiscoRepositorio _repo;
    public AreasRiscoController(IAreaRiscoRepositorio repo) => _repo = repo;

    /// <summary>Lista todas as áreas vulneráveis monitoradas.</summary>
    [HttpGet]
    public async Task<IEnumerable<AreaRisco>> Listar() => await _repo.ListarAsync();

    /// <summary>Detalha uma área de risco.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AreaRisco>> Obter(Guid id)
    {
        var a = await _repo.ObterPorIdAsync(id) ?? throw new RecursoNaoEncontradoException("Área de risco");
        return Ok(a);
    }

    /// <summary>Cadastra uma nova área de risco (gestor ou admin).</summary>
    [HttpPost]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<ActionResult<AreaRisco>> Criar([FromBody] AreaRiscoDTO dto)
    {
        var a = new AreaRisco
        {
            Nome = dto.Nome,
            Municipio = dto.Municipio,
            Estado = dto.Estado,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            RaioKm = dto.RaioKm,
            PopulacaoEstimada = dto.PopulacaoEstimada
        };
        await _repo.AdicionarAsync(a);
        return CreatedAtAction(nameof(Obter), new { id = a.Id }, a);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Remover(Guid id)
    {
        await _repo.RemoverAsync(id);
        return NoContent();
    }
}