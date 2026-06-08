using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitGuardAI.Application.DTOs;
using OrbitGuardAI.Domain.Entities;
using OrbitGuardAI.Domain.Interfaces;

namespace OrbitGuardAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TelemetriaController : ControllerBase
{
    private readonly ILeituraRepositorio _repo;
    private readonly ISatelliteDataGateway _gateway;

    public TelemetriaController(ILeituraRepositorio repo, ISatelliteDataGateway gateway)
    {
        _repo = repo;
        _gateway = gateway;
    }

    /// <summary>Recebe leituras IoT (gateway de campo).</summary>
    [HttpPost("leituras")]
    public async Task<IActionResult> RegistrarLeitura([FromBody] LeituraDTO dto)
    {
        var leitura = new LeituraTelemetrica
        {
            DispositivoId = dto.DispositivoId,
            DataHora = dto.DataHora == default ? DateTime.UtcNow : dto.DataHora,
            Indicadores = dto.Indicadores
        };
        await _repo.AdicionarAsync(leitura);
        return Ok(new { leitura.Id });
    }

    /// <summary>Lista o histórico de leituras de um dispositivo.</summary>
    [HttpGet("leituras/{dispositivoId:guid}")]
    public async Task<IEnumerable<LeituraTelemetrica>> Historico(
        Guid dispositivoId, [FromQuery] int horas = 24) =>
        await _repo.ListarPorDispositivoAsync(dispositivoId, DateTime.UtcNow.AddHours(-horas));

    /// <summary>Consulta dados orbitais simulados (NASA / INPE / Copernicus).</summary>
    [HttpGet("orbital")]
    public async Task<IEnumerable<LeituraTelemetrica>> ConsultarOrbital(
        [FromQuery] double lat, [FromQuery] double lon, [FromQuery] int horas = 24) =>
        await _gateway.ObterDadosOrbitaisAsync(lat, lon, DateTime.UtcNow.AddHours(-horas));
}