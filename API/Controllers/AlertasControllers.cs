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
public class AlertasController : ControllerBase
{
    private readonly IAlertaService _alertaSvc;
    private readonly IPreditorClimaticoService _preditor;
    private readonly IAreaRiscoRepositorio _areaRepo;
    private readonly ILeituraRepositorio _leiturasRepo;
    private readonly ISatelliteDataGateway _gateway;

    public AlertasController(
        IAlertaService alertaSvc,
        IPreditorClimaticoService preditor,
        IAreaRiscoRepositorio areaRepo,
        ILeituraRepositorio leiturasRepo,
        ISatelliteDataGateway gateway)
    {
        _alertaSvc = alertaSvc;
        _preditor = preditor;
        _areaRepo = areaRepo;
        _leiturasRepo = leiturasRepo;
        _gateway = gateway;
    }

    /// <summary>Lista alertas ativos (Dashboard Web e App Mobile consomem aqui).</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IEnumerable<Alerta>> Listar() => await _alertaSvc.ListarAtivosAsync();

    /// <summary>
    /// Endpoint principal: roda inferência preditiva sobre uma área de risco e,
    /// se houver risco, gera + persiste + dispara o alerta.
    /// </summary>
    [HttpPost("avaliar/{areaId:guid}")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<ActionResult<Alerta>> AvaliarArea(Guid areaId)
    {
        var area = await _areaRepo.ObterPorIdAsync(areaId)
            ?? throw new RecursoNaoEncontradoException("Área de risco");

        // Combina dados orbitais + IoT
        var orbital = await _gateway.ObterDadosOrbitaisAsync(area.Latitude, area.Longitude, DateTime.UtcNow.AddHours(-24));
        var iot = await _leiturasRepo.ListarAsync();
        var historico = orbital.Concat(iot).ToList();

        var previsao = await _preditor.PreverAsync(area, historico);
        var alerta = await _alertaSvc.GerarAlertaAsync(area, previsao);
        return Ok(alerta);
    }

    [HttpPost("{id:guid}/encerrar")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> Encerrar(Guid id)
    {
        await _alertaSvc.EncerrarAsync(id);
        return NoContent();
    }
}